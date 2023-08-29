using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LSAnimatorControl : MonoBehaviour{
    Animator anim;
    public Button bLeft;
    public Button bRight;

    public void Start(){
        anim = GetComponent<Animator>();
    }

    public void MoveRight(){
        int moveCount = anim.GetInteger("moveCount");
        moveCount++;
        anim.SetInteger("moveCount", moveCount);
        if(moveCount == 2){
            bRight.interactable = false;
        }
        bLeft.interactable = true;
    }

    public void MoveLeft(){
        int moveCount = anim.GetInteger("moveCount");
        moveCount--;
        anim.SetInteger("moveCount", moveCount);
        if(moveCount == 0){
            bLeft.interactable = false;
        }
        bRight.interactable = true;
    }
}
