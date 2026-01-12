using Game.Scripts.Data;

namespace Game.Scripts.Systems.Initialize.Cache
{
	public class QuestDataCache : IDataCache<QuestSaveData>
	{
		private QuestSaveData _data;

		void IDataCache<QuestSaveData>.SetData(QuestSaveData data) => _data = data;
		public QuestSaveData GetData() => _data ?? QuestSaveData.Default;
	}
}