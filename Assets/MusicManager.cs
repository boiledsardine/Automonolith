using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour{
    AudioSource source;
    void Awake(){
        source = GetComponent<AudioSource>();
    }
    // Start is called before the first frame update
    void Start(){
        source.Play();
    }
}
