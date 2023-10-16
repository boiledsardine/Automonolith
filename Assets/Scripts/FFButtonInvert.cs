using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FFButtonInvert : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler{
    public int timeScale;
    public void OnPointerEnter(PointerEventData eventData){
        if(Time.timeScale != timeScale){
            var arrowImage = transform.GetChild(0).gameObject.GetComponent<Image>();
            arrowImage.color = Color.black;
        }
    }

    public void OnPointerExit(PointerEventData eventData){
        if(Time.timeScale != timeScale){
            var arrowImage = transform.GetChild(0).gameObject.GetComponent<Image>();
            arrowImage.color = Color.white;
        }
    }
}
