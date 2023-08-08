using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flags : MonoBehaviour{
    public static Flags Instance { get; private set; }
    void Awake(){
        if(Instance == null){
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    public bool isExpectingBrace = false;
    //public bool isExpectingSemicolon = true;
    
    //array flags and persistents
    public bool isArray = false;
    public bool isInArray = false;
    public ValueType arrType = ValueType.none;
    public List<string> arrayElements = new List<string>();
}