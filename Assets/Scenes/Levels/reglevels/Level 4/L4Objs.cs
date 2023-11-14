using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class L4Objs : ObjectiveBase{
    public TMPro.TMP_Text primObj, primObj2, secObj1, secObjHeader;
    int intVarCount;
    void Start(){
        intVarCount = Compiler.Instance.intVars.Count;

        primObj.text = ">Reach the coordinate in the blue panel";
        primObj2.text = ">Add the values of the green panels";
        
        secObjHeader.transform.gameObject.SetActive(true);
        secObj1.transform.gameObject.SetActive(true);
        secObj1.text = ">Use only one int variable";
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
            primObj2.color = successColor;
        } else {
            primObj2.color = defaultColor;
        }
        
        if(Objective3()){
            secObj1.color = successColor;
        } else {
            secObj1.color = defaultColor;
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
        //add stuff
        if(ObjectiveManager.Instance.exitTouched){
            return true;
        } else {
            return false;
        }
    }

    public override bool Objective3(){
        if(Compiler.Instance.intVars.Count - intVarCount > 1){
            return false;
        } else {
            return true;
        }
    }
}
