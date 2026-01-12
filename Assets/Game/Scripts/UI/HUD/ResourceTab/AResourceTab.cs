using Game.Scripts.Data;
using Game.Scripts.Systems.PlayerController;
using UnityEngine;
using Zenject;

namespace Game.Scripts.UI.ResourceTab
{
	public abstract class AResourceTab : ATab
	{
		[SerializeField] protected ResourceTabPresenter presenter;
		public abstract ResourceType Type { get; }

		[Inject] protected IPlayerCommand _playerCommand;
		
		protected override void OnEnableAction()
		{
			
			SubscribeOnResourceChange();
			
			Refresh();

			base.OnEnableAction();
		}

		protected abstract void SubscribeOnResourceChange();
		protected abstract void UnubscribeResourceChange();

		protected virtual float GetProgress()
		{
			return 0;
		}

		protected void ResourceChangeHandler(int prev, int newValue)
		{
			Refresh();
		}

		public override void Refresh()
		{
			var resource = new Resource
			{
				Type = Type
			};
			resource.Value = _playerCommand.GetResourceCount(resource);
			presenter.Refresh(resource, GetProgress());
		}

		public void RefreshIcons()
		{
			var resource = new Resource
			{
				Type = Type
			};
			presenter.RefreshIcon(resource);
		}

		public virtual void ChangeCounter(int delta)
		{
			presenter.ChangeCounter(delta);
		}
		
		protected override void OnDisableAction()
		{
			base.OnDisableAction();
			UnubscribeResourceChange();
		}
	}
}