using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommandControl;

public class Floating : MonoBehaviour{
    [SerializeField] private float degPerSec = 30f;
    [SerializeField] private float _amplitude = 10f;
    [SerializeField] private float frequency = 1f;

    private Vector3 _posOffset, tempPos;

    public Vector3 posOffset{
        get { return _posOffset; }
        set { _posOffset = value; }
    }

    public float amplitude{
        get { return _amplitude; }
        set { _amplitude = value; }
    }

    private void Start(){
        posOffset = transform.position;
    }

    private void Update(){
        //rotate around y axis
        transform.Rotate(new Vector3(0f, Time.deltaTime * degPerSec, 0f), Space.World);
        
        //float in place
        if(!GetComponent<ObjectBase>().isHeld){
            tempPos = posOffset;
            tempPos.y += Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude;

            transform.position = tempPos;
        }
    }
}