using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubePickup : ObjectBase{
    public CubeColor cubeColor;
    private new void Start(){
        base.Start();
        isMovable = true;
    }

    public string color{
        get{
            if(cubeColor == CubeColor.Red){
                return "red";
            } else if(cubeColor == CubeColor.Green) {
                return "green";
            } else {
                return "blue";
            }
        }
    }

    public enum CubeColor{
        Blue,
        Red,
        Green
    }
}
