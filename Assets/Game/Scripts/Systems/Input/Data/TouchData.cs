using System;
using UnityEngine;

namespace Game.Scripts.Systems.Input.Data
{
	public class TouchData
	{
		public event Action<TouchData> TouchChanged;
		public event Action<TouchData> ScrollChanged;
		public event Action<TouchData> TouchPositionChanged;

		public bool HasTouchChanges { get; private set; }

		public bool HasTouchPositionChanges { get; private set; }
		public bool HasScrollChanges { get; private set; }

		private Vector3? _startTouchPosition;
		public Vector3 StartTouchPosition => _startTouchPosition ?? Vector3.zero;

		public bool HasStartTouchPosition => _startTouchPosition.HasValue;

		public void ReplaceStartTouchPosition(Vector3 touchPosition)
		{
			_startTouchPosition = touchPosition;
		}

		private GameObject _touchTarget;
		public GameObject TouchTarget => _touchTarget;
		public bool HasTouchTarget => _startTouchPosition.HasValue;

		public void ReplaceTouchTarget(GameObject touchTarget)
		{
			_touchTarget = touchTarget;
		}

		private Vector2? _touchPosition;
		public Vector2 TouchPosition => _touchPosition ?? Vector2.zero;
		public bool HasTouchPosition => _touchPosition.HasValue;

		public void ReplaceTouchPosition(Vector2 touchPosition)
		{
			_touchPosition = touchPosition;
			HasScrollChanges = true;
			HasTouchPositionChanges = true;
		}

		private Touch? _touch;
		public Touch? Touch => _touch;
		public bool HasTouch => _touch.HasValue;

		public void ReplaceTouch(Touch touch)
		{
			_touch = touch;
			HasTouchChanges = true;
		}

		private bool _isTouchUp;

		public bool isTouchUp
		{
			get => _isTouchUp;
			set
			{
				if (_isTouchUp == value)
					return;

				_isTouchUp = value;
				HasScrollChanges = true;
			}
		}

		private bool _isTouchDown;

		public bool isTouchDown
		{
			get => _isTouchDown;
			set => _isTouchDown = value;
		}

		public void NotifyChanges()
		{
			if (HasTouchChanges)
			{
				TouchChanged?.Invoke(this);
				HasTouchChanges = false;
			}

			if (HasScrollChanges)
			{
				ScrollChanged?.Invoke(this);
				HasScrollChanges = false;
			}

			if (HasTouchPositionChanges)
			{
				TouchPositionChanged?.Invoke(this);
				HasTouchPositionChanges = false;
			}
		}
	}
}