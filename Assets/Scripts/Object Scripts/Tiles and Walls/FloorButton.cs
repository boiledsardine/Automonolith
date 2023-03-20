using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorButton : ButtonBase{
    private bool _isActivated = false;
    private Material currentMaterial;
    private Renderer render;
    [SerializeField] Material colorOff;
    [SerializeField] Material colorOn;

    public bool isActivated{
        get { return _isActivated; }
        set { _isActivated = value; }
    }

    private new void Start(){
        base.Start();
        render = GetComponent<Renderer>();
        render.enabled = true;
        render.sharedMaterial = colorOff;
    }

    private void OnTriggerEnter(Collider col) {
        if(col.gameObject.tag == "Big Object" || col.gameObject.tag == "Player"){
            isActivated = true;
            render.sharedMaterial = colorOn;
            transform.position = originPos - new Vector3(0f, 3f, 0f);
            Debug.Log(gameObject.name + " is live");
            boundObject.GetComponent<IActivate>().activate();
        }
    }

    private void OnTriggerExit(Collider col) {
        if(col.gameObject.tag == "Big Object" || col.gameObject.tag == "Player"){
            isActivated = false;
            render.sharedMaterial = colorOff;
            transform.position = originPos;
            Debug.Log(gameObject.name + " is dead");
            boundObject.GetComponent<IActivate>().deactivate();
        }
    }
}
