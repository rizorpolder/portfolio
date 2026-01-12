using System;
using System.Collections.Generic;
using CooldownSystem;
using Game.Scripts.Systems.SaveSystem;
using Newtonsoft.Json;
using UnityEngine;

namespace Game.Scripts.Systems.PlayerController.Data
{
	[Serializable]
	public class PlayerData
	{
		public string Name;
		public string ID;
		public int Points;
		public bool IsGameFinished;
		public string LastTutorialID;
		public FloorNumber CurrentFloor = FloorNumber.FirstFloor; 

		public int SelectedIndex;

		public List<CharacterPositionData> Positions;

		public List<TimerData> Timers = new List<TimerData>();
		
		public static PlayerData Default => new PlayerData()
		{
			Name = "Player",
			Points = 0,
			ID = "",
			SelectedIndex = 0,
			IsGameFinished = false,
			Positions = new List<CharacterPositionData>(),
			LastTutorialID = string.Empty,
			Timers = new List<TimerData>(),
			CurrentFloor = FloorNumber.FirstFloor
		};
	}

	[Serializable]
	public class CharacterPositionData
	{
		public string CharacterID;

		[JsonConverter(typeof(Vector3Converter))]
		public Vector3 Position;

		public CharacterPositionData(string characterID, Vector3 position)
		{
			CharacterID = characterID;
			Position = position;
		}
	}

	public enum FloorNumber
	{
		FirstFloor,
		SecondFloor,
	}
}