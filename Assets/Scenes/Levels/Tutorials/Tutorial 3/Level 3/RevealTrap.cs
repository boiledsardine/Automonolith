using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevealTrap : MonoBehaviour{
    [SerializeField] private PlainTile cover;
    [SerializeField] private FloorButton button;
    
    public void OnTriggerEnter(Collider col){
        cover.GetComponent<MeshRenderer>().enabled = false;
        button.gameObject.GetComponent<MeshRenderer>().enabled = false;
        gameObject.GetComponent<MeshRenderer>().enabled = true;
    }
}
