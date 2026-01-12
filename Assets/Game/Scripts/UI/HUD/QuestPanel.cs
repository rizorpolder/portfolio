using Game.Scripts.Common.Pool;
using Game.Scripts.Systems.QuestSystem;
using UnityEngine;
using Zenject;

namespace Game.Scripts.UI
{
	public class QuestPanel : HUDPanel
	{
		[SerializeField] private QuestPanelEntry _prefab;
		[SerializeField] private Transform _root;

		[Inject] private IQuestListener _questListener;
		[Inject] private IQuestCommand _questCommand;
		[Inject] private ObjectsPoolFactory _factory;

		private ObjectsPool<QuestPanelEntry> _pool;

		private void Start()
		{
			_questListener.OnQuestStateChanged += OnQuestStateChangedHandler;
			_questListener.OnQuestEntryStateChanged += OnQuestStateChangedHandler;
			_pool = _factory.CreatePool(_prefab, _root);
			Initialize();
		}
		private void OnQuestStateChangedHandler(string questID)
		{
			UpdateView();
		}

		private void Initialize()
		{
			UpdateView();
		}

		private void UpdateView()
		{
			_pool.ResetPool();

			var quests = _questCommand.GetActiveQuests();

			if (quests.Count == 0)
			{
				var item = _pool.GetItem();
				var data = _questCommand.GetQuestData($"q1_1");
				
				item.Initialize(data);
				return;
			}
			
			foreach (var questEntity in quests)
			{
				var item = _pool.GetItem();
				var data = _questCommand.GetQuestData(questEntity.Name);
				
				item.Initialize(data);
			}
		}

		private void OnDestroy()
		{
			_questListener.OnQuestStateChanged -= OnQuestStateChangedHandler;
			_questListener.OnQuestEntryStateChanged -= OnQuestStateChangedHandler;
		}
	}
}