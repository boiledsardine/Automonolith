using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonBase : MonoBehaviour{
    protected Vector3 originPos;
    [SerializeField] public GameObject boundObject;

    public bool latch = false;
    public bool latchActive = false;
    public Material latchColor;
    private bool _isActivated = false;
    
    protected void Start(){
        originPos = transform.position;
    }

    public bool isActivated{
        get { return _isActivated; }
        set { _isActivated = value; }
    }
}
