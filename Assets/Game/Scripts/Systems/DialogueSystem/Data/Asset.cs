using System;

namespace Game.Scripts.Systems.DialogueSystem.Data
{
	[Serializable]
	public class Asset
	{
		private int _id;
		private string _name;

		public int ID => _id;
		public string Name => _name;

		public Asset(Asset sourceAsset)
		{
			_id = sourceAsset._id;
			_name = sourceAsset._name;
		}

		public Asset(int id, string name)
		{
			_id = id;
			_name = name;
		}
	}
}