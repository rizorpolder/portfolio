using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Scripts.Systems.Input
{
	public class InputModule : StandaloneInputModule
	{
		public PointerEventData GetEventData(int id)
		{
			PointerEventData pointerEventData = GetLastPointerEventData(id);

			return pointerEventData;
		}

		public GameObject GetPointerTarget(int id)
		{
			PointerEventData pointerEventData = GetLastPointerEventData(id);

			GameObject raycastTarget = pointerEventData?.pointerCurrentRaycast.gameObject;

			return raycastTarget;
		}
	}
}