using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallPanel : MonoBehaviour{
    private string _storedLine;
    [SerializeField] string storedText;
    [SerializeField] string password;
    [SerializeField] GameObject boundObject;

    public string storedLine{
        get { return _storedLine; }
        set { _storedLine = value; }
    }

    private void Awake(){
        storedLine = storedText;
    }

    public void checkPassword(){
        if(!string.IsNullOrWhiteSpace(password)){
            if(storedLine == password){
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