using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : ObjectBase{
    private ObjectEnvironment envirScript;

    private new void Start(){
        base.Start();
        envirScript = GetComponent<ObjectEnvironment>();
        isMovable = true;
    }

    private void Update(){
        if(isHeld && envirScript.tileUnder != null){
            envirScript.tileUnder.isOccupied = false;
        } else if(!isHeld && envirScript.tileUnder != null) {
            envirScript.tileUnder.isOccupied = true;
        }
    }
}