using TMPro;
using UnityEngine;

namespace Game.Scripts.Helpers
{
	public class InputValidationText : MonoBehaviour
	{
		[SerializeField] private TMP_InputField _inputField;

		private void Start()
		{
			_inputField.onValidateInput += OnValidateChar;
		}

		private char OnValidateChar(string text, int charIndex, char addedChar)
		{
			if (charIndex >= 20)
				return '\0';

			if (System.Text.RegularExpressions.Regex.IsMatch(addedChar.ToString(), @"^[a-zA-Z0-9а-яА-ЯёЁ]$"))
				return addedChar;

			return '\0';
		}

		private void OnDestroy()
		{
			_inputField.onValidateInput -= OnValidateChar;
		}
	}
}