using Game.Scripts.Systems.Actions;
using Game.Scripts.Systems.DialogueSystem.Data;

namespace Game.Scripts.Systems.DialogueSystem
{
	public class ConversationState
	{
		public Subtitle Subtitle;

		public Response[] PcResponses;
		public Response[] NpcResponses;

		public CustomAction[] Events;

		public bool HasPcResponse => PcResponses is {Length: > 0};
		public bool HasNpcResponse => NpcResponses is {Length: > 0};

		public bool HasEvent => Events is {Length: > 0};

		public Response FirstPcResponse => HasPcResponse ? PcResponses[0] : null;
		public Response FirstNpcResponse => HasNpcResponse ? NpcResponses[0] : null;

		public ConversationState(Subtitle subtitle, Response[] pcResponses, Response[] npcResponses, CustomAction[] events)
		{
			Subtitle = subtitle;
			PcResponses = pcResponses;
			NpcResponses = npcResponses;
			Events = events;
		}
	}
}