namespace Game.Scripts.Systems.DialogueSystem.Data
{
	public class Subtitle
	{
		public CharacterInfo SpeakerInfo;
		public CharacterInfo ListenerInfo;
		public string FormattedText;

		public bool AutoTransition;
		//info came from
		public DialogueEntry DialogueEntry;


		public Subtitle()
		{
			
		}
		
		public Subtitle SetSpeakerInfo(CharacterInfo actorInfo)
		{
			SpeakerInfo = actorInfo;
			return this;
		}
		public Subtitle SetListenerInfo(CharacterInfo listenerInfo)
		{
			ListenerInfo = listenerInfo;
			return this;
		}
		
		public Subtitle SetText(string formattedText)
		{
			FormattedText = formattedText;
			return this;
		}
		
		public Subtitle SetDialogueEntry(DialogueEntry dialogueEntry)
		{
			DialogueEntry = dialogueEntry;
			return this;
		}

		public Subtitle SetAutoTransition(bool autoTransition)
		{
			AutoTransition = autoTransition;
			return this;
		}
		
	}
}