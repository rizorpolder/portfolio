using System;

namespace Game.Scripts.Data.SaveData
{
	public class SaveMetaData
	{
		public string Identifier;
		/// <summary>
		/// Last saved device ID
		/// </summary>
		public string DeviceId;

		public DateTimeOffset UpdateDate;
	}
}