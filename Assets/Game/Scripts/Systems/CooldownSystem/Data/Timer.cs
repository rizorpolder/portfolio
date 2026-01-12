using Game.Scripts.Helpers;

namespace CooldownSystem
{
	public class Timer
	{
		private Cooldown _cooldown;
		public Cooldown Cooldown => _cooldown;

		private bool _isCharacter;
		public bool IsCharacterCooldown => _isCharacter;

		public void SetCooldown(Cooldown cooldown)
		{
			_cooldown = cooldown;
		}

		public bool IsDefault()
		{
			return _cooldown.CompletionDate.Equals(TimeHelper.DataConst);
		}

		public bool IsActual()
		{
			return !_cooldown.IsComplete;
		}

		public void AssignCharacter()
		{
			_isCharacter = true;
		}
	}
}