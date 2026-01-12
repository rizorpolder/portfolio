using System.Collections.Generic;
using Game.Scripts.Systems.DialogueSystem.Data;
using UnityEngine;

namespace Game.Scripts.Systems.DialogueSystem
{
	[CreateAssetMenu(menuName = "Project/Dialogue System/DialogueDB")]
	public class DialogueDB : ScriptableObject
	{
		[SerializeField] private ActorsConfig _actors;
		[SerializeField] private List<ConversationConfig> _conversations;

		public DialogueDatabase GetInitialDB()
		{
			var database = new DialogueDatabase();
			foreach (var actorConfig in _actors.Actors)
			{
				var actor = new Actor(actorConfig);
				database.AddActor(actor);
			}

			foreach (var conversationConfig in _conversations)
			{
				var conversation = new Conversation(conversationConfig);
				database.AddConversation(conversation);
			}

			return database;
		}
	}
}