using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallButton : ButtonBase, IActivate, IActivator{
    public void activate(){
        if(latch){
            latchActive = true;
            GetComponent<MeshRenderer>().material = latchColor;
        }
        if(isActivated && !latch){
            deactivate();
        } else {
            isActivated = true;
            boundObject.GetComponent<IActivate>().activate();
        }
    }

    public void deactivate(){
        isActivated = false;
        boundObject.GetComponent<IActivate>().deactivate();
    }

    public bool IsActive(){
        return isActivated;
    }
}