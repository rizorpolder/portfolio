namespace Game.Scripts.Systems.Loading
{
	public static class SceneNames
	{
		public const string Office = "Office";
		public const string Start = "start";
		public const string CreateCharacter = "CreateCharacter";
		public const string TestingRoom = "TestingRoom";
		public const string MentorsRoom = "MentorsRoom";
		public const string MeetingRoom = "MeetingRoom";
		public const string Final = "Final";

		public static bool IsOfficeScene(string sceneName)
		{
			return sceneName == Office;
		}

		public static bool IsMentorsScene(string activeScene)
		{
			return activeScene == MentorsRoom;

		}
	}
}