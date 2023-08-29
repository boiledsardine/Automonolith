using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBase : MonoBehaviour {
    //everything node related
    public int g;
    public int h;
    public int f{
        get { return g + h; }
    }
    
    public bool isBlocked {
        get{ return isOccupied; }
    }
    public TileBase previousTile;
    public Vector2Int gridLocation;

    //related to tile types
    public bool isDirtTile = false;

    //everything tile related
    protected GameObject _occupant;
    protected string _tileName;
    [SerializeField] protected bool _isOccupied = false;
    [SerializeField] protected TileBase _neighborN, _neighborE, _neighborS, _neighborW;
    public bool isActive = false;

    //upon start, tile fires four raycasts at each direction
    //detects objects with TileBase scripts (layer 7)
    //if the raycast hits a tile, the tile assigns the hit to a neighbor tile
    //everything has to be done in Awake()
    //so Pathfinder doesn't take nulls
    protected void Awake(){
        tileName = gameObject.name;
        GetTileNeighbors();
        SetCoordinates();
    }

    void Update(){
        GetTileNeighbors();
    }

    void GetTileNeighbors(){
        int layermask = 1 << 7;
        int wallmask = 1 << 6;
        float distance = 100f;
        Vector3 north = new Vector3(0, 0, 1);
        Vector3 south = new Vector3(0, 0, -1);
        Vector3 east = new Vector3(1, 0, 0);
        Vector3 west = new Vector3(-1, 0, 0);
        RaycastHit hit;
        RaycastHit wallHit;

        if(!Physics.Raycast(transform.position, north, out wallHit, distance, wallmask)){
            if(Physics.Raycast(transform.position, north, out hit, distance, layermask)){
                neighborN = hit.transform.gameObject.GetComponent<TileBase>();
            }
        } else {
            neighborN = null;
        }
        if(!Physics.Raycast(transform.position, south, out wallHit, distance, wallmask)){
            if(Physics.Raycast(transform.position, south, out hit, distance, layermask)){
                neighborS = hit.transform.gameObject.GetComponent<TileBase>();
            }
        } else { 
            neighborS = null;
        }
        if(!Physics.Raycast(transform.position, east, out wallHit, distance, wallmask)){
            if(Physics.Raycast(transform.position, east, out hit, distance, layermask)){
                neighborE = hit.transform.gameObject.GetComponent<TileBase>();
            }
        } else {
            neighborE = null;
        }
        if(!Physics.Raycast(transform.position, west, out wallHit, distance, wallmask)){
            if(Physics.Raycast(transform.position, west, out hit, distance, layermask)){
                neighborW = hit.transform.gameObject.GetComponent<TileBase>();
            }
        } else {
            neighborW = null;
        }
    }

    private void SetCoordinates(){
        //take tilename
        //split by char and number
        //char is y coord, number is x coord
        gridLocation.x = int.Parse(tileName.Substring(1));
        gridLocation.y = CharToNum(tileName[0]);
    }

    private int CharToNum(char c){
        return c switch{
            'A' => 0,
            'B' => 1,
            'C' => 2,
            'D' => 3,
            'E' => 4,
            'F' => 5,
            'G' => 6,
            'h' => 7,
            'I' => 8,
            'J' => 9,
            _ => 0,
        };
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
        if(col.name != "CubeKey"){
            occupant = null;
            isOccupied = false;
        }
    }

    protected void ChangeActive(){
        //change the color
        isActive = true;
    }

    private void DrawRays(){
        Debug.DrawRay(transform.position, Vector3.back * Globals.Instance.distancePerTile, Color.red);
        Debug.DrawRay(transform.position, Vector3.forward * Globals.Instance.distancePerTile, Color.blue);
        Debug.DrawRay(transform.position, Vector3.left * Globals.Instance.distancePerTile, Color.yellow);
        Debug.DrawRay(transform.position, Vector3.right * Globals.Instance.distancePerTile, Color.green);
    }
}
