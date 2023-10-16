using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BindFloorButton : MonoBehaviour{
    public void Awake(){
        FloorButton floorButton = GetComponent<FloorButton>();
        floorButton.boundObject = GameObject.Find("TutorialManager2");
    }
}