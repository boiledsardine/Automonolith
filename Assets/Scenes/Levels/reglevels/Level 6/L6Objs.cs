using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class L6Objs : ObjectiveBase{
    public string objText1;
    public TMPro.TMP_Text primObj, secObj1, secObj2, secObjHeader;

    void Start(){
        primObj.text = ">" + objText1;
    }

    void Update(){
        var successColor = ObjectiveManager.Instance.successColor;
        if(Objective1()){
            primObj.color = successColor;
        } else {
            primObj.color = defaultColor;
        }
    }

    public override bool IsComplete(){
        return true;
    }

    public override bool Objective1(){
        //reach the correct exit
        if(ObjectiveManager.Instance.exitTouched){
            return true;
        } else {
            return false;
        }
    }

    public override bool Objective2(){
        return true;
    }

    public override bool Objective3(){
        return true;
    }
}
