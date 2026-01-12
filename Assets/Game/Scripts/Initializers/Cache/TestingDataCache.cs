using Game.Scripts.Data.SaveData;

namespace Game.Scripts.Systems.Initialize.Cache
{
	public class TestingDataCache : IDataCache<TestingSaveData>
	{
		private TestingSaveData _data;

		void IDataCache<TestingSaveData>.SetData(TestingSaveData data) => _data = data;
		public TestingSaveData GetData() => _data ?? TestingSaveData.Default;
	}
}