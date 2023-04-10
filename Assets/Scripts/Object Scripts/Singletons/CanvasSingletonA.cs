using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasSingletonA : MonoBehaviour {
    public static CanvasSingletonA Instance { get; private set; }
    
    void Awake(){
        if(Instance == null){
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }
}