using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MethodSet : MonoBehaviour{
    // Start is called before the first frame update
    
    void Start(){
        var text = GetComponent<TMPro.TMP_Text>();
        text.text = gameObject.transform.name;
    }
}
