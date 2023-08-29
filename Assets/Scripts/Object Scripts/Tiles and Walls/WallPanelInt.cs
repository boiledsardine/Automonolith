using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallPanelInt : MonoBehaviour{
    [SerializeField] int _storedInt;
    [SerializeField] int password;
    [SerializeField] bool passwordSet;
    [SerializeField] GameObject boundObject;
    public PanelType panelType;

    public int storedInt{
        get { return _storedInt; }
        set { _storedInt = value; }
    }

    public void checkPassword(){
        if(passwordSet){
            if(storedInt == password){
                if(boundObject != null){
                    boundObject.GetComponent<IActivate>().activate();
                }
                Debug.LogWarning("Password correct!");
            } else {
                Debug.LogWarning("Password incorrect!");
            }
        }
    }
}