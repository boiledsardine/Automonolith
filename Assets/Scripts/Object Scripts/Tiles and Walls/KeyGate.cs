using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommandControl;

public class KeyGate : GateBase, IActivate{
    [SerializeField] KeyPickup boundKey;
    Interaction playerAct;

    private new void Start(){
        base.Start();
        //playerAct = GameObject.Find("PlayerCharacter").GetComponent<Interaction>();
        Invoke("activate", 5f);
        Invoke("deactivate", 8f);
    }

    public void activate(){
        gameObject.layer = 0;
        var animator = GetComponent<Animator>();
        animator.SetBool("isOpen", true);
        
        /*if(playerAct.isHoldingSmall){
            GameObject playerHeldObject = playerAct.heldObject;
            if(playerHeldObject == boundKey.transform.gameObject){
                playerAct.drop_NoExecute();
                StartCoroutine(openGate());
                Destroy(boundKey.transform.gameObject);
            }
        }*/
    }

    public void deactivate(){
        gameObject.layer = 0;
        var animator = GetComponent<Animator>();
        animator.SetBool("isOpen", false);
        //StartCoroutine(closeGate());
    }
}
