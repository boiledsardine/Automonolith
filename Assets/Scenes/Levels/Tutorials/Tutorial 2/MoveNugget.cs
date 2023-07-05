using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveNugget : MonoBehaviour{
    TutorialManager2 tm2;
    public GameObject[] tpLocs;

    private void Awake(){
        tm2 = GameObject.Find("TutorialManager2").GetComponent<TutorialManager2>();
        Move();
    }
    
    public void Move(){
        switch(tm2.currentObjective){
            case Tutorial2Objectives.MethodsIntro:
                gameObject.transform.position = tpLocs[0].transform.position;
            break;
            case Tutorial2Objectives.MethodsPlay:
                gameObject.transform.position = tpLocs[1].transform.position;
            break;
            case Tutorial2Objectives.ParamsIntro:
                gameObject.transform.position = tpLocs[2].transform.position;
            break;
            case Tutorial2Objectives.TurnInteract:
                gameObject.transform.position = tpLocs[3].transform.position;
            break;
            case Tutorial2Objectives.TurnInteract2:
                gameObject.transform.position = tpLocs[4].transform.position;
            break;
            case Tutorial2Objectives.FinalStretch:
                gameObject.transform.position = tpLocs[5].transform.position;
            break;
        }
    }
}
