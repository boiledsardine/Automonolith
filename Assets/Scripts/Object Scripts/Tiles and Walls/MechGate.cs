using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechGate : GateBase, IActivate{
    public void activate(){
        gameObject.layer = 0;

        var animator = GetComponent<Animator>();
        animator.SetBool("isOpen", true);
        //StartCoroutine(openGate());
        UpdateTiles(false);
    }

    public void deactivate(){
        gameObject.layer = 6;

        var animator = GetComponent<Animator>();
        animator.SetBool("isOpen", false);
        //StartCoroutine(closeGate());
        UpdateTiles(true);
    }

    void UpdateTiles(bool enableCollider){
        var boxCol = gameObject.GetComponent<BoxCollider>();
        boxCol.enabled = enableCollider;

        tileFront.GetTileNeighbors();
        tileBack.GetTileNeighbors();
    }
}
