using System;
using System.Collections.Generic;
using Game.Scripts.Systems.Actions.Data;
using Game.Scripts.Systems.Actions.Interfaces;
using Zenject;

namespace Game.Scripts.Systems.Actions
{
	public class CustomActionsFactory
	{
		private readonly DiContainer _container;
		private readonly Dictionary<Type, Type> _actionMap = new();

		public CustomActionsFactory(DiContainer container)
		{
			_container = container;
			
			Register<QuestActionData, QuestAction>();
			Register<QuestEntryData, QuestEntryAction>();
			Register<QuestRewardData, QuestRewardAction>();
			Register<DialogueActionData, DialogueAction>();
			Register<WindowActionData, WindowAction>();
		}

		private void Register<TData, TAction>()
			where TData : IActionData
			where TAction : BaseAction<TData>
		{
			_actionMap[typeof(TData)] = typeof(TAction);
		}

		public IActionExecutable Create(IActionData data)
		{
			var dataType = data.GetType();

			if (!_actionMap.TryGetValue(dataType, out var actionType))
				throw new InvalidOperationException($"No action registered for {dataType}");

			var action = (IActionExecutable) _container.Instantiate(actionType);
			action.Initialize(data);
			return action;
		}
	}
}