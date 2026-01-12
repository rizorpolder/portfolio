using Zenject;

namespace Game.Scripts.Systems.Character.Reaction
{
	public class ReactionFactory
	{
		[Inject] private DiContainer diContainer;

		public T GetReaction<T>() where T : new()
		{
			var result = new T();
			diContainer.Inject(result);
			return result;
		}
	}
}