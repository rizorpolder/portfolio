namespace Game.Scripts.Systems.Character
{
	public enum State
	{
		None,

		///<summary>В свободном полёте - управляется idle-системой </summary>
		Idle,

		///<summary>Под управлением игрока. Логически Может быть только один персонаж в данном стейте. </summary>
		UnderPlayerControl,

		MoveToObject,
	}
	
	public static class CharacterStateExtensions
	{
		public static bool IsIdle(this State state) 
		{
			return state.Equals(State.Idle);
		}

		public static bool Busy(this State state)
		{
			return !state.Equals(State.Idle) && !state.Equals(State.UnderPlayerControl);
		}

	}
}