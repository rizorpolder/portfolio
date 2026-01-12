using System;
using System.Collections;
using Game.Scripts.Helpers;
using Game.Scripts.Systems.DialogueSystem.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Scripts.UI.DialogueSystem
{
	public class MainBarkUI : ABarkUI
	{
		private Action _hideAction;

		[SerializeField] private float _duration = 4f;
		[SerializeField] private float _maxDuration = 5f;
		[SerializeField] private float arrowMaxDelta;

		[SerializeField] private TextMeshProUGUI _barkText;
		[SerializeField] private Button _closeButton;
		[SerializeField] private Canvas _canvas;
		[SerializeField] private CanvasScaler _canvasScaler;
		[SerializeField] private RectTransform _rectTransform;
		[SerializeField] private RectTransform _arrowRect;
		[SerializeField] private RectTransform _bubble;
		[SerializeField] private RectTransform _text;
		[SerializeField] private Transform _rootObject;

		[SerializeField] private Vector2 _minSizeDots = new Vector2(100, 40);
		[SerializeField] private Vector2 _minSize = new Vector2(150, 80);

		[SerializeField] private Animation _anim;

		[Inject] CoroutineHelper _coroutineHelper;

		private bool _barkOpened = false;
		private bool _isPlaying;

		public override bool IsPlaying => _isPlaying;

		#region Animation Transitions

		[Serializable]
		public class AnimationTransitions
		{
			public string showAnim = "BarkShowAnim";
			public string hideAnim = "BarkHideAnim";
			public float animTime = 1f;
		}

		public AnimationTransitions animationTransitions = new AnimationTransitions();

		#endregion

		private IEnumerator hideCor;
		private IEnumerator showCor;


		private void OnEnable()
		{
			// if (!_canvas.worldCamera)
			// 	_canvas.worldCamera = _inputHandler.Camera;
		}

		private void Start()
		{
			_closeButton.onClick.AddListener(OnCloseByButton);
		}

		public override void Bark(Subtitle subtitle)
		{
			Bark(subtitle.FormattedText);
		}

		public void Bark(string subtitle)
		{
			Bark(subtitle, 0);
		}

		public void Bark(string subtitle, float customDuration)
		{
			Bark(subtitle, customDuration, false, null);
		}

		public void Bark(string subtitle, float customDuration, Action actionEnd = null)
		{
			Bark(subtitle, customDuration, false, actionEnd);
		}

		public void Bark(string subtitle, float customDuration = 0f, bool left = false, Action actionEnd = null)
		{
			if (showCor != null || _barkOpened)
			{
				actionEnd?.Invoke();
				SetText(subtitle);
				return;
			}

			if (hideCor != null)
			{
				StopCoroutine(hideCor);
				hideCor = null;
			}

			// if (rectTransform.IsVisibleFrom(_inputHandler.Camera))
			// 	ManagerAudio.SharedInstance.PlayAudioClip(barkShowAudios.GetRandom());
			gameObject.SetActive(true);

			showCor = ShowCoroutine();
			_coroutineHelper.StartCoroutine(showCor);

			SetSide(left);
			SetText(subtitle);

			if (customDuration == 0f)
			{
				customDuration = _barkText.text.Length <= 55 ? _duration : _maxDuration;
			}

			CancelInvoke("Hide");
			Invoke("Hide", customDuration + animationTransitions.animTime);

			_hideAction = actionEnd;
		}

		private void SetText(string text)
		{
			// трансформации для получения токена
			string title = text;
			// title = LocalizationHelper.GetStringByToken(text); //TODO text tokens to string
			//
			// FormattedText.ReplaceVarTags(ref title);

			if (_barkText != null)
				_barkText.text = title;

			var layout = _bubble.GetComponent<LayoutElement>();
			if (layout != null)
			{
				layout.minWidth = _barkText.text == "..." ? _minSizeDots.x : _minSize.x;
				layout.minHeight = _barkText.text == "..." ? _minSizeDots.y : _minSize.y;
			}
		}

		private void SetSide(bool left)
		{
			var pos = left ? new Vector3(-1, 1, 1) : new Vector3(1, 1, 1);
			_bubble.localScale = pos;
			_text.localScale = pos;
		}

		private void HideWrapper(Transform transform)
		{
			if (!_barkOpened || hideCor != null)
				return;

			InstantHide();
		}

		private void InstantHide()
		{
			if (!_barkOpened)
				return;


			if (hideCor != null)
			{
				StopCoroutine(hideCor);
				hideCor = null;
			}

			if (showCor != null)
			{
				StopCoroutine(showCor);
				showCor = null;
			}

			// if (rectTransform.IsVisibleFrom(_inputHandler.Camera))
			// 	ManagerAudio.SharedInstance.PlayAudioClip(barkHideAudios.GetRandom());
			gameObject.SetActive(false);
			_closeButton.gameObject.SetActive(false);
			_barkOpened = false;
			_hideAction?.Invoke();
			CancelInvoke("Hide");
		}

		public override void Hide()
		{
			if (hideCor != null || !_barkOpened)
			{
				Debug.LogWarning($"Bark Hide error: hideCor  - {hideCor != null}, barkOpened - {_barkOpened}");
				return;
			}

			if (showCor != null)
			{
				StopCoroutine(showCor);
				showCor = null;
			}

			hideCor = HideCoroutine();
			// if (rectTransform.IsVisibleFrom(_inputHandler.Camera))
			// 	ManagerAudio.SharedInstance.PlayAudioClip(barkHideAudios.GetRandom());

			_coroutineHelper.StartCoroutine(hideCor);
		}

		IEnumerator HideCoroutine()
		{
			_anim.Play(animationTransitions.hideAnim);
			_closeButton.gameObject.SetActive(false);
			yield return new WaitForSeconds(animationTransitions.animTime);
			_barkOpened = false;
			_canvas.gameObject.SetActive(false);

			_hideAction?.Invoke();
		}

		IEnumerator ShowCoroutine()
		{
			_barkOpened = true;
			// if (DialogueManager.IsConversationActive)
			// {
			// 	closeButton.gameObject.SetActive(true);
			// }

			_anim.Play(animationTransitions.showAnim);
			yield return new WaitForSeconds(animationTransitions.animTime);
		}

		private void OnCloseByButton()
		{
			Hide();
			CancelInvoke("Hide");

			// if (DialogueManager.IsConversationActive)
			// {
			// 	var conversation = DialogueManager.instance.LastConversationStarted;
			// 	var id = DialogueManager.instance.conversationController.currentState.subtitle.dialogueEntry.id;
			// 	SkipActionAnalyticData.Send("bark", conversation, id, true);
			// }
		}
	}
}