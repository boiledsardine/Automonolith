using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flags : MonoBehaviour{
    public static Flags Instance { get; private set; }
    public bool isExpectingArrayCount = false;
    void Awake(){
        if(Instance == null){
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    public int bracesCounted = 0;
    public bool isExpectingBrace = false;
    //public bool isExpectingSemicolon = true;
    
    //array flags and persistents
    public bool isArray = false;
    public bool isInArray = false;
    public int indexOfArrayInitializer = 0;
    public int expectedIndexCount = 0;
    public string arrName = "";
    public bool arrIsUsingNewOrVar = false;
    public ValueType arrType = ValueType.none;
    public List<string> arrayElements = new List<string>();
    public bool hasArrayError = false;

    //read flags
    public bool hasParam = false;

    public void ResetFlags(){
        isArray = false;
        isInArray = false;
        indexOfArrayInitializer = 0;
        expectedIndexCount = 0;
        arrIsUsingNewOrVar = false;
        isExpectingArrayCount = false;
        hasArrayError = false;
    }
}