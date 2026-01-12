using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(TextMeshProUGUI))]
public class OpenHyperlink : MonoBehaviour, IPointerClickHandler
{
	private TextMeshProUGUI m_textMeshPro;

	void Start()
	{
		m_textMeshPro = GetComponent<TextMeshProUGUI>();
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		int linkIndex = TMP_TextUtilities.FindIntersectingLink(m_textMeshPro, Input.mousePosition, Camera.main);
		if (linkIndex != -1)
		{
			TMP_LinkInfo linkInfo = m_textMeshPro.textInfo.linkInfo[linkIndex];
			Application.OpenURL(linkInfo.GetLinkID());
		}
	}
}