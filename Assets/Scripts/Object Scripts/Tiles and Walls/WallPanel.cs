using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallPanel : MonoBehaviour{
    [SerializeField] string _storedText;
    [SerializeField] string password;
    [SerializeField] GameObject boundObject;

    public string storedText{
        get { return _storedText; }
        set { _storedText = value; }
    }

    public void checkPassword(){
        if(!string.IsNullOrWhiteSpace(password)){
            if(storedText == password){
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