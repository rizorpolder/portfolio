using System.Collections;
using TMPro;
using UnityEngine;

namespace Game.Scripts.Helpers
{
	public class TypewriterEffect : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI _tmpText;
		[SerializeField] private float delay = 0.05f;
		private string _fullText;
		private Coroutine _typingCoroutine;

		public bool IsTyping => _typingCoroutine != null;

		public void Initialize(string text)
		{
			_fullText = text;
		}

		public void StartTypewrite()
		{
			if (_typingCoroutine != null)
				StopCoroutine(_typingCoroutine);

			_typingCoroutine = StartCoroutine(TypeText());
		}

		private IEnumerator TypeText()
		{
			for (var i = 0; i <= _fullText.Length; i++)
			{
				var current = _fullText.Substring(0, i);
				_tmpText.text = current;
				yield return new WaitForSeconds(delay);
			}

			_typingCoroutine = null;
		}

		public void SkipTyping()
		{
			if (_typingCoroutine != null)
				StopCoroutine(_typingCoroutine);

			_typingCoroutine = null;
			_tmpText.text = _fullText;
		}
	}
}