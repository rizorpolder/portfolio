using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins;
using DG.Tweening.Plugins.Core.PathCore;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace Game.Scripts.Extensions
{
	public static class DoTweenExtension
	{
		public static TweenerCore<Vector3, Path, PathOptions> DOAnchoredPath(this RectTransform target,
			Vector3[] path,
			float duration,
			PathType pathType = PathType.Linear,
			PathMode pathMode = PathMode.Full3D,
			int resolution = 10,
			Color? gizmoColor = null)
		{
			if (resolution < 1)
			{
				resolution = 1;
			}

			TweenerCore<Vector3, Path, PathOptions> tweenerCore = DOTween.To(PathPlugin.Get(),
				() => target.anchoredPosition,
				delegate(Vector3 x) { target.anchoredPosition = x; },
				new Path(pathType, path, resolution, gizmoColor),
				duration).SetTarget(target);
			tweenerCore.plugOptions.mode = pathMode;
			return tweenerCore;
		}
	}
}