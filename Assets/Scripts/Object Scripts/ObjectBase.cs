using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectBase : MonoBehaviour{
    protected bool _held;
    protected bool _isMovable;
    protected Vector3 _originPos;
    protected BoxCollider _collider;

    protected void Start(){
        _originPos = transform.position;
        _collider = GetComponent<BoxCollider>();
    }

    public bool isHeld{
        get { return _held; }
        set { _held = value; }
    }

    public bool isMovable{
        get { return _isMovable; }
        set { _isMovable = value; }
    }

    public Vector3 originPos{
        get { return _originPos; }
    }
}