using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SetHeight : MonoBehaviour{
    [SerializeField] private GameObject obj;
    RectTransform objRect;
    RectTransform rt;


    // Start is called before the first frame update
    void Start(){
        objRect = obj.GetComponent<RectTransform>();
        rt = GetComponent<RectTransform>();
    }

    void LateUpdate(){
        Vector2 newSize = new Vector2(rt.sizeDelta.x, objRect.sizeDelta.y);
        rt.sizeDelta = newSize;
    }
}
