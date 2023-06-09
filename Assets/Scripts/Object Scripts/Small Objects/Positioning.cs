using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommandControl;

public class Positioning : MonoBehaviour{
    private GameObject playerCharacter;
    private Floating floatScript;

    //sets how high object hovers over player head when held
    [SerializeField] private float hoverDistance;
    
    //should be equal to yPos as much as possible
    [SerializeField] private float yOffset;

    private void Start(){
        playerCharacter = GameObject.Find("PlayerCharacter");
        floatScript = GetComponent<Floating>();
    }

    private void Update(){
        if(GetComponent<ObjectBase>().isHeld){
            relocate();
        }
    }
    
    //keeps small object over player's head while held
    private void relocate(){
        Vector3 playerPosition = playerCharacter.transform.position;
        transform.position = new Vector3(playerPosition.x,
        (playerPosition.y + hoverDistance), playerPosition.z);
    }

    //puts small object down
    //keeps small object over player's head if target tile is null
    public void release(){
        TileBase releaseTile = null;
        try{
            releaseTile = playerCharacter.GetComponent<CommandControl.Environment>().tileFront;
        } catch (NullReferenceException){
            Debug.LogWarning("Target tile is null!");
            floatScript.amplitude = 0f;
            floatScript.posOffset = transform.position;
        }
        
        if(releaseTile != null){
            Vector3 releasePoint = releaseTile.transform.position;
            if(floatScript != null){
                floatScript.posOffset = new Vector3(releasePoint.x,
                (releasePoint.y + yOffset), releasePoint.z);
            }
        } else {
            Compiler.Instance.terminateExecution();
        }
    }
}
