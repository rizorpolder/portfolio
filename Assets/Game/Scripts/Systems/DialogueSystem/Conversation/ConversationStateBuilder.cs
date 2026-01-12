using System.Collections.Generic;
using Game.Scripts.Systems.DialogueSystem.Data;

namespace Game.Scripts.Systems.DialogueSystem
{
	public class ConversationStateBuilder
	{
		private Subtitle subtitle;
		private readonly List<Response> pcResponses = new();
		private readonly List<Response> npcResponses = new();
		private readonly List<CustomAction> actions = new();

		public ConversationStateBuilder WithSubtitle(Subtitle subtitle)
		{
			this.subtitle = subtitle;
			return this;
		}

		public ConversationStateBuilder AddPcResponse(Response response)
		{
			pcResponses.Add(response);
			return this;
		}

		public ConversationStateBuilder AddPcResponses(IEnumerable<Response> responses)
		{
			pcResponses.AddRange(responses);
			return this;
		}

		public ConversationStateBuilder AddNpcResponse(Response response)
		{
			npcResponses.Add(response);
			return this;
		}

		public ConversationStateBuilder AddNpcResponses(IEnumerable<Response> responses)
		{
			npcResponses.AddRange(responses);
			return this;
		}

		public ConversationStateBuilder AddEvent(CustomAction action)
		{
			actions.Add(action);
			return this;
		}

		public ConversationStateBuilder AddEvents(IEnumerable<CustomAction> actions)
		{
			this.actions.AddRange(actions);
			return this;
		}

		public static implicit operator ConversationState(ConversationStateBuilder builder)
		{
			return new ConversationState(
				builder.subtitle,
				builder.pcResponses.ToArray(),
				builder.npcResponses.ToArray(),
				builder.actions.ToArray()
			);
		}
	}
}