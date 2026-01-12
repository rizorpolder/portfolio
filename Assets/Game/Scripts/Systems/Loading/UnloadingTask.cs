using System;

namespace Game.Scripts.Systems.Loading
{
	public class UnloadingTask
	{
		#region Fields

		private string _sceneName;
		private LoadingScreenType _loadingScreenType = LoadingScreenType.Default;
		private bool _hideLoadingView = true;
		private Action _postAnimation = null;
		private Action _callback = null;

		#endregion

		#region Props

		public string SceneName => _sceneName;
		public LoadingScreenType LoadingScreenType => _loadingScreenType;
		public bool HideLoadingView => _hideLoadingView;
		public Action PostAnimationAction => _postAnimation;
		public Action Callback => _callback;

		#endregion

		public UnloadingTask SetSceneName(string sceneName)
		{
			_sceneName = sceneName;
			return this;
		}

		public UnloadingTask SetLoadingScreenType(LoadingScreenType type)
		{
			_loadingScreenType = type;
			return this;
		}

		public UnloadingTask SetHideLoadingView(bool value)
		{
			_hideLoadingView = value;
			return this;
		}

		public UnloadingTask AddPostAnimationAction(Action action)
		{
			_postAnimation = action;
			return this;
		}

		public UnloadingTask AddCallback(Action callback)
		{
			_callback = callback;
			return this;
		}
	}
}