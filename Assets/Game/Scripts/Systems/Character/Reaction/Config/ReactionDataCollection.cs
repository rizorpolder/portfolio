using System.Collections.Generic;
using Game.Scripts.Systems.Character.Reaction.Data;
using UnityEngine;

namespace Game.Scripts.Systems.Character.Reaction.Config
{
	[CreateAssetMenu(fileName = "Reaction Data Collection", menuName = "Project/Reactions/Reaction Data Collection")]
	public class ReactionDataCollection : ScriptableObject
	{
		public string CharacterID;

		[Header("Start delay in seconds")]
		[Range(0, 100)]
		public float RandomStartDelayMin;

		[Range(0, 100)]
		public float RandomStartDelayMax;

		[Header("Path time in seconds")]
		public float RandomPathTimeMin;

		public float RandomPathTimeMax;

		[Header("Probability of reactions")]
		public List<ReactionProbability> Probabilites;

		[Range(0, 100)]
		public float PathActionProbability;

		[Header("Reactions")]
		// public List<MonologueReactionData> Monologues;
		public List<PathReactionData> Paths;

		public List<ReactionData> Animations;
		public List<DialogueReactionData> Dialogues;
		public List<MonologueReactionData> Monologues;
	}

	[System.Serializable]
	public class ReactionProbability
	{
		public ReactionType Type;

		[Range(0, 100)]
		public float Probability;
	}
}