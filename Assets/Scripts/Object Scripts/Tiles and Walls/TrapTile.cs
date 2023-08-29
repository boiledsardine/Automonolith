using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommandControl;

public class TrapTile : TileBase {
    public bool isHidden = false;
    
    public Mesh trapMesh;
    public Material trapMats;
    public Mesh tileMesh;
    public Material tileMats;

    MeshFilter mf;
    MeshRenderer mr;
    
    new void Awake(){
        base.Awake();

        mf = GetComponent<MeshFilter>();
        mr = GetComponent<MeshRenderer>();

        if(isHidden){
            mf.mesh = tileMesh;
            mr.material = tileMats;
        }
    }

    public new void OnTriggerEnter(Collider col){
        base.OnTriggerEnter(col);
        if(occupant.tag == "Player"){
            if(isHidden){
                //unhide
                mf.mesh = trapMesh;
                mr.material = trapMats;
            }

            Compiler.Instance.terminateExecution();
            Destroy(col.gameObject);
            isOccupied = false;
        }
    }
}
