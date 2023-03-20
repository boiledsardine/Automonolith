using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChange : MonoBehaviour, IActivate{
    private Renderer render;

    [SerializeField] Material newColor;

    void Start(){
        render = GetComponent<Renderer>();
        render.enabled = true;
    }

    public void activate(){
        render.sharedMaterial = newColor;
    }

    public void deactivate(){

    }
}
