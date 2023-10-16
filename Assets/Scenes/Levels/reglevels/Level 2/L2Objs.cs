using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class L2Objs : ObjectiveBase{
    public TMPro.TMP_Text primObj, secObj1, secObj2, secObjHeader;
    void Start(){
        primObj.text = ">Reach the exit";
        
        secObjHeader.transform.gameObject.SetActive(true);
        secObj1.transform.gameObject.SetActive(true);
        secObj1.text = ">Use 7 lines or less";
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
        return true;
    }

    public override bool Objective3(){
        //count statements
        if(Compiler.Instance.linesCount > 7){
            return false;
        }
        return true;
    }
}
