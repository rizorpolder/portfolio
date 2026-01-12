using System.Collections.Generic;
using Game.Scripts.Systems.Actions;
using Game.Scripts.Systems.DialogueSystem.Data;
using UnityEngine;
using CharacterInfo = Game.Scripts.Systems.DialogueSystem.Data.CharacterInfo;

namespace Game.Scripts.Systems.DialogueSystem
{
	public class ConversationModel
	{
		private Dictionary<int, CharacterInfo> m_characterInfoCache = new Dictionary<int, CharacterInfo>();
		private DialogueDatabase _database;

		private CharacterInfo _actorInfo;
		private CharacterInfo _conversantInfo;

		private ConversationState _firstState;
		public ConversationState FirstState => _firstState;

		private Conversation _currentConversation;

		public ConversationModel(DialogueDatabase database, string title)
		{
			_database = database;

			_currentConversation = database.GetConversation(title);
			if (_currentConversation != null)
			{
				_firstState = GetState(_currentConversation.GetFirstDialogueEntry());
			}
		}

		public ConversationState GetState(DialogueEntry entry)
		{
			if (entry == null)
				return null;

			_actorInfo = GetCharacterInfo(entry.ActorID);
			_conversantInfo = GetCharacterInfo(entry.ConversantID);
			var formattedText = entry.Title;
			Subtitle subtitle = new Subtitle()
				.SetSpeakerInfo(_actorInfo)
				.SetListenerInfo(_conversantInfo)
				.SetDialogueEntry(entry)
				.SetText(formattedText)
				.SetAutoTransition(entry.AutoTransition);

			var conversationState = new ConversationStateBuilder();
			conversationState.WithSubtitle(subtitle);
			List<Response> pcResponses = new List<Response>();
			List<Response> npcResponses = new List<Response>();

			EvaluateLinks(entry, pcResponses, npcResponses);

			conversationState.AddPcResponses(pcResponses);
			conversationState.AddNpcResponses(npcResponses);

			List<CustomAction> actions = new List<CustomAction>();
			EvaluateEvents(entry, actions);
			conversationState.AddEvents(actions);

			return conversationState;
		}

		private void EvaluateLinks(DialogueEntry entry, List<Response> pcResponses, List<Response> npcResponses)
		{
			if (entry == null)
				return;

			foreach (var outgoingLink in entry.OutgoingLinks)
			{
				var destinationEntry = _currentConversation.GetDialogueEntry(outgoingLink.TargetEntryID);
				if (destinationEntry == null)
					continue;

				//bool isValid = destinationEntry. проверить на соответствие условию (выполнен квест, и тп)


				if (_actorInfo.IsNPC)
				{
					npcResponses.Add(new Response(destinationEntry, outgoingLink.ButtonText));
				}
				else
				{
					pcResponses.Add(new Response(destinationEntry, outgoingLink.ButtonText));
				}
			}
		}

		private void EvaluateEvents(DialogueEntry entry, List<CustomAction> actions)
		{
			foreach (var endEvent in entry.EndEvents)
			{
				actions.Add(endEvent);
			}
		}

		private CharacterInfo GetCharacterInfo(int id)
		{
			if (!m_characterInfoCache.ContainsKey(id))
			{
				Actor actor = null;
				actor = _database.GetActor(id);
				if (actor == null)
				{
					Debug.LogAssertionFormat("No Actor found with ID {0}", id);
					return null;
				}

				string nameInDatabase = (actor != null) ? actor.Name : string.Empty;

				var actorID = (actor != null) ? actor.ID : id;
				var portrait = (actor != null) ? actor.GetPortraitSprite() : null;

				CharacterInfo characterInfo = new CharacterInfo(actorID, nameInDatabase, actor.CharacterType, portrait);
				m_characterInfoCache.Add(id, characterInfo);
			}

			return m_characterInfoCache[id];
		}

		public Sprite GetPCSprite()
		{
			if (_actorInfo.IsPlayer)
			{
				return _actorInfo.portrait;
			}
			else if (_conversantInfo.IsPlayer)
			{
				return _conversantInfo.portrait;
			}
			else
			{
				return null;
			}
		}

		public int GetConversationID(ConversationState state)
		{
			return (state != null && state.Subtitle != null && state.Subtitle.DialogueEntry != null)
				? state.Subtitle.DialogueEntry.ConversationID
				: -1;
		}
	}
}