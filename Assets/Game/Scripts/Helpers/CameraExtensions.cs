using Game.Scripts.Extensions;
using UnityEngine;

namespace Game.Scripts.Helpers
{
	public static class CameraExtensions
	{
		public static Bounds GetOrthographicBounds(this Camera camera)
		{
			var heightExtent = camera.orthographicSize;
			var widthExtent = heightExtent * Screen.width / Screen.height;

			return new Bounds(camera.transform.position.WithZ(0),
				new Vector3(widthExtent * 2f, heightExtent * 2f, 0));
		}
	}
}