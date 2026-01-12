using UnityEngine;

public class AudioFocusObserver : MonoBehaviour
{
	private void OnApplicationFocus(bool hasFocus)
	{
#if UNITY_WEBGL

		PauseAudio(!hasFocus);
#endif
	}

	private void PauseAudio(bool value)
	{
		AudioListener.pause = value;
		AudioListener.volume = value ? 0 : 1;
	}
}