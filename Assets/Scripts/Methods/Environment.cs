using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommandControl{

public class Environment : MonoBehaviour {
    public static Environment Instance { get; private set; }
    
    private Collider col;
    private TileBase tileUnder;

    public TileBase startTile;

    void Awake(){
        if(Instance == null){
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }
    
    //checks the tile under the character
    //then retrieves neighbors of tile under character
    //then decides whether tile is valid or not
    //unoccupied and exists = valid
    //occupied or null = invalid
    public bool neighborIsValid(string direction){
        Debug.Log("Standing on: " + tileUnder.name);
        switch(direction){
            case "north":
                if(tileUnder.neighborN != null && !tileUnder.neighborN.isOccupied){
                    return true;
                } else {
                    return false;
                }
            case "south":
                if(tileUnder.neighborS != null && !tileUnder.neighborS.isOccupied){
                    return true;
                } else {
                    return false;
                }
            case "west":
                if(tileUnder.neighborW != null && !tileUnder.neighborW.isOccupied){
                    return true;
                } else {
                    return false;
                }
            case "east":
                if(tileUnder.neighborE != null && !tileUnder.neighborE.isOccupied){
                    return true;
                } else {
                    return false;
                }
            default:
                return false;
        }
    }

    public void resetStartTile(){
        tileUnder = startTile;
        Debug.LogWarning("Standing on starting tile");
    }

    //assigns Collider col with object from colliding trigger (in this case, tiles)
    private void OnTriggerEnter(Collider other){
        col = other;
        tileUnder = col.gameObject.GetComponent<TileBase>();
        Debug.Log("Entered tile: " + tileUnder.name);
    }

    private void declareTile(Collider other){
        if(col.gameObject.tag == "TileBlack"){
            Debug.Log("Black tile");
        } else if(col.gameObject.tag == "TileWhite"){
            Debug.Log("White tile");
        }
    }
}

}