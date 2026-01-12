using System;
using Game.Scripts.Helpers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI.DialogueSystem
{
	public class DialogueTextPanelEntry : MonoBehaviour
	{
		[SerializeField] private PanelSkinWrapper _activeSkin;
		[SerializeField] private PanelSkinWrapper _disabledSkin;

		[SerializeField] private Image _arrowImage;
		[SerializeField] private Image _backgroundImage;

		[SerializeField] private TextMeshProUGUI _text;
		[SerializeField] private TypewriterEffect _typewriterEffect;

		public bool IsTyping => _typewriterEffect.IsTyping;
		
		public void SetState(bool isActive)
		{
			var wrapper = isActive ? _activeSkin : _disabledSkin;
			_arrowImage.sprite = wrapper.ArrowSprite;
			_backgroundImage.sprite = wrapper.BackgroundSprite;
		}

		public void SetText(string text)
		{
			_typewriterEffect.Initialize(text);
			_typewriterEffect.StartTypewrite();
		}

		public void Release()
		{
			_text.text = "";
		}

		public void SkipTypewriterEffect()
		{
			_typewriterEffect.SkipTyping();
		}

		[Serializable]
		private class PanelSkinWrapper
		{
			public Sprite ArrowSprite;
			public Sprite BackgroundSprite;
		}
	}
}