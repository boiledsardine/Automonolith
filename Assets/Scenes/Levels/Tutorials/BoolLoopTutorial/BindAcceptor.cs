using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BindAcceptor : MonoBehaviour{
    public void Awake(){
        var acceptor = GetComponent<ItemAcceptor>();
        acceptor.boundObject = GameObject.Find("TutorialManager2");
    }
}
