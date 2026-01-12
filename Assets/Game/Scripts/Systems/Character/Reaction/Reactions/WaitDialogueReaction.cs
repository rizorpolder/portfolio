using Zenject;

namespace Game.Scripts.Systems.Character.Reaction
{
	public class WaitDialogueReaction : BaseReaction
	{
		private string _speaker;

		[Inject] CharacterFactory _characterFactory;
		
		public WaitDialogueReaction()
		{
			
		}

		public void SetSpeadeker(string speaker)
		{
			_speaker = speaker;
		}
		
		public override void Start(string target)
		{
		}

		public override void Stop()
		{
			base.Stop();

			var _speakerCharacter = _characterFactory.GetCharacter(_speaker);
			;
			if (_speakerCharacter is null)
				return;

			_speakerCharacter.ReplaceReactionState(ReactionState.Terminated);
		}
	}
}