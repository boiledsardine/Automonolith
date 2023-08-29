using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MethodSet : MonoBehaviour{
    // Start is called before the first frame update
    void Start(){
        var text = GetComponent<TMPro.TMP_Text>();
        text.text = gameObject.transform.name;
    }

    void Update(){
        var border = transform.GetChild(0);
        var thisRt = GetComponent<RectTransform>().sizeDelta;
        var rt = border.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(thisRt.x, thisRt.y);
    }
}
