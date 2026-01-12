using Game.Scripts.Systems.QuestSystem;
using Game.Scripts.Systems.QuestSystem.Data;
using Game.Scripts.Systems.Texts;
using TMPro;
using UnityEngine;
using Zenject;

namespace Game.Scripts.UI
{
	public class QuestPanelEntry : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI _questName;
		[SerializeField] private TextMeshProUGUI _questDescription;

		[Inject] private TextsValidationSystem _textsValidationSystem;
		[Inject] private IQuestCommand _questCommand;

		public void Initialize(QuestData questData)
		{
			var questProgress = "";
			if (questData.QuestEntries.Count > 0)
			{
				_questCommand.GetQuestProgress(questData.QuestID, out int currentProgress, out int totalProgress);
				questProgress = currentProgress + "/" + totalProgress;
			}

			_questName.text = _textsValidationSystem.GetLocalizedString(questData.QuestNameToken);
			_questDescription.text =
				_textsValidationSystem.GetLocalizedString(questData.QuestGoalToken, "value", questProgress);
		}
	}
}