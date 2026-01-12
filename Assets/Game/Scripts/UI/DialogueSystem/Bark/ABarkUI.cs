using Game.Scripts.Systems.DialogueSystem.Data;
using UnityEngine;

namespace Game.Scripts.UI.DialogueSystem
{
	public abstract class ABarkUI : MonoBehaviour

	{
		public abstract bool IsPlaying { get; }

		public abstract void Bark(Subtitle subtitle);

		public abstract void Hide();
	}
}