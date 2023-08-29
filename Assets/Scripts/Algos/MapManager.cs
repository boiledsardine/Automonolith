using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour{
    public static MapManager Instance { get; private set; }
    public TileBase tilePrefab;
    public GameObject tileGrid;

    public Dictionary<Vector2Int, TileBase> map;
    void Awake(){
        if(Instance == null){
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    void Start(){
        var tileMap = gameObject.GetComponentInChildren<Tilemap>();
        BoundsInt bounds = tileMap.cellBounds;
        //loop through all tiles
        for(int z = bounds.max.z; z > bounds.min.z; z--){
            for(int y = bounds.min.y; y < bounds.max.y; y++){
                for(int x = bounds.min.x; x < bounds.max.x; x++){
                    var tileLocation = new Vector3Int(x, y, z);

                    if(tileMap.HasTile(tileLocation)){
                        
                    }
                }
            }
        }
    }
}