using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SetHeight : MonoBehaviour{
    [SerializeField] private GameObject obj;
    RectTransform objRect;
    RectTransform rt;
    public SizeMode sizeMode;

    // Start is called before the first frame update
    void Start(){
        objRect = obj.GetComponent<RectTransform>();
        rt = GetComponent<RectTransform>();
    }

    void LateUpdate(){
        if(sizeMode == SizeMode.HeightOnly){
            Vector2 newSize = new Vector2(rt.sizeDelta.x, objRect.sizeDelta.y);
            rt.sizeDelta = newSize;
        }
        if(sizeMode == SizeMode.HeightAndWidth){
            Vector2 newSize = new Vector2(objRect.sizeDelta.x, objRect.sizeDelta.y);
            rt.sizeDelta = newSize;
        }
    }
}

public enum SizeMode{
    HeightOnly,
    WidthOnly,
    HeightAndWidth
}