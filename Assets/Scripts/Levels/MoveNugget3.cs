using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveNugget3 : MonoBehaviour{
    public GameObject[] tpLocs;
    int index;

    private void Start(){
        index = Globals.Instance.moveIndex;
        Move(index);
    }
    
    public void Move(int index){
        gameObject.transform.position = tpLocs[index].transform.position;
        gameObject.transform.rotation = tpLocs[index].transform.rotation;
    }
}