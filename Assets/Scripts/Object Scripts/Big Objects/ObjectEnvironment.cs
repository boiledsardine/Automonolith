using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectEnvironment : MonoBehaviour, IEnvironment{
    [SerializeField] private TileBase _tileUnder;

    public TileBase tileUnder{
        get { return _tileUnder; }
        set { _tileUnder = value; }
    }

    void Update(){
        Vector3 origin = new Vector3(transform.position.x, transform.position.y + 200, transform.position.z);
        Debug.DrawRay(origin, Vector3.down * 100, Color.blue);
    }

    void Start(){
        tileUnder = GetTileUnder();
    }

    public bool neighborIsValid(char direction){
        tileUnder = GetTileUnder();
        switch(direction){
            case 'N':
                if(tileUnder.neighborN != null && !tileUnder.neighborN.isOccupied){
                    return true;
                } else if (tileUnder.neighborN == null){
                    Compiler.Instance.addErr(string.Format("Line {0}: can't move held object there - there's no tile!", Compiler.Instance.currentIndex + 1));
                    Compiler.Instance.errorChecker.writeError();
                    Compiler.Instance.killTimer();
                    Debug.LogWarning("Destination is null");
                    return false;
                } else {
                    Compiler.Instance.addErr(string.Format("Line {0}: can't move held object to the destination tile - it's already occupied!", Compiler.Instance.currentIndex + 1));
                    Compiler.Instance.errorChecker.writeError();
                    Compiler.Instance.killTimer();
                    Debug.LogWarning("Destination " + tileUnder.neighborN + " is occupied");
                    return false;
                }
            case 'S':
                if(tileUnder.neighborS != null && !tileUnder.neighborS.isOccupied){
                    return true;
                } else if (tileUnder.neighborS == null){
                    Compiler.Instance.addErr(string.Format("Line {0}: can't move held object there - there's no tile!", Compiler.Instance.currentIndex + 1));
                    Compiler.Instance.errorChecker.writeError();
                    Compiler.Instance.killTimer();
                    Debug.LogWarning("Destination is null");
                    return false;
                } else {
                    Compiler.Instance.addErr(string.Format("Line {0}: can't move held object to the destination tile - it's already occupied!", Compiler.Instance.currentIndex + 1));
                    Compiler.Instance.errorChecker.writeError();
                    Compiler.Instance.killTimer();
                    Debug.LogWarning("Destination " + tileUnder.neighborS + " is occupied");
                    return false;
                }
            case 'W':
                if(tileUnder.neighborW != null && !tileUnder.neighborW.isOccupied){
                    return true;
                } else if (tileUnder.neighborW == null){
                    Compiler.Instance.addErr(string.Format("Line {0}: can't move held object there - there's no tile!", Compiler.Instance.currentIndex + 1));
                    Compiler.Instance.errorChecker.writeError();
                    Compiler.Instance.killTimer();
                    Debug.LogWarning("Destination is null");
                    return false;
                } else {
                    Compiler.Instance.addErr(string.Format("Line {0}: can't move held object to the destination tile - it's already occupied!", Compiler.Instance.currentIndex + 1));
                    Compiler.Instance.errorChecker.writeError();
                    Compiler.Instance.killTimer();
                    Debug.LogWarning("Destination " + tileUnder.neighborW + " is occupied");
                    return false;
                }
            case 'E':
                if(tileUnder.neighborE != null && !tileUnder.neighborE.isOccupied){
                    return true;
                } else if (tileUnder.neighborE == null){
                    Compiler.Instance.addErr(string.Format("Line {0}: can't move held object there - there's no tile!", Compiler.Instance.currentIndex + 1));
                    Compiler.Instance.errorChecker.writeError();
                    Compiler.Instance.killTimer();
                    Debug.LogWarning("Destination is null");
                    return false;
                } else {
                    Compiler.Instance.addErr(string.Format("Line {0}: can't move held object to the destination tile - it's already occupied!", Compiler.Instance.currentIndex + 1));
                    Compiler.Instance.errorChecker.writeError();
                    Compiler.Instance.killTimer();
                    Debug.LogWarning("Destination " + tileUnder.neighborE + " is occupied");
                    return false;
                }
            default:
                return false;
        }
    }

    public TileBase GetTileUnder(){
        Vector3 origin = new Vector3(transform.position.x, transform.position.y + 200, transform.position.z);
        if(Physics.Raycast(origin, Vector3.down, out RaycastHit hit, Globals.Instance.distancePerTile, 1 << 7)){
            return hit.transform.gameObject.GetComponent<TileBase>();
        } else {
            return null;
        }
    }

    public void OnTriggerEnter(Collider other){
        //tileUnder = other.gameObject.GetComponent<TileBase>();
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
            Compiler.Instance.addErr(string.Format("Line {0}: can't move held objects through walls!", Compiler.Instance.currentIndex + 1));
            Compiler.Instance.errorChecker.writeError();
            Compiler.Instance.killTimer();
            Debug.LogWarning(gameObject.name + ": Wall found");
            return true;
        } else {
            return false;
        }
    }
}
