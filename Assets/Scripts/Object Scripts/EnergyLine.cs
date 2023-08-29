using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyLine : MonoBehaviour{
    public GameObject activator;
    public Material lineOnColor;
    public Material lineOffColor;
    private MeshRenderer mr;

    void Awake(){
        mr = gameObject.GetComponent<MeshRenderer>();
    }
    
    void Update(){
        if(activator != null && activator.GetComponent<IActivator>().IsActive()){
            mr.material = lineOnColor;
        } else {
            mr.material = lineOffColor;
        }
    }
}
