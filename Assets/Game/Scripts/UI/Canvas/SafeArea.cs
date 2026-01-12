using UnityEngine;

namespace Game.Scripts.UI
{
	public class SafeArea : MonoBehaviour
	{
		[SerializeField] private RectTransform _panel;

		private ScreenOrientation _orientation;

		public RectTransform Panel => _panel ??= gameObject.GetComponent<RectTransform>();

		private void Start()
		{
			Refresh();
		}

		private void Awake()
		{
			Refresh();
		}

		private void FixedUpdate()
		{
			if (_orientation != Screen.orientation)
				Refresh();
		}

		private void Refresh()
		{
#if TEST_SAFE_AREA
			var area = new Rect(132, 63, 2172, 1062);
#else
			var area = Screen.safeArea;
#endif
			ApplySafeArea(area);
		}

		private void ApplySafeArea(Rect safeArea)
		{
			_orientation = Screen.orientation;

			var anchorMin = safeArea.position;
			var anchorMax = safeArea.position + safeArea.size;
			anchorMin.x /= Screen.width;
			anchorMin.y /= Screen.height;
			anchorMax.x /= Screen.width;
			anchorMax.y /= Screen.height;
			Panel.anchorMin = anchorMin;
			Panel.anchorMax = anchorMax;
		}
	}
}