using System;
using DG.Tweening;
using Game.Scripts.Systems.DialogueSystem.Data;
using Game.Scripts.UI.Common;
using Game.Scripts.UI.DialogueSystem.Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using CharacterInfo = Game.Scripts.Systems.DialogueSystem.Data.CharacterInfo;

namespace Game.Scripts.UI.DialogueSystem
{
	public class DialogueCharacterPanel : BasePanel, IDialogueContent
	{
		[Serializable]
		private class PanelVisualState
		{
			[SerializeField] private Color color;
			[SerializeField] private Vector3 scale;
			[SerializeField] private float time = 0.2f;

			public void Apply(Image image)
			{
				image.DOKill();
				image.DOColor(color, time).SetEase(Ease.InOutSine).Play();
				image.transform.DOScale(scale, time).SetEase(Ease.InOutSine).Play();
			}
		}

		[SerializeField] private bool _isPlayerPanel;
		[SerializeField] private Image _actorImage;
		[SerializeField] private TextMeshProUGUI _characterName;

		[Header("Listener character visual parameters")]
		[SerializeField] PanelVisualState onActiveState;

		[SerializeField] PanelVisualState onInactiveState;

		private bool _isSelected = false;

		public void SetContent(Subtitle subtitle)
		{
			_isSelected = (subtitle.SpeakerInfo.IsPlayer && _isPlayerPanel) ||
			              (!subtitle.SpeakerInfo.IsPlayer && !_isPlayerPanel);
			
			UpdateCharacter(subtitle);

			if (_isSelected)
			{
				onActiveState.Apply(_actorImage);
			}
			else
			{
				onInactiveState.Apply(_actorImage);
			}
		}

		public void Close()
		{
			gameObject.SetActive(false);
		}

		private void UpdateCharacter(Subtitle subtitle)
		{
			var isPlayerSpeaker = subtitle.SpeakerInfo.IsPlayer;
			CharacterInfo characterInfo = null;

			if (_isPlayerPanel)
			{
				characterInfo = isPlayerSpeaker ? subtitle.SpeakerInfo : subtitle.ListenerInfo;
			}
			else
			{
				characterInfo = isPlayerSpeaker ? subtitle.ListenerInfo : subtitle.SpeakerInfo;
			}

			//если панель игрока и спикер игрок - берем спикеринфо
			//если панель игрока и спикер не игрок - берем листенерИнфо
			//если панель не игрока и спикер не игрок - берем спикер инфо
			_actorImage.sprite = characterInfo.portrait;
			if (!characterInfo.IsPlayer && !_isPlayerPanel)
			{
				_characterName.text = characterInfo.Name; // TODO ConvertFromTextRep
			}
			Show();
		}
	}
}