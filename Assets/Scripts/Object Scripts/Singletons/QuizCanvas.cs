using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizCanvas : MonoBehaviour{
    public static QuizCanvas Instance { get; private set; }
    
    void Awake(){
        if(Instance == null){
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }
}
