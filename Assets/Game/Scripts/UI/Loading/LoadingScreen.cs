using System;
using Game.Scripts.Systems.Loading;
using UnityEngine;

namespace Game.Scripts.UI.Loading
{
	public class LoadingScreen : MonoBehaviour
	{
		[SerializeField] private LoadingView _darkView;
		[SerializeField] private LoadingView _defaultView;
		private LoadingView _lastUsedView;
		public LoadingView LastUsedView => _lastUsedView;

		public LoadingView GetLoadingView(LoadingScreenType type)
		{
			switch (type)
			{
				case LoadingScreenType.Dark:
					_lastUsedView = _darkView;
					break;

				case LoadingScreenType.Default:
				default:
					_lastUsedView = _defaultView;
					break;
			}

			return _lastUsedView;
		}

		private void OnEnable()
		{
			_darkView.Hide();
			_defaultView.Hide();
		}
	}
}