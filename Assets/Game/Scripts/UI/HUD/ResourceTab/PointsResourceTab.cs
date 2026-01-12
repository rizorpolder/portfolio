using Game.Scripts.Data;
using Game.Scripts.Systems.PlayerController;
using Zenject;

namespace Game.Scripts.UI.ResourceTab
{
	public class PointsResourceTab : AResourceTab
	{
		public override ResourceType Type => ResourceType.Points;
		[Inject] private IPlayerListener _playerListener;
		protected override void SubscribeOnResourceChange()
		{
			_playerListener.OnPlayerPointChanged += ResourceChangeHandler;
		}

		protected override void UnubscribeResourceChange()
		{
			_playerListener.OnPlayerPointChanged -= ResourceChangeHandler;
		}
	}
}