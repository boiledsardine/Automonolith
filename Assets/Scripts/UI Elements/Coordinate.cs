using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Coordinate : MonoBehaviour{
    void Start(){
        var textMesh = gameObject.GetComponent<TMP_Text>();
        textMesh.text = gameObject.name[0].ToString();
    }

    private void LateUpdate(){
        transform.forward = Camera.main.transform.forward;
    }
}
