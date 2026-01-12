namespace Game.Scripts.Systems.Character.Reaction
{
	public delegate void OnReactionFinishedHandler();

	public interface IReaction
	{
		void Start(string target);
		void Stop();
		bool CheckConditions();
		bool CheckOtherTargets(string target2);
		float GetChance();
		float GetDefaultChance();
		void SetChance(float chance);

		event OnReactionFinishedHandler OnFinished; // вызывается при успешном окончании реакции
	}
}