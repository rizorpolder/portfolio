using Game.Scripts.UI.WindowsSystem;
using UnityEngine;

namespace Game.Scripts.UI.Common
{
	public class GlobalUI : MonoBehaviour
	{
		[SerializeField] private WindowsController _windowsController;
		[SerializeField] private Canvas _tutorialCanvas;
		[SerializeField] private Canvas _globalCanvas;
		[SerializeField] private HUD _hud;

		private Camera _camera;
		public Camera Camera => _camera;

		public bool HasCamera => _camera;

		public WindowsController WindowsController => _windowsController;

		public Canvas TutorialCanvas => _tutorialCanvas;
		public Canvas GlobalCanvas => _globalCanvas;

		public HUD HUD => _hud;

		private void Awake()
		{
			Instance = this;
		}

		public static GlobalUI Instance;

		public void ReplaceCamera(Camera camera)
		{
			_camera = camera;

			GlobalCanvas.worldCamera = _camera;
		}

		public void SetActiveHud(bool isActive)
		{
			_hud.gameObject.SetActive(isActive);
		}
	}
}