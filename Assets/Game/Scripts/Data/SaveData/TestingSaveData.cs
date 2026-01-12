using System;
using System.Collections.Generic;

namespace Game.Scripts.Data.SaveData
{
	[Serializable]
	public class TestingSaveData
	{
		public Dictionary<int, TestModuleProgress> SavedModules;

		public static TestingSaveData Default => new TestingSaveData()
		{
			SavedModules = new Dictionary<int, TestModuleProgress>()
		};

		public void SetTestingData(int id, TestModuleProgress progress)
		{
			SavedModules[id] = progress;
		}

		public bool GetTestingData(int id, out TestModuleProgress progress)
		{
			return SavedModules.TryGetValue(id, out progress);
		}

		public Dictionary<int, TestModuleProgress> GetFullTestingData()
		{
			return SavedModules;
		}
	}

	[Serializable]
	public class TestModuleProgress
	{
		public int ModuleId;
		public int BestScore;
		public int BestAccuracy;
		public int TotalAttempts;
		public string LastPlayedDate;
	}
}