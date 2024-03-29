using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Globals : MonoBehaviour{
    public static Globals Instance { get; private set; }

    void Awake(){
        if(Instance == null){
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    //time for all objects to move in seconds
    //default: 0.5f
    [SerializeField] private float _timeToMove = 0.25f;
    
    //time for all objects to rotate in seconds
    //default: 0.5f
    [SerializeField] private float _timeToRotate = 0.25f;

    //time for each in-game step to occur
    //default: 0.25f
    [SerializeField] private float _timePerStep = 0.25f;

    //distance between the center of each tile in units
    //should always match tile scale in units
    //default: 100f
    [SerializeField] private float _distancePerTile = 100f;

    //distance between the center of each object on a tile
    //should always be half the distance per tile
    //default: 50f
    [SerializeField] private float _distancePerObject = 50f;

    [SerializeField] private float _pickupYFixedPos = 140f;

    //getters and setters
    public float timeToMove{
        get { return _timeToMove; }
    }

    public float timeToRotate{
        get { return _timeToRotate; }
    }

    public float timePerStep{
        get { return _timePerStep; }
    }

    public float distancePerTile{
        get { return _distancePerTile; }
    }

    public float distancePerObject{
        get { return _distancePerObject; }
    }

    public float pickupYFixedPos{
        get { return _pickupYFixedPos; }
    }

    public int moveIndex = 0;
    
    //TODO: TT2 function for 1.5x and 2x
    //low prio
}
