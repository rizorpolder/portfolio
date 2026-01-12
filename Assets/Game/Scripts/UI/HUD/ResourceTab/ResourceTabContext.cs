using System;

namespace Game.Scripts.UI.ResourceTab
{
	[Flags]
	public enum ResourceTabContext
	{
		None = 0,
		Meta = 1 << 0,
		Core = 1 << 1,
		All = Meta | Core,
	}
}