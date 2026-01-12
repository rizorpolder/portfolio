using System;

namespace Game.Scripts.Systems.PlayerController
{
	public interface IPlayerListener
	{
		public event Action OnPlayerDataUpdated;
		public event Action<int, int> OnPlayerPointChanged;
	}
}