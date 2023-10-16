using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResizeScrollObject : MonoBehaviour{
    public void Resize(){
        var vlg = GetComponent<VerticalLayoutGroup>();
        float spacing = vlg.spacing;
        
        var child = transform.GetChild(0);
        float width = child.GetComponent<RectTransform>().rect.width;
        float height = child.GetComponent<RectTransform>().rect.height;

        //get index of last active child
        int lastActiveChild = 0;
        for(int i = 0; i < transform.childCount; i++){
            if(transform.GetChild(i).gameObject.activeInHierarchy){
                lastActiveChild = i;
            }
        }
        lastActiveChild++;
        Debug.Log(lastActiveChild);

        float unspacedHeight = height * lastActiveChild;
        float spacesHeight = spacing * (lastActiveChild - 1);
        float totalHeight = unspacedHeight + spacesHeight;

        var rt = GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(width, totalHeight);
    }
}