using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {
    public string header;
    [TextArea(3,10)] public string content;
    public Image borderImage;
    public bool isUIElement = false;
    public void OnPointerEnter(PointerEventData eventData) {
        TooltipManager.Instance.ShowTooltip(content, header, isUIElement);
        if(borderImage != null){
            borderImage.gameObject.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        TooltipManager.Instance.HideTooltip();
        if(borderImage != null){
            borderImage.gameObject.SetActive(false);
        }
    }

    public void OnPointerClick(PointerEventData eventData){
        if(isUIElement){
            return;
        }

        var text = GetComponent<TMPro.TMP_Text>().text;
        CodeEditor.Instance.AddLine("Bot." + text + ";");
        CodeEditor.Instance.FocusCaret(true);
    }
}
