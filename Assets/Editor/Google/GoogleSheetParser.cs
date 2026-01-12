using System;
using System.Globalization;
using Game.Scripts.Systems.Google;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace Editor.Google
{
	public abstract class GoogleSheetParser<TConfig> : IGoogleSheetParser where TConfig : AGoogleSheetRemoteConfig
	{
		private Action<bool> _onSuccess = null;
		protected readonly TConfig _config;
		private bool _isFirstEntry = false;

		protected GoogleSheetParser(TConfig config)
		{
			_config = config;
		}

		public void Parse(Action<bool> onSuccess)
		{
			_isFirstEntry = false;
			_onSuccess = onSuccess;
			var ls = _config.GoogleSheetPublicURLs();
			for (var index = 0; index < ls.Count; index++)
			{
				var url = ls[index];
				var req = UnityWebRequest.Get(url);
				var op = req.SendWebRequest();
				op.completed += _ =>
				{
					try
					{
						ImportEntries(req.downloadHandler.text);
						req.Dispose();
						_onSuccess?.Invoke(op.isDone);
					}
					catch (Exception ex)
					{
						Debug.LogError(ex.Message);
						_onSuccess?.Invoke(false);
					}
				};
			}
		}

		private void ImportEntries(string text)
		{
			var i = 0;

			if (!_isFirstEntry)
			{
				_config.Clear();
				_isFirstEntry = true;
			}

			string parsingString = "none";

			try
			{
				foreach (var myString in text.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries))
				{
					if (++i <= IgnoreRowCount())
						continue;

					parsingString = myString;

					var finalString = myString;
					if (finalString.EndsWith('\r'))
					{
						finalString = finalString.Substring(0, myString.Length - 1);
					}

					var data = finalString.Split('\t');
					ImportEntry(data);
				}

				EditorUtility.SetDirty(_config);
			}
			catch (Exception e)
			{
				Debug.LogError($"Failed to import entry {i}\n{parsingString}\n{e.Message}");
				throw;
			}
		}

		protected virtual int IgnoreRowCount()
		{
			return 1;
		}

		protected abstract void ImportEntry(string[] data);

		protected int ToInt(string s)
		{
			try
			{
				var data = s.Trim();
				return data == string.Empty ? 0 : int.Parse(data);
			}
			catch (Exception ex)
			{
				Debug.LogError($"Failed to parse int from {s}\n{ex.Message}");
				throw;
			}
		}

		protected float ToFloat(string s)
		{
			try
			{
				s = s.Replace(",", ".").Replace("\u00A0", "");
				return s == string.Empty ? 0 : float.Parse(s, CultureInfo.GetCultureInfo("en-US"));
			}
			catch (Exception ex)
			{
				Debug.LogError($"Failed to parse float from {s}\n{ex.Message}");
				return 0;
			}
		}

		protected bool ToBoolean(string s)
		{
			if (s == string.Empty)
				return false;

			if (bool.TryParse(s, out bool result))
				return result;

			if (s == "ЛОЖЬ" || s == "0")
				return false;
			else if (s == "ИСТИНА" || s == "1")
				return true;
			else
				return false;
		}

		protected string ClearWhiteSpaces(string val)
		{
			return val.Replace(" ", "");
		}
	}
}