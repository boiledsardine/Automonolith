using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LatchCounter : MonoBehaviour{
    public static LatchCounter Instance { get; private set; }
    public int latch = 0;

    void Awake(){
        if(Instance == null){
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }
}
