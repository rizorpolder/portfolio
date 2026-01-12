using Game.Scripts.Data;

namespace Game.Scripts.Systems.QuestSystem.Data
{
	public class QuestGroupEntity
	{
		public readonly string GroupID;
		private Resource _resource;
		public Resource Resources => _resource;

		public QuestGroupEntity(string groupID)
		{
			GroupID = groupID;
		}

		public void ReplaceResources(Resource resource)
		{
			_resource = resource;
		}
	}
}