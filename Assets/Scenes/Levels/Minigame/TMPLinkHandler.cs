using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;

[RequireComponent(typeof(TMP_Text))]
public class TMPLinkHandler : MonoBehaviour, IPointerClickHandler{
    TMP_Text tmpTextBox;
    Canvas textCanvas;
    [SerializeField] Camera cameraToUse;

    public GameObject dragSlot;
    
    //notifies external script listeners
    public delegate void ClickOnLinkEvent(string keyword);
    public static event ClickOnLinkEvent OnClickedOnLinkEvent;

    void Awake(){
        tmpTextBox = GetComponent<TMP_Text>();
        textCanvas = GetComponentInParent<Canvas>();

        if(textCanvas.renderMode == RenderMode.ScreenSpaceOverlay){
            cameraToUse = null;
        } else {
            cameraToUse = textCanvas.worldCamera;
        }
    }

    public void OnPointerClick(PointerEventData eventData){
        Vector3 mousePosition = new Vector3(eventData.position.x, eventData.position.y, 0);
        var linkTaggedText = TMP_TextUtilities.FindIntersectingLink(tmpTextBox, mousePosition, cameraToUse);
        if(linkTaggedText != -1){
            TMP_LinkInfo linkInfo = tmpTextBox.textInfo.linkInfo[linkTaggedText];
            if(linkInfo.GetLinkID() != "key"){
                return;
            }
            OnClickedOnLinkEvent?.Invoke(linkInfo.GetLinkText());
        }
    }
}
