using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBase : MonoBehaviour {
    protected GameObject _occupant;
    protected string _tileName;
    [SerializeField] protected bool _isOccupied = false;
    [SerializeField] protected TileBase _neighborN, _neighborE, _neighborS, _neighborW;

    //upon start, tile fires four raycasts at each direction
    //detects objects with TileBase scripts (layer 7)
    //if the raycast hits a tile, the tile assigns the hit to a neighbor tile
    private void Start(){
        int layermask = 1 << 7;
        tileName = gameObject.name;
        float distance = Globals.Instance.distancePerTile;
        Vector3 north = new Vector3(0, 0, 1);
        Vector3 south = new Vector3(0, 0, -1);
        Vector3 east = new Vector3(1, 0, 0);
        Vector3 west = new Vector3(-1, 0, 0);
        RaycastHit hit;

        if(Physics.Raycast(transform.position, north, out hit, distance, layermask)){
            neighborN = hit.transform.gameObject.GetComponent<TileBase>();
        }
        if(Physics.Raycast(transform.position, south, out hit, distance, layermask)){
            neighborS = hit.transform.gameObject.GetComponent<TileBase>();
        }
        if(Physics.Raycast(transform.position, east, out hit, distance, layermask)){
            neighborE = hit.transform.gameObject.GetComponent<TileBase>();
        }
        if(Physics.Raycast(transform.position, west, out hit, distance, layermask)){
            neighborW = hit.transform.gameObject.GetComponent<TileBase>();
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
    public TileBase neighborN{
        get { return _neighborN; }
        set { _neighborN = value; }
    }

    public TileBase neighborS{
        get { return _neighborS; }
        set { _neighborS = value; }
    }

    public TileBase neighborW{
        get { return _neighborW; }
        set { _neighborW = value; }
    }

    public TileBase neighborE{
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

    /*private void Update(){
        Debug.DrawRay(transform.position, Vector3.back * Globals.Instance.distancePerTile, Color.red);
        Debug.DrawRay(transform.position, Vector3.forward * Globals.Instance.distancePerTile, Color.blue);
        Debug.DrawRay(transform.position, Vector3.left * Globals.Instance.distancePerTile, Color.yellow);
        Debug.DrawRay(transform.position, Vector3.right * Globals.Instance.distancePerTile, Color.green);
    }*/
}
