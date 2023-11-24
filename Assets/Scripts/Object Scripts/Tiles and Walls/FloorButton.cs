using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorButton : ButtonBase, IActivator{
    private Material currentMaterial;
    public bool isTrap = false;

    private void OnTriggerEnter(Collider col) {
        if(col.gameObject.tag == "Pillar" || col.gameObject.tag == "Big Object" || col.gameObject.tag == "Player"){
            if(!isTrap){
                if(latch){
                    latchActive = true;
                    GetComponent<MeshRenderer>().material = latchColor;
                }
                isActivated = true;
                transform.position = originPos - new Vector3(0f, 3f, 0f);
                Debug.Log("activating");
                boundObject.GetComponent<IActivate>().activate();
            } else {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider col) {
        if(col.gameObject.tag == "Pillar" || col.gameObject.tag == "Big Object" || col.gameObject.tag == "Player" && !latchActive){
            isActivated = false;
            transform.position = originPos;
            Debug.Log("deactivating");
            boundObject.GetComponent<IActivate>().deactivate();
        }
    }
    
    public bool IsActive(){
        return isActivated;
    }
}
