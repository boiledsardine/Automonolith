using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBase : MonoBehaviour {
    public bool isOccupied = false;
    public TileBase neighborN, neighborE, neighborS, neighborW;

    public void OnTriggerEnter(Collider col) {
        isOccupied = true;
    }

    public void OnTriggerExit(Collider col) {
        isOccupied = false;
    }
}
