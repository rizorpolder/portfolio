using UnityEngine;

namespace Game.Scripts.Initializers
{
	public class StartDataLoader : MonoBehaviour
	{
		public static StartDataLoader Instance;

		private void Awake()
		{
			if (Instance)
			{
				Debug.LogError($"{name} duplication");
				return;
			}

			Instance = this;
		}
	}
}