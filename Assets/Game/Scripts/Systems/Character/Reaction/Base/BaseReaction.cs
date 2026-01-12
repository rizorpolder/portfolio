using Game.Scripts.Systems.Character.Reaction.Data;

namespace Game.Scripts.Systems.Character.Reaction
{
	public class BaseReaction : IReaction
	{
		public event OnReactionFinishedHandler OnFinished;
		protected string _target;

		protected ReactionData reactionData;

		private float _chance = 0;

		public ReactionData ReactionData
		{
			get { return reactionData; }
			set { reactionData = value; }
		}

		public virtual void Start(string target)
		{
			this._target = target;
		}

		public virtual void Stop()
		{
		}

		protected virtual void OnFinish()
		{
			OnFinished?.Invoke();
			OnFinished = null;
		}

		public virtual bool CheckConditions()
		{
			return reactionData;
		}

		public virtual bool CheckOtherTargets(string target2)
		{
			return false;
		}

		public float GetChance()
		{
			return _chance;
		}

		public float GetDefaultChance()
		{
			return reactionData.Chance;
		}

		public void SetChance(float chance)
		{
			_chance = chance;
		}
	}
}