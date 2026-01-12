using System.Collections.Generic;
using Game.Scripts.Systems.Input.Data;

namespace Game.Scripts.Systems.Input
{
	public interface IInputData
	{
		public bool IsSingleTouch { get; }
		public List<TouchData> GetTouches();
	}
}