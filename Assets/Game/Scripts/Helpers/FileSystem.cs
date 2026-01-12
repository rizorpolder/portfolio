using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using Cysharp.Threading.Tasks;

namespace Game.Scripts.Helpers
{
	public static class FileSystem
	{
#if UNITY_WEBGL
		[DllImport("__Internal")]
		static extern void FileSystemSyncfsAddEvent(Action<int, string> action);

		[DllImport("__Internal")]
		static extern void FileSystemSyncfs(int id);
#else
        private static Action<int, string> _callback;
        private static void FileSystemSyncfsAddEvent(Action<int, string> action) { _callback = action; }
        private static void FileSystemSyncfs(int id) { _callback?.Invoke(id, ""); }
#endif

		private readonly static Dictionary<int, Action<string>> _callbacks = new Dictionary<int, Action<string>>();

		static FileSystem()
		{
#if UNITY_WEBGL && !UNITY_EDITOR
            FileSystemSyncfsAddEvent(Callback);
#endif
		}

		[MonoPInvokeCallback(typeof(Action<int, string>))]
		static void Callback(int id, string error)
		{
			var cb = _callbacks[id];
			_callbacks.Remove(id);
			cb?.Invoke(string.IsNullOrEmpty(error) ? null : error);
		}

		public static void Syncfs(Action<string> callback)
		{
#if UNITY_WEBGL && !UNITY_EDITOR
                var id = _callbacks.Count + 1;
                _callbacks.Add(id, callback);
                FileSystemSyncfs(id);
#else
			//simulate long fs sync in editor
			_ = DelayWithCallback(TimeSpan.FromSeconds(0.5f), () => callback?.Invoke(null));
#endif
		}

		private static async UniTaskVoid DelayWithCallback(TimeSpan delay, Action callback)
		{
			await UniTask.Delay(delay);
			callback?.Invoke();
		}
	}
}