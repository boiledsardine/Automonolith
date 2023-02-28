using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Objects{

public class PlayerCharacter : MonoBehaviour{
    public static PlayerCharacter Instance { get; private set; }
    
    private Vector3 originPos;
    private Quaternion originRot;
    
    void Awake(){
        if(Instance == null){
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
        
        originPos = gameObject.transform.position;
        originRot = gameObject.transform.rotation;
    }

    public Vector3 getOriginPos(){
        return originPos;
    }

    public Quaternion getOriginRot(){
        return originRot;
    }

    public Vector3 getCurrentPos(){
        return transform.position;
    }

    public void setCurrentPos(Vector3 pos){
        transform.position = pos;
    }

    public void setCurrentRot(Quaternion rot){
        transform.rotation = rot;
    }
}

}