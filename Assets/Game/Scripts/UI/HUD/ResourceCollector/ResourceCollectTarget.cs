using Game.Scripts.UI.Common;
using UnityEngine;

namespace UI.HUD.ResourceCollector
{
	public struct ResourceCollectTarget
	{
		public static Vector3 DefaultSize = new Vector3(88, 88);

		public Vector3 Position;
		public Vector3 SizeDelta;
		public Quaternion Rotation;

		public RectTransform RectTransform { get; private set; }
		
		private Canvas _canvas;

		public Canvas Canvas
		{
			get
			{
				_canvas = GlobalUI.Instance.GlobalCanvas;
				if (!_canvas.worldCamera)
					_canvas.worldCamera = Camera.main;
				return _canvas;
			}
		}

		public static ResourceCollectTarget FromWorldPosition(Vector3 position)
		{
			var result = new ResourceCollectTarget();
			result.SetPositionFromWorld(position);
			result.SizeDelta = DefaultSize;
			return result;
		}

		public static ResourceCollectTarget FromRectTransform(RectTransform rt)
		{
			var result = new ResourceCollectTarget();
			result.RectTransform = rt;
			return result;
		}

		public static ResourceCollectTarget Empty = new ResourceCollectTarget
		{
			Position = Vector3.zero,
			SizeDelta = DefaultSize,
		};

		public Vector3 GetScreenPositionFromWorld(Vector3 worldPos)
		{
			var canvas = Canvas;
			var screenPosition = canvas.worldCamera.WorldToScreenPoint(worldPos);
			screenPosition = new Vector2(screenPosition.x / canvas.scaleFactor, screenPosition.y / canvas.scaleFactor);
			return screenPosition;
		}

		private void SetPositionFromWorld(Vector3 worldPos)
		{
			Position = GetScreenPositionFromWorld(worldPos);
		}

		public Vector3 GetActualPosition()
		{
			if (RectTransform)
			{
				Position = Canvas.worldCamera.WorldToScreenPoint(RectTransform.position);
				var canvas = RectTransform.GetComponentInParent<Canvas>();
				if (canvas)
				{
					Position = new Vector2(Position.x / canvas.scaleFactor, Position.y / canvas.scaleFactor);
				}
			}

			return Position;
		}
	}

	public static class ResourceCollectTargetExtensions
	{
		public static ResourceCollectTarget GetResourceCollectTargetData(this Transform transform)
		{
			var rt = transform.GetComponent<RectTransform>();

			var result = ResourceCollectTarget.FromRectTransform(rt);
			var rect = rt.rect;
			var sizeDelta = new Vector2(rect.width, rect.height);

			result.GetActualPosition();
			result.Rotation = transform.rotation;
			result.SizeDelta = sizeDelta;

			return result;
		}

		public static void ApplyTargetResourceCollectData(this RectTransform transform, ResourceCollectTarget data)
		{
			var rectTrasform = transform.GetComponent<RectTransform>();
			if (rectTrasform)
			{
				rectTrasform.sizeDelta = data.SizeDelta;
			}

			transform.anchoredPosition = data.Position;
		}
	}
}