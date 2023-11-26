using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SliderMouseCheck : MonoBehaviour, IBeginDragHandler, IEndDragHandler{
    public bool isDragging = false;

    public void OnBeginDrag(PointerEventData eventData){
        isDragging = true;
    }

    public void OnEndDrag(PointerEventData eventData){
        isDragging = false;
    }
}
