using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler{
    public Image image;
    
    //original parent object
    public Transform ogParent;
    //original location
    public Vector3 ogPosition;    

    void Start(){
        image = GetComponent<Image>();
        transform.position = transform.parent.transform.position;
    }

    public void OnBeginDrag(PointerEventData eventData){
        ogPosition = transform.position;

        ogParent = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();

        image.raycastTarget = false;

        SendMessage("ClearSlot");
    }

    public void OnDrag(PointerEventData eventData){
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData){
        transform.SetParent(ogParent);
        transform.position = ogPosition;

        image.raycastTarget = true;

        SendMessage("CheckSlot");
    }
}