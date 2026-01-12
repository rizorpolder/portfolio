using System;
using Game.Scripts.Systems.Actions.Interfaces;
using UnityEngine;

namespace Game.Scripts.Systems.DialogueSystem.Data
{
	[Serializable]
	public class CustomAction
	{
		public ActionType ActionType;

		[SerializeReference]
		public IActionData ActionData;
	}
}