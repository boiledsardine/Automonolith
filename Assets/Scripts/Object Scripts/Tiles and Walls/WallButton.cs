using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallButton : ButtonBase, IActivate{
    [SerializeField] private bool isActivated = false;
    
    public void activate(){
        if(isActivated){
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
}