using System;
using System.Collections.Generic;
using Game.Scripts.Systems.Actions.Data;
using Game.Scripts.Systems.DialogueSystem.Data;

//Нужно для корректного отображения в Editor
public static class CustomActionTypeMap
{
	public static readonly Dictionary<ActionType, Type> DataTypes = new()
	{
		//Словарь зависимостей даты от enum
		{ActionType.None, typeof(EmptyActionData)},
		{ActionType.Quest, typeof(QuestActionData)},
		{ActionType.QuestEntry, typeof(QuestEntryData)},
		{ActionType.Dialogue, typeof(DialogueActionData)},
		{ActionType.ShowWindow, typeof(WindowActionData)},
		{ActionType.QuestReward, typeof(QuestRewardData)},
	};
}