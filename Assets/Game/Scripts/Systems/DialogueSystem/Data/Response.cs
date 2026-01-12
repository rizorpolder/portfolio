namespace Game.Scripts.Systems.DialogueSystem.Data
{
	public class Response
	{
		public DialogueEntry DestinationEntry;
		public string FormattedText;

		public Response(DialogueEntry destinationEntry)
		{
			DestinationEntry = destinationEntry;
		}

		public Response(DialogueEntry destinationEntry, string formattedText) : this(destinationEntry)
		{
			FormattedText = formattedText;
		}
	}
}