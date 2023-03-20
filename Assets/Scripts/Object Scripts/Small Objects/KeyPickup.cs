using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyPickup : ObjectBase {
    private new void Start(){
        base.Start();
        isMovable = true;
    }
}