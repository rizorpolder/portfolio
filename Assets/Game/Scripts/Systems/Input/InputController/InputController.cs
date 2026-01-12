using System;
using System.Collections.Generic;
using System.Linq;
using Game.Scripts.Systems.CameraScrollSystem;
using Game.Scripts.Systems.Initialize;
using Game.Scripts.Systems.Input.Data;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Systems.Input
{
	public class InputController : MonoBehaviour, IInputListener, IInputData, IDisposable
	{
		public event Action<TouchData> TouchPositionChanged;
		public event Action<TouchData> TouchChanged;
		public event Action<TouchData> ScrollDataChanged;

		[SerializeField] InputModule _inputModule;

		[Inject] InputHandler _inputHandler;
		[Inject] SceneContainerCoordinator _coordinator;

		
		private readonly Dictionary<int, TouchData> _touchesCache = new Dictionary<int, TouchData>();
		private readonly List<int> _completedTouches = new List<int>();

		private CameraScrollHandler _scrollHandler;
		private CameraPositionClamping _positionClamping;
		private ZoomInputHandler _zoomInputHandler;

		public bool IsSingleTouch => _touchesCache.Count == 1;

		private bool _isActive = false;

		private void Start()
		{
			var _diContainer = _coordinator.GetContainer();
			
			_scrollHandler = new CameraScrollHandler();
			_diContainer.Inject(_scrollHandler);
			_scrollHandler.Initialize();

			_positionClamping = new CameraPositionClamping();
			_diContainer.Inject(_positionClamping);
			_positionClamping.Initialize();

			if (Application.isMobilePlatform)
				_zoomInputHandler = new TouchZoomInputHandler();
			else
				_zoomInputHandler = new MouseZoomInputHandler();
			_diContainer.Inject(_zoomInputHandler);
			_zoomInputHandler.Initialize();

			Activate();
		}

		private void Activate()
		{
			_isActive = true;
		}

		public void Update()
		{
			if (!_isActive)
				return;

			UpdateInput();

			foreach (var kv in _touchesCache)
			{
				kv.Value.NotifyChanges();
				if (kv.Value.isTouchUp)
					_completedTouches.Add(kv.Key);
			}

			_inputHandler.ProcessCameraSpeed();
			if (!Application.isMobilePlatform)
				_zoomInputHandler.Handle();

			_inputHandler.ProcessInertia();
			_positionClamping.Process();


			if (_completedTouches.Count > 0)
			{
				foreach (var touchId in _completedTouches)
					_touchesCache.Remove(touchId);
				_completedTouches.Clear();
			}

			if (_touchesCache.Count == 0)
			{
				if (_inputHandler.HasStartTouchPosition)
					_inputHandler.RemoveStartTouchPosition();
			}
		}

		private void UpdateInput()
		{
			if (Application.isMobilePlatform)
			{
				TouchInput();
			}
			else
			{
				MouseInput();
			}
		}

		private void TouchInput()
		{
			if (UnityEngine.Input.touchCount > 0)
			{
				for (int i = 0; i < UnityEngine.Input.touchCount; i++)
				{
					var touch = UnityEngine.Input.GetTouch(i);
					var touchEntity = GetTouchData(touch.fingerId);
					var touchPosition = touch.position;

					touchEntity.isTouchDown = touch.phase == TouchPhase.Began;
					touchEntity.isTouchUp = touch.phase == TouchPhase.Ended;

					if (touchEntity.isTouchDown || !touchEntity.HasStartTouchPosition)
					{
						touchEntity.ReplaceStartTouchPosition(touchPosition);

						GameObject target = _inputModule.GetPointerTarget(touch.fingerId);
						touchEntity.ReplaceTouchTarget(target);
					}

					touchEntity.ReplaceTouchPosition(touchPosition);
					touchEntity.ReplaceTouch(touch);
				}
			}
		}

		private void MouseInput()
		{
			if (UnityEngine.Input.GetMouseButton(0) || UnityEngine.Input.GetKeyUp(KeyCode.Mouse0))
			{
				var touchEntity = GetTouchData(0);
				var touchPosition = UnityEngine.Input.mousePosition;

				touchEntity.isTouchDown = UnityEngine.Input.GetKeyDown(KeyCode.Mouse0);
				touchEntity.isTouchUp = UnityEngine.Input.GetKeyUp(KeyCode.Mouse0);

				if (touchEntity.isTouchDown || !touchEntity.HasStartTouchPosition)
				{
					touchEntity.ReplaceStartTouchPosition(touchPosition);

					GameObject target = _inputModule.GetPointerTarget(-1);

					touchEntity.ReplaceTouchTarget(target);
				}

				touchEntity.ReplaceTouchPosition(touchPosition);
			}
		}

		private TouchData GetTouchData(int fingerId)
		{
			TouchData data;
			if (!_touchesCache.TryGetValue(fingerId, out data) || data == null)
			{
				data = new TouchData();
				data.TouchChanged += TouchChangedHandler;
				data.ScrollChanged += ScrollChangedHandler;
				data.TouchPositionChanged += TouchPositionHandler;
				_touchesCache[fingerId] = data;
			}

			return data;
		}

		private void TouchChangedHandler(TouchData data)
		{
			TouchChanged?.Invoke(data);
		}

		private void TouchPositionHandler(TouchData data)
		{
			TouchPositionChanged?.Invoke(data);
		}

		private void ScrollChangedHandler(TouchData data)
		{
			ScrollDataChanged?.Invoke(data);
		}

		public List<TouchData> GetTouches()
		{
			return _touchesCache.Values.ToList();
		}

		public void Dispose()
		{
			this._scrollHandler.Dispose();
		}
	}
}