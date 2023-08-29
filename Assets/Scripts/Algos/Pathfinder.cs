using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour{
    public static Pathfinder Instance { get; private set; }
    
    public TileBase startTile;
    public TileBase endTile;
    public Material mat;

    void Awake(){
        if(Instance == null){
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    public void ColorTiles(List<TileBase> tiles){
        foreach(TileBase tile in tiles){
            tile.gameObject.GetComponent<MeshRenderer>().material = mat;
        }
    }

    public List<char> GetDirections(List<TileBase> tiles){
        List<char> directions = new List<char>(0);
        //get a tile
        //get the names of its neigbors
        //get the next tile
        //compare
        for(int i = 0; i < tiles.Count; i++){
            var currentTile = tiles[i];
            Dictionary<string,char> neighborDirections = GetNeighborDirection(currentTile);
            
            if(i != tiles.Count - 1){
                var nextTile = tiles[i + 1];
                directions.Add(neighborDirections[nextTile.tileName]);
            }
        }
        return directions;
    }

    public List<TileBase> FindPath(TileBase start, TileBase end){
        List<TileBase> openList = new List<TileBase>();
        List<TileBase> closedList = new List<TileBase>();
        
        openList.Add(start);

        while(openList.Count > 0){
            TileBase currentTile = openList.OrderBy(x => x.f).First();

            openList.Remove(currentTile);
            closedList.Add(currentTile);

            if(currentTile == end){
                //finalize path
                return GetFinalList(start, end);
            }

            var neighborTiles = GetNeighborTiles(currentTile);

            foreach(var neighbor in neighborTiles){
                if(neighbor.isOccupied || closedList.Contains(neighbor)){
                    continue;
                }

                //calculate g and h cost (manhattan distance)
                //no diagonals plox
                neighbor.g = GetManhattan(start, neighbor);
                neighbor.h = GetManhattan(end, neighbor);

                neighbor.previousTile = currentTile;

                if(!openList.Contains(neighbor)){
                    openList.Add(neighbor);
                }
            }
        }

        Debug.LogWarning("Failed to find a path!");
        return new List<TileBase>();
    }

    private List<TileBase> GetFinalList(TileBase start, TileBase end){
        List<TileBase> finalList = new List<TileBase>();
        TileBase currentTile = end;

        while(currentTile != start){
            finalList.Add(currentTile);
            currentTile = currentTile.previousTile;
        }
        finalList.Add(start);

        finalList.Reverse();
        return finalList;
    }

    private int GetManhattan(TileBase start, TileBase neighbor){
        return Mathf.Abs(start.gridLocation.x - neighbor.gridLocation.x) + Mathf.Abs(start.gridLocation.y - neighbor.gridLocation.y);
    }

    private List<TileBase> GetNeighborTiles(TileBase currentTile){
        List<TileBase> neighbors = new List<TileBase>();
        
        //north
        if(currentTile.neighborN != null){
            neighbors.Add(currentTile.neighborN);
        }
        //south
        if(currentTile.neighborS != null){
            neighbors.Add(currentTile.neighborS);
        }
        //west
        if(currentTile.neighborW != null){
            neighbors.Add(currentTile.neighborW);
        }
        //east
        if(currentTile.neighborE != null){
            neighbors.Add(currentTile.neighborE);
        }      

        return neighbors;
    }

    private Dictionary<string, char> GetNeighborDirection(TileBase currentTile){
        Dictionary<string,char> neighborDirections = new Dictionary<string,char>();

        //north
        if(currentTile.neighborN != null){
            neighborDirections.Add(currentTile.neighborN.tileName, 'n');
        }
        //south
        if(currentTile.neighborS != null){
            neighborDirections.Add(currentTile.neighborS.tileName, 's');
        }
        //east
        if(currentTile.neighborE != null){
            neighborDirections.Add(currentTile.neighborE.tileName, 'e');
        }
        //west
        if(currentTile.neighborW != null){
            neighborDirections.Add(currentTile.neighborW.tileName, 'w');
        }

        return neighborDirections;
    }
}