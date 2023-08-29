using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxGate : GateBase, IActivate{
    public string password;
    public bool caseInsensitive;

    public void activate(){
        gameObject.layer = 0;
        var animator = GetComponent<Animator>();
        animator.SetBool("isOpen", true);
        //StartCoroutine(openGate());
    }

    public void deactivate(){
        gameObject.layer = 6;
        var animator = GetComponent<Animator>();
        animator.SetBool("isOpen", false);
        //StartCoroutine(closeGate());
    }

    //todo: thing that says wrong password called from interaction
}
