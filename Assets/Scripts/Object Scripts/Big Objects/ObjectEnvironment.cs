using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectEnvironment : MonoBehaviour, IEnvironment{
    [SerializeField] private TileBase _tileUnder;

    public TileBase tileUnder{
        get { return _tileUnder; }
        set { _tileUnder = value; }
    }

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

    public void OnTriggerEnter(Collider other){
        tileUnder = other.gameObject.GetComponent<TileBase>();
    }

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
