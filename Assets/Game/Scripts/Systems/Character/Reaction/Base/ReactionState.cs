namespace Game.Scripts.Systems.Character.Reaction
{
	public enum ReactionState
	{
		/// <summary>
		/// Проиграть реакцию
		/// </summary>
		Play,

		/// <summary>
		/// В режиме проигрывания idle-реакций
		/// </summary>
		Idle,

		/// <summary>
		/// Прервать реакцию
		/// </summary>
		Terminated
	}
}