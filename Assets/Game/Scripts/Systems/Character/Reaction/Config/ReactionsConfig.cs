using System.Collections.Generic;
using Game.Scripts.Systems.Character.Reaction.Data;
using UnityEngine;

namespace Game.Scripts.Systems.Character.Reaction.Config
{
	[CreateAssetMenu(fileName = "ReactionsConfig", menuName = "Project/Reactions/Reactions Config")]
	public class ReactionsConfig : ScriptableObject
	{
		[Tooltip("Список персонажей с реакциями по ним.")]
		public List<ReactionDataCollection> collections;

		[SerializeField] private float idleTimeDelay = 2f;
		public float IdleTimeDelay => this.idleTimeDelay;

		[SerializeField] private int pathFindingRadius = 350;
		public int PathFindingRadius => this.pathFindingRadius;

		[SerializeField] private float pathFindingMultiplier = 0.25f;
		public float PathFindingMultiplier => this.pathFindingMultiplier;

		public bool TryToGetCollection(string id, out ReactionDataCollection collection)
		{
			collection = collections.Find(x => x.CharacterID == id);
			if (collection == null)
				return false;

			return true;
		}

		public bool TryToGetReactions(string id, out Dictionary<ReactionType, List<ReactionData>> result)
		{
			result = null;

			var collection = collections.Find(x => x.CharacterID == id);
			if (collection == null)
				return false;

			result = new Dictionary<ReactionType, List<ReactionData>>();

			// foreach (var reactionData in collection.Animations)
			// {
			// 	if (!result.ContainsKey(ReactionType.Animation))
			// 		result.Add(ReactionType.Animation, new List<IReaction>());
			//
			// 	var reaction = new SequencerReaction();
			// 	reaction.ReactionData = reactionData;
			// 	result[ReactionType.Animation].Add(reaction);
			// }


			foreach (var reactionData in collection.Monologues)
			{
				if (!result.ContainsKey(ReactionType.Monologue))
					result.Add(ReactionType.Monologue, new List<ReactionData>());

				result[ReactionType.Monologue].Add(reactionData);
			}

			foreach (var reactionData in collection.Paths)
			{
				if (!result.ContainsKey(ReactionType.Movement))
					result.Add(ReactionType.Movement, new List<ReactionData>());

				result[ReactionType.Movement].Add(reactionData);
			}

			foreach (var reactionData in collection.Dialogues)
			{
				if (!result.ContainsKey(ReactionType.Dialogue))
					result.Add(ReactionType.Dialogue, new List<ReactionData>());

				result[ReactionType.Dialogue].Add(reactionData);
			}

			return true;
		}
	}
}