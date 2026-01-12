using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Systems.Character.Reaction.Data
{
	[CreateAssetMenu(fileName = "PathReaction", menuName = "Project/Reactions/PathReaction")]

	public class PathReactionData : ReactionData
	{
		public List<string> Path;
	}
}