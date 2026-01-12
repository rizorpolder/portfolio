using Game.Scripts.UI.Common;
using UnityEngine;
using Zenject;

namespace Game.Scripts.UI
{
	public class SetCanvasCamera : MonoBehaviour
	{
		[SerializeField] private Camera _camera;

		[Inject] private GlobalUI _globalUI;

		private void Awake()
		{
			if (!_camera)
				_camera = Camera.main;

			_globalUI.ReplaceCamera(_camera);
		}

		private void OnEnable()
		{
			if (!_camera)
				_camera = Camera.main;
			_globalUI.ReplaceCamera(_camera);
		}
	}
}