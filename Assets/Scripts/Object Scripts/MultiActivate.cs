using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiActivate : MonoBehaviour, IActivate{
    public GameObject boundObject;
    public GameObject[] activators;

    public void activate(){
        if(CheckActivators() && boundObject != null){
            boundObject.GetComponent<IActivate>().activate();
        }
    }

    public void deactivate(){
        //check activators
        if(!CheckActivators() && boundObject != null){
            boundObject.GetComponent<IActivate>().deactivate();
        }
    }

    bool CheckActivators(){
        List<bool> activatorBools = new List<bool>();
        foreach(GameObject obj in activators){
            activatorBools.Add(obj.GetComponent<IActivator>().IsActive());
        }
        bool allActive = !activatorBools.Contains(false);
        return allActive;
    }
}
