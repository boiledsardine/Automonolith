using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommandControl{

public class Environment : MonoBehaviour, IEnvironment {
    [SerializeField] private TileBase tileUnder;
    private TileBase _tileFront;

    private Movement moveScript;

    private void Awake(){
        moveScript = gameObject.GetComponent<Movement>();
    }

    public TileBase currentTile{
        get { return tileUnder; }
    }

    public TileBase tileFront{
        get{
            switch(moveScript.facing){
                case 'N':
                    return tileUnder.neighborN.GetComponent<TileBase>();
                case 'S':
                    return tileUnder.neighborS.GetComponent<TileBase>();
                case 'W':
                    return tileUnder.neighborW.GetComponent<TileBase>();
                case 'E':
                    return tileUnder.neighborE.GetComponent<TileBase>();
                default:
                    return null;
            }
        }
    }
    
    //called by Movement methods
    //checks the tile under the character
    //then retrieves neighbors of tile under character
    //then decides whether tile is valid or not
    //unoccupied and exists = valid
    //occupied or null = invalid
    public bool neighborIsValid(char direction){
        switch(direction){
            case 'N':
                if(tileUnder.neighborN != null && !tileUnder.neighborN.isOccupied){
                    return true;
                } else if (tileUnder.neighborN == null){
                    Debug.LogWarning("Destination is null");
                    return false;
                } else {
                    Debug.LogWarning("Destination " + tileUnder.neighborN + " is occupied");
                    return false;
                }
            case 'S':
                if(tileUnder.neighborS != null && !tileUnder.neighborS.isOccupied){
                    return true;
                } else if (tileUnder.neighborS == null){
                    Debug.LogWarning("Destination is null");
                    return false;
                } else {
                    Debug.LogWarning("Destination " + tileUnder.neighborS + " is occupied");
                    return false;
                }
            case 'W':
                if(tileUnder.neighborW != null && !tileUnder.neighborW.isOccupied){
                    return true;
                } else if (tileUnder.neighborW == null){
                    Debug.LogWarning("Destination is null");
                    return false;
                } else {
                    Debug.LogWarning("Destination " + tileUnder.neighborW + " is occupied");
                    return false;
                }
            case 'E':
                if(tileUnder.neighborE != null && !tileUnder.neighborE.isOccupied){
                    return true;
                } else if (tileUnder.neighborE == null){
                    Debug.LogWarning("Destination is null");
                    return false;
                } else {
                    Debug.LogWarning("Destination " + tileUnder.neighborE + " is occupied");
                    return false;
                }
            default:
                return false;
        }
    }

    //assigns Collider col with object from colliding trigger (in this case, tiles)
    //also on Awake: sets the starting tile to level starting tile
    public void OnTriggerEnter(Collider col){
        if(col.gameObject.tag == "Tile"){
            tileUnder = col.gameObject.GetComponent<TileBase>();
        }
    }

    //checkForWalls and wallRaycast shoot a raycast in the direction player is moving
    //returns true if the raycast hits a wall (layer 6)
    public bool checkForWalls(char direction){
        switch(direction){
            case 'N':
                return wallRaycast(new Vector3(0, 0, 1));
            case 'S':
                return wallRaycast(new Vector3(0, 0, -1));
            case 'E':
                return wallRaycast(new Vector3(1, 0, 0));
            case 'W':
                return wallRaycast(new Vector3(-1, 0, 0));
            default:
                return false;
        }
    }

    private bool wallRaycast(Vector3 moveDir){
        int layerMask = 1 << 6;
        Vector3 fwd = transform.TransformDirection(Vector3.forward);
        float distance = Globals.Instance.distancePerObject;

        if(Physics.Raycast(transform.position, moveDir, distance, layerMask)){
            Debug.LogWarning(gameObject.name + ": Wall found");
            return true;
        } else {
            return false;
        }
    }
}

}