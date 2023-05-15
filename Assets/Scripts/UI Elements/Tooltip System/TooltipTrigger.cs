using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    public string header;
    [TextArea(3,10)] public string content;
    public Image borderImage;
    public void OnPointerEnter(PointerEventData eventData) {
        TooltipManager.Instance.ShowTooltip(content, header);
        borderImage.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData) {
        TooltipManager.Instance.HideTooltip();
        borderImage.gameObject.SetActive(false);
    }
}
