using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonBase : MonoBehaviour{
    protected Vector3 originPos;
    [SerializeField] protected GameObject boundObject;

    protected void Start(){
        originPos = transform.position;
    }
}
