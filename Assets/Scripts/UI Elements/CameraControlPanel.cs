using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraControlPanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    public bool pointerNotObstructed = false;
    public void OnPointerEnter(PointerEventData eventData) {
        pointerNotObstructed = true;
    }

    public void OnPointerExit(PointerEventData eventData) {
        pointerNotObstructed = false;
    }
}
