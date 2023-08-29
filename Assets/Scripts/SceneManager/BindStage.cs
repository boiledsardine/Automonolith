using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BindStage : MonoBehaviour{
    public int num;
    void Awake(){
        var tutorialManager = GameObject.Find("TutorialManager2").GetComponent<TutorialBase>();
        tutorialManager.stages[num] = gameObject;
        gameObject.SetActive(false);
        if(tutorialManager.activeStage == num){
            gameObject.SetActive(true);
        }
    }
}
