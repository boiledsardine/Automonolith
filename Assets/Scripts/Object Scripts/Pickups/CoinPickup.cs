using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPickup : ObjectBase{
    [SerializeField] private float degPerSec = 30f;

    private new void Start(){
        base.Start();
        isMovable = false;
    }

    private void OnTriggerEnter(Collider col) {
        if(col.gameObject.tag == "Player"){
            Debug.Log("Gem picked up!");
            Destroy(this.gameObject);
        }
    }

    private void Update(){
        transform.Rotate(new Vector3(0f, Time.deltaTime * degPerSec, 0f), Space.World);
    }
}
