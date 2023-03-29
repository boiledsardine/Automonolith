using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommandControl;

public class KeyGate : GateBase, IActivate{
    [SerializeField] KeyPickup boundKey;
    Interaction playerAct;

    private new void Start(){
        base.Start();
        playerAct = GameObject.Find("PlayerCharacter").GetComponent<Interaction>();
    }

    public void activate(){
        if(playerAct.isHoldingSmall){
            GameObject playerHeldObject = playerAct.heldObject;
            if(playerHeldObject == boundKey.transform.gameObject){
                playerAct.drop_NoExecute();
                StartCoroutine(openGate());
                Destroy(boundKey.transform.gameObject);
            }
        }
    }

    public void deactivate(){
        StartCoroutine(closeGate());
    }
}
