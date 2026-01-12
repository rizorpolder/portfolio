using System.Collections.Generic;
using Game.Scripts.Systems.Actions.Interfaces;
using Game.Scripts.Systems.Conditions;
using Game.Scripts.Systems.DialogueSystem.Data;
using UnityEngine;

namespace Game.Scripts.Systems.Character.Reaction.Data
{
	public class ReactionData : ScriptableObject
	{
		[Range(0, 100)]
		[SerializeField] private float _chance;

		public float Chance => _chance;

		public Condition conditions = new Condition();

		public CustomAction Action = new CustomAction();

		public void SetQuestConditions(QuestCondition[] questConditions)
		{
			conditions ??= new Condition();
			conditions.QuestConditions = questConditions;
		}
	}
}