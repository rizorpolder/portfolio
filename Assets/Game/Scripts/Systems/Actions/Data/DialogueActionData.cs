using System;
using Game.Scripts.Systems.Actions.Interfaces;
using UnityEngine;

namespace Game.Scripts.Systems.Actions.Data
{
	[Serializable]
	public class DialogueActionData  : IActionData
	{
		[SerializeField] public string DialogueID;
	}
}