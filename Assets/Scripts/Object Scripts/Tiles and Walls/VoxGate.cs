using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxGate : GateBase, IActivate, IVox{
    public string password;
    public bool caseInsensitive;
    public Material[] errorColor;

    public void activate(){
        gameObject.layer = 0;
        var animator = GetComponent<Animator>();
        animator.SetBool("isOpen", true);
    }

    public void activateVox(string input){
        if(caseInsensitive){
            if(password.ToLower() == input.ToLower()){
                activate();
            } else {
                var mr = gameObject.GetComponent<MeshRenderer>();
                mr.materials = errorColor;
                gameObject.GetComponent<BotKiller>().activate();
            }
        } else {
            if(password == input){
                activate();
            } else {
                var mr = gameObject.GetComponent<MeshRenderer>();
                mr.materials = errorColor;
                gameObject.GetComponent<BotKiller>().activate();
            }
        }
    }

    public void deactivate(){
        throw new System.NotImplementedException();
    }

    //todo: thing that says wrong password called from interaction
}
