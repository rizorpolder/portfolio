using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Systems.DialogueSystem
{
	[Serializable,CreateAssetMenu(fileName = "Conversation", menuName = "Project/Dialogue System/Conversation")]
	public class ConversationConfig : ScriptableObject
	{
		[SerializeField] public string ConversationID;
		[SerializeField] public List<DialogueEntryConfig> DialogueEntries = new List<DialogueEntryConfig>();
	}
}