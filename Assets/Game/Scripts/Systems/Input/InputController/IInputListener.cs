using System;
using Game.Scripts.Systems.Input.Data;

namespace Game.Scripts.Systems.Input
{
	public interface IInputListener
	{
		public event Action<TouchData> TouchPositionChanged;
		public event Action<TouchData> TouchChanged;
		public event Action<TouchData> ScrollDataChanged;
	}
}