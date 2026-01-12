using AudioManager.Runtime.Core.Manager;

namespace Plugins.AudioManager.Runtime.Core
{
	public static class ManagerAudioExtensions
	{
		public static void PlayAudioClip(this ManagerAudio manager, TAudio audio, float delayExtra = 0)
		{
			manager.PlayAudioClip(audio.ToString(), delayExtra: delayExtra);
		}
	}
}
