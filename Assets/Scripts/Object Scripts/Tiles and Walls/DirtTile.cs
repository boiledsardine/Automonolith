using System.Collections;
using System.Collections.Generic;
using CommandControl;
using UnityEngine;

public class DirtTile : TileBase{
    public Mesh mesh;
    public Material material;

    public GameObject storedObject;
    public ObjectType objectType;

    public int hoverDistance = 140; //default offset of held objects
    public float vertSpawnLoc = 30f; //30 by default
    
    void Start(){
        isDirtTile = true;
    }

    public void DigTile(){
        var mf = gameObject.GetComponent<MeshFilter>();
        var mr = gameObject.GetComponent<MeshRenderer>();

        mf.mesh = mesh;
        mr.material = material;

        if(storedObject != null){
            if(objectType == ObjectType.holdable){
                var player = GameObject.Find("PlayerCharacter");
                Vector3 itemPos = new Vector3(player.transform.position.x, 140f, transform.position.z);
                var item = Instantiate(storedObject, itemPos, storedObject.transform.rotation);
                item.name = storedObject.name;
                item.GetComponent<ObjectBase>().isHeld = true;

                var playerAct = player.GetComponent<Interaction>();
                playerAct.heldObject = item;
                playerAct.isHolding = true;
                playerAct.isHoldingSmall = true;
            } else {
                Vector3 itemPos = new Vector3(transform.position.x, vertSpawnLoc, transform.position.z);
                var item = Instantiate(storedObject, itemPos, storedObject.transform.rotation);
                item.name = storedObject.name;
            }
        }
    }

    public enum ObjectType{
        pickup,
        holdable
    }
}
