using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommandControl;

public class TrapTile : TileBase {
    public new void OnTriggerEnter(Collider col){
        base.OnTriggerEnter(col);
        if(occupant.tag == "Player"){
            Compiler.Instance.terminateExecution();
            Destroy(col.gameObject);
            isOccupied = false;
        }
    }
}
