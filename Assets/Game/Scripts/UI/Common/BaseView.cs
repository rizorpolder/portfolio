using UnityEngine;

namespace Game.Scripts.UI.Common
{
	public class BaseView : AbstractUIElement
	{
		[Header("BaseView:")]
		[SerializeField] protected UnityEngine.UI.Image imageComponent;

		public UnityEngine.UI.Image ImageComponent
		{
			get { return imageComponent; }
		}

		[SerializeField] protected TMPro.TextMeshProUGUI titleComponent;

		public TMPro.TextMeshProUGUI TitleComponent
		{
			get { return titleComponent; }
		}

		public void SetViewData(Sprite sprite, string text, bool localize = true)
		{
			ImageComponent.sprite = sprite;
			//TitleComponent.text = localize ? LocalizationHelper.GetStringByToken(text) : text;
		}

		public override void Show(System.Action callback = null)
		{
			OnShowAction();

			if (TitleComponent != null)
				TitleComponent.gameObject.SetActive(true);

			if (ImageComponent != null)
				ImageComponent.gameObject.SetActive(true);
		}

		public override void Hide(System.Action callback = null)
		{
			if (TitleComponent != null)
				TitleComponent.gameObject.SetActive(false);

			if (ImageComponent != null)
				ImageComponent.gameObject.SetActive(false);
		}
	}
}