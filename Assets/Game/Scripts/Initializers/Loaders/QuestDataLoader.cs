using Game.Scripts.Data;
using Game.Scripts.Systems.SaveSystem;

namespace Game.Scripts.Systems.Initialize.Loaders
{
	public class QuestDataLoader : ALocalDataLoader<QuestSaveData>
	{
		public override QuestSaveData DefaultData => QuestSaveData.Default;

		protected override SaveDataType _loadDataType => SaveDataType.QuestData;

		public QuestDataLoader(IDataCache<QuestSaveData> cache) : base(cache)
		{
		}
	}
}