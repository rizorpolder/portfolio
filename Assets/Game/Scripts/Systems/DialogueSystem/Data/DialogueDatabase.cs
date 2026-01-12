using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Game.Scripts.Systems.DialogueSystem.Data
{
	public class DialogueDatabase
	{
		private List<Actor> _actors;
		private List<Conversation> _conversations;

		private Dictionary<string, Actor> actorNameCache = null;
		private Dictionary<string, Conversation> conversationTitleCache = null;

		public DialogueDatabase()
		{
			_actors = new List<Actor>();
			_conversations = new List<Conversation>();
		}

		public DialogueDatabase(DialogueDatabase source)
		{
			_actors = source._actors;
			_conversations = source._conversations;
		}

		public void Initialize()
		{
			SetupCaches();
		}

		#region Caching

		private void SetupCaches()
		{
			if (actorNameCache == null) actorNameCache = CreateCache<Actor>(_actors);
			if (conversationTitleCache == null) conversationTitleCache = CreateCache<Conversation>(_conversations);
		}

		private Dictionary<string, T> CreateCache<T>(List<T> assets) where T : Asset
		{
			var useTitle = typeof(T) == typeof(Conversation);
			var cache = new Dictionary<string, T>();
			if (Application
			    .isPlaying) // Only build cache at runtime so Dialogue Editor doesn't have to worry about updating it.
			{
				for (int i = 0; i < assets.Count; i++)
				{
					var asset = assets[i];
					var key = useTitle ? (asset as Conversation).Title : asset.Name;
					if (!cache.ContainsKey(key)) cache.Add(key, asset);
				}
			}

			return cache;
		}

		public void ResetCache()
		{
			actorNameCache = null;
			conversationTitleCache = null;
		}

		#endregion

		public void AddActor(Actor actor)
		{
			if (!_actors.Contains(actor))

				_actors.Add(actor);
		}

		private Actor GetActor(string actorName)
		{
			return actorNameCache.TryGetValue(actorName, out var result)
				? result
				: _actors.Find(a => string.Equals(a.Name, actorName));
		}

		public Actor GetActor(int id)
		{
			return _actors.Find(a => a.ID == id);
		}

		public List<Actor> GetActorsList()
		{
			return actorNameCache.Values.ToList();
		}

		public Conversation GetConversation(string conversationTitle)
		{
			return conversationTitleCache.TryGetValue(conversationTitle, out var result)
				? result
				: _conversations.Find(c => string.Equals(c.Title, conversationTitle));
		}

		public Conversation GetConversation(int conversationID)
		{
			return _conversations.Find(c => c.ID == conversationID);
		}

		public void AddConversation(Conversation conversation)
		{
			if (!_conversations.Contains(conversation))
				_conversations.Add(conversation);
		}

		public object GetActorField(string characterName, string propertyName)
		{
			var actor = GetActor(characterName);
			return GetPropertyValue(actor, propertyName);
		}

		private object GetPropertyValue(object obj, string propertyName)
		{
			if (obj == null || string.IsNullOrEmpty(propertyName))
				return null;
			var type = obj.GetType();
			PropertyInfo prop = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
			return prop?.GetValue(obj, null);
		}

		public void ReplacePlayerData(AssetReference reference, Sprite playerPortrait)
		{
			var actor = GetActor(0);
			if (actor == null)
			{
#if UNITY_EDITOR
				Debug.Log("No player data found");

#endif
				return;
			}

			actor.UpdateAssetRefGuid(reference.AssetGUID);
			actor.SetPortraitSprite(playerPortrait);
		}
	}
}