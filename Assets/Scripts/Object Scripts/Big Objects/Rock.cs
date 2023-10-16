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
}