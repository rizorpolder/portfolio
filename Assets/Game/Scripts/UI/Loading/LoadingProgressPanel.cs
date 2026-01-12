using System.Collections;
using Game.Scripts.UI.Common.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI.Loading
{
	public class LoadingProgressPanel : MonoBehaviour
	{
		[Header("Loading progress panel:")]
		[SerializeField] private Image progressImageView;

		[SerializeField] private UIFiller progressFiller;

		[SerializeField] private TextMeshProUGUI _textAsset;
		[SerializeField] private TextMeshProUGUI _appVersionText;
		private float _progress = 0f;

		private void OnEnable()
		{
			SetProgress(0f);
			_appVersionText.text = Application.version;
		}

		// private void OnLoadingTextAssetReady(AsyncOperationHandle<GameObject> obj)
		// {
		//     _textInstance = obj.Result.GetComponent<LoadingProgressText>();
		//     SetProgress("", _progress);
		// }

		public void SetProgress(float value)
		{
			_progress = value;

			if (progressImageView)
			{
				progressFiller.SetFillerValue(value);
			}

			if (!_textAsset)
				return;


			var percent = (int) (value * 100);
			_textAsset.text = percent.ToString() + "%";
		}
	}
}