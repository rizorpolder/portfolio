using UnityEngine;

namespace Game.Scripts.Systems.Character.Reaction.Data
{
	[CreateAssetMenu(fileName = "DialogueReaction", menuName = "Project/Reactions/DialogueReaction")]
	public class DialogueReactionData : ReactionData
	{
		public string dialogueListener;
	}
}