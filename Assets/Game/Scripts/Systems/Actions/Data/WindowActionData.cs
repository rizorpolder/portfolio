using System;
using Game.Scripts.Systems.Actions.Interfaces;

namespace Game.Scripts.Systems.Actions.Data
{
	[Serializable]
	public class WindowActionData : IActionData
	{
		public string WindowName;
		public string InitializeToken;
	}
}