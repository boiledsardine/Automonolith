using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour{
    //everything node related
    public int g;
    public int h;
    public int f{
        get { return g + h; }
    }
    
    public bool isBlocked;
    public Node previousTile;
    public Vector3Int gridLocation; 

    //everything tile related
    protected GameObject _occupant;
    protected string _tileName;
    [SerializeField] protected bool _isOccupied = false;
    [SerializeField] protected Node _neighborN, _neighborE, _neighborS, _neighborW;
    public bool isActive = false;

    //tile fires four raycasts at each direction
    //detects objects with TileBase scripts (layer 7)
    //if the raycast hits a tile, the tile assigns the hit to a neighbor tile
    //if the raycast hits a wall, tile in that direction is nulled
    private void Start(){
        tileName = gameObject.name;
    }

    void Update(){
        int layermask = 1 << 7;
        int wallmask = 1 << 6;
        float distance = Globals.Instance.distancePerTile;
        Vector3 north = new Vector3(0, 0, 1);
        Vector3 south = new Vector3(0, 0, -1);
        Vector3 east = new Vector3(1, 0, 0);
        Vector3 west = new Vector3(-1, 0, 0);
        RaycastHit hit;
        RaycastHit wallHit;

        if(!Physics.Raycast(transform.position, north, out wallHit, distance, wallmask)){
            if(Physics.Raycast(transform.position, north, out hit, distance, layermask)){
                neighborN = hit.transform.gameObject.GetComponent<Node>();
            }
        } else {
            neighborN = null;
        }
        if(!Physics.Raycast(transform.position, south, out wallHit, distance, wallmask)){
            if(Physics.Raycast(transform.position, south, out hit, distance, layermask)){
                neighborS = hit.transform.gameObject.GetComponent<Node>();
            }
        } else { 
            neighborS = null;
        }
        if(!Physics.Raycast(transform.position, east, out wallHit, distance, wallmask)){
            if(Physics.Raycast(transform.position, east, out hit, distance, layermask)){
                neighborE = hit.transform.gameObject.GetComponent<Node>();
            }
        } else {
            neighborE = null;
        }
        if(!Physics.Raycast(transform.position, west, out wallHit, distance, wallmask)){
            if(Physics.Raycast(transform.position, west, out hit, distance, layermask)){
                neighborW = hit.transform.gameObject.GetComponent<Node>();
            }
        } else {
            neighborW = null;
        }
    }

    //variable getters and setters
    public bool isOccupied{
        get { return _isOccupied; }
        set { _isOccupied = value; }
    }

    public string tileName{
        get { return _tileName; }
        set { _tileName = value; }
    }

    public GameObject occupant{
        get { return _occupant; }
        set { _occupant = value; }
    }

    //neighbor tile getters and setters
    public Node neighborN{
        get { return _neighborN; }
        set { _neighborN = value; }
    }

    public Node neighborS{
        get { return _neighborS; }
        set { _neighborS = value; }
    }

    public Node neighborW{
        get { return _neighborW; }
        set { _neighborW = value; }
    }

    public Node neighborE{
        get { return _neighborE; }
        set { _neighborE = value; }
    }
    
    //checks for occupants
    protected void OnTriggerEnter(Collider col){
        occupant = col.gameObject;
        if(col.tag != "Player"){
            isOccupied = true;
        }
    }

    protected void OnTriggerExit(Collider col){
        occupant = null;
        isOccupied = false;
    }
}
