using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

public class TextItem : MonoBehaviour, IPointerDownHandler{
    public string textToShow;
    public float sizeOffset = 4;
    TMPro.TMP_Text childText;
    public int indexID;
    public delegate void ClickOnLinkEvent(string keyword);
    public static event ClickOnLinkEvent OnClickedOnLinkEvent;

    void Start(){
        childText = transform.GetChild(0).GetComponent<TMPro.TMP_Text>();
        childText.text = textToShow;

        var thisrt = GetComponent<RectTransform>();
        thisrt.sizeDelta = new Vector2(childText.preferredWidth + sizeOffset + 2, childText.preferredHeight + sizeOffset);
    }

    //ClearSlot and CheckSlot called by DraggableItem.cs with SendMessage on drag begin and end
    public void ClearSlot(){
        var ogParent = transform.GetComponent<DraggableItem>().ogParent;
        DragSlot slot = ogParent.GetComponent<DragSlot>();
        slot.isCorrect = false;
    }

    public void CheckSlot(){
        DragSlot slot = transform.parent.GetComponent<DragSlot>();
        if(slot.correctAnswers.Contains(textToShow)){
            slot.isCorrect = true;
        } else {
            slot.isCorrect = false;
        }
    }

    public void OnPointerDown(PointerEventData eventData){
        TMPro.TMP_Text dictText = GameObject.Find("DictTextBody").GetComponent<TMPro.TMP_Text>();
        TMPro.TMP_Text dictName = GameObject.Find("ItemNameText").GetComponent<TMPro.TMP_Text>();
        
        //find an entry in the dictionaries
        OnClickedOnLinkEvent?.Invoke(textToShow);
    }
}
