using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class L3Objs : ObjectiveBase{
    public string objText1, objText2, objText3;
    public TMPro.TMP_Text primObj, secObj1, secObj2, secObjHeader;
    void Start(){
        primObj.text = ">" + objText1;
        
        secObj1.transform.gameObject.SetActive(true);
        secObj1.text = ">" + objText2;

        secObj2.transform.gameObject.SetActive(true);
        secObj2.text = ">" + objText3;
    }

    public override bool IsComplete(){
        return true;
    }

    void Update(){
        var successColor = ObjectiveManager.Instance.successColor;
        if(Objective1()){
            primObj.color = successColor;
        } else {
            primObj.color = defaultColor;
        }
        
        if(Objective2()){
            secObj1.color = successColor;
        } else {
            secObj1.color = defaultColor;
        }

        if(Objective3()){
            secObj2.color = successColor;
        } else {
            secObj2.color = defaultColor;
        }
    }

    public override bool Objective1(){
        //reach the exit
        if(ObjectiveManager.Instance.exitTouched){
            return true;
        } else {
            return false;
        }
    }

    public override bool Objective2(){
        //count statements
        if(Compiler.Instance.moveCount <= 7){
            return true;
        } else {
            return false;
        }
    }

    public override bool Objective3(){
        //gem
        GemPickup[] activeGems = FindObjectsOfType<GemPickup>();
        if(activeGems.Length != 0){
            return false;
        }
        return true;
    }
}
