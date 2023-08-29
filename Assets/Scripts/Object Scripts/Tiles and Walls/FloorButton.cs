using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorButton : ButtonBase, IActivator{
    private bool _isActivated = false;
    private Material currentMaterial;
    public bool isTrap = false;
    public bool latch = false;
    bool latchActive = false;
    public Material latchColor;

    public bool isActivated{
        get { return _isActivated; }
        set { _isActivated = value; }
    }

    private void OnTriggerEnter(Collider col) {
        if(col.gameObject.tag == "Big Object" || col.gameObject.tag == "Player"){
            if(!isTrap){
                if(latch){
                    latchActive = true;
                    GetComponent<MeshRenderer>().material = latchColor;
                }
                isActivated = true;
                transform.position = originPos - new Vector3(0f, 3f, 0f);
                boundObject.GetComponent<IActivate>().activate();
            } else {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider col) {
        if(col.gameObject.tag == "Big Object" || col.gameObject.tag == "Player" && !latchActive){
            isActivated = false;
            transform.position = originPos;
            boundObject.GetComponent<IActivate>().deactivate();
        }
    }

    public bool IsActive(){
        return isActivated;
    }
}
