using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechGate : GateBase, IActivate{
    public void activate(){
        StartCoroutine(openGate());
    }

    public void deactivate(){
        StartCoroutine(closeGate());
    }
}
