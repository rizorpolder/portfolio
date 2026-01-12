using System;
using Game.Scripts.Systems.DialogueSystem.Data;

namespace Game.Scripts.Systems.DialogueSystem.Events
{
	public class SelectedResponseEventArgs : EventArgs
	{
		public Response response;

		public DialogueEntry DestinationEntry
		{
			get { return response == null ? null : response.DestinationEntry; }
		}

		public SelectedResponseEventArgs(Response response)
		{
			this.response = response;
		}
	}
}