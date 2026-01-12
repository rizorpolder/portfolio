using System;
using Newtonsoft.Json;
using UnityEngine;

namespace Game.Scripts.Systems.SaveSystem
{
	public class Vector3Converter : JsonConverter<Vector3>
	{
		public override Vector3 ReadJson(JsonReader reader,
			Type objectType,
			Vector3 existingValue,
			bool hasExistingValue,
			JsonSerializer serializer)
		{
			var data = serializer.Deserialize<Vector3Data>(reader);
			return new Vector3(data.x, data.y, data.z);
		}

		public override void WriteJson(JsonWriter writer, Vector3 value, JsonSerializer serializer)
		{
			var data = new Vector3Data {x = value.x, y = value.y, z = value.z};
			serializer.Serialize(writer, data);
		}
	}

	[System.Serializable]
	public class Vector3Data
	{
		public float x;
		public float y;
		public float z;
	}
}