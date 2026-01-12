using System;
using Cysharp.Threading.Tasks;
using Game.Scripts.UI.Common;
using Game.Scripts.UI.Common.Animation;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Game.Scripts.UI.Loading
{
	[RequireComponent(typeof(PlayableAnimation))]
	public class LoadingView : BaseView
	{
		[SerializeField] private bool isNeedSound = false;
		[SerializeField] private PlayableAnimation playableView;

		public void LoadScreenAsset(AssetReference asset)
		{
			asset.LoadAssetAsync<Sprite>().Completed += delegate(AsyncOperationHandle<Sprite> operation)
			{
				ImageComponent.sprite = operation.Result;
			};
		}

		public UniTask ShowWithAnimation(Action action = null)
		{
			base.Show();
			gameObject.SetActive(true);
			// if (isNeedSound)
			// 	ManagerAudio.SharedInstance.PlayAudioClip(TAudio.loader_down.ToString());

			var observable = playableView.Show(() => { action?.Invoke(); });

			return observable;
		}

		public UniTask HideWithAnimation(Action action = null)
		{
			// if (isNeedSound)
			// 	ManagerAudio.SharedInstance.PlayAudioClip(TAudio.loader_up.ToString());

			var observable = playableView.Hide(() =>
			{
				action?.Invoke();
				base.Hide();
				gameObject.SetActive(false);
			});

			return observable;
		}
	}
}