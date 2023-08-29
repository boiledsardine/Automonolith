using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BindMultiactivator : MonoBehaviour{
    public void Awake(){
        MultiActivate activator = GetComponent<MultiActivate>();
        activator.boundObject = GameObject.Find("TutorialManager2");
    }
}