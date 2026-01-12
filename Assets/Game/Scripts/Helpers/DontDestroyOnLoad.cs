using UnityEngine;

namespace Game.Scripts.Helpers
{
	public class DontDestroyOnLoad : MonoBehaviour
	{
		private void Start()
		{
			DontDestroyOnLoad(this);
		}
	}
}