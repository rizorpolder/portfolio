using System;

namespace Editor.Google
{
	public interface IGoogleSheetParser
	{
		void Parse(Action<bool> onSuccess);
	}
}