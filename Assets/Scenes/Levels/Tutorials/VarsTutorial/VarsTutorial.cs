using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeEditorComponents;

public class VarsTutorial : TutorialBase{
    public VarsObjectives currentObjective;

    void Start(){
        switch(currentObjective){
            case VarsObjectives.Start:
                StartIntCall();
            break;
            case VarsObjectives.AfterIntCall:
                StartIntDeclare();
            break;
            case VarsObjectives.AfterIntDeclare:
                StartIntAssign();
            break;
            case VarsObjectives.AfterIntAssign:
                StartMathOps();
            break;
            case VarsObjectives.AfterMathOps:
                StartAssignOps();
            break;
        }
    }

    public override IEnumerator ActivateButton(){
        hasError = false;
        yield return new WaitForSeconds(1);
        if(hasError){
            hasError = false;
            yield break;
        }
        
        var ci = Compiler.Instance;
        switch(currentObjective){
            case VarsObjectives.IntCall:
                objectiveText[0].color = successColor;
                currentObjective = VarsObjectives.AfterIntCall;
                StartCoroutine(convoManager.StartDialogue(1, 0));
            break;
            case VarsObjectives.IntDeclare:
                //check compiler dictionaries
                if(ci.allVars.Count >= ci.reservedVars && ci.intVars.Count >= ci.reservedInts){
                    objectiveText[0].color = successColor;
                    objectiveText[1].color = successColor;
                    currentObjective = VarsObjectives.AfterIntDeclare;
                    StartCoroutine(convoManager.StartDialogue(3, 0));
                } else {
                    objectiveText[0].color = successColor;
                    objectiveText[1].color = Color.red;
                    StartCoroutine(convoManager.StartFailDialogue(0, 0));
                }
            break;
            case VarsObjectives.IntAssign:
                //check compiler dictionaries
                if(ci.allVars.Count == ci.reservedVars + 1 && ci.intVars["num"] == 2){
                    objectiveText[0].color = successColor;
                    objectiveText[1].color = successColor;
                    currentObjective = VarsObjectives.AfterIntAssign;
                    StartCoroutine(convoManager.StartDialogue(5, 0));
                } else {
                    objectiveText[0].color = successColor;
                    objectiveText[1].color = Color.red;
                    StartCoroutine(convoManager.StartFailDialogue(1, 0));
                }
            break;
            case VarsObjectives.MathOps:
                objectiveText[0].color = successColor;
                currentObjective = VarsObjectives.AfterMathOps;
                StartCoroutine(convoManager.StartDialogue(7, 0));
            break;
            case VarsObjectives.AssignOps:
                //check compiler dictionaries
                if(ci.allVars.Count == ci.reservedVars + 1 && ci.intVars.Count == ci.reservedInts + 1){
                    objectiveText[0].color = successColor;
                    objectiveText[1].color = successColor;
                    currentObjective = VarsObjectives.AfterAssignOps;
                    StartCoroutine(convoManager.StartDialogue(9, 0));
                } else {
                    objectiveText[0].color = successColor;
                    objectiveText[1].color = Color.red;
                    StartCoroutine(convoManager.StartFailDialogue(2, 0));
                }
            break;
            default:
            break;
        }
    }

    public override void DialogueEnd(){
        if(DialogueManager.Instance.ErrorDialogue){
            return;
        }
        
        if(hintIsOpen){
            hintIsOpen = false;
            return;
        }
        dialogueEndCount++;

        switch(dialogueEndCount){
            case 2:
                StartIntDeclare();
            break;
            case 4:
                StartIntAssign();
            break;
            case 6:
                StartMathOps();
            break;
            case 8:
                StartAssignOps();
            break;
            case 10:
                GameObject.Find("exit-point").GetComponent<IActivate>().activate();
            break;
            default: break;
        }
    }

    /*
    private int timesPressed = 0;
    public override void NextLinePressed(){
        timesPressed++;

        if(hintIsOpen && timesPressed == hintButton.convoToLoad.LineCount()){
            if(timesPressed == hintButton.convoToLoad.LineCount()){
                timesPressed = 0;
                hintIsOpen = false;
            }
            return;
        }
        
        switch(currentObjective){
            case VarsObjectives.AfterIntCall:
                if(timesPressed == convoManager.convos[1].LineCount()){
                    StartIntDeclare();
                    timesPressed = 0;
                }
            break;
            case VarsObjectives.AfterIntDeclare:
                if(timesPressed == convoManager.convos[3].LineCount()){
                    StartIntAssign();
                    timesPressed = 0;
                }
            break;
            case VarsObjectives.AfterIntAssign:
                if(timesPressed == convoManager.convos[5].LineCount()){
                    StartMathOps();
                    timesPressed = 0;
                }
            break;
            case VarsObjectives.AfterMathOps:
                if(timesPressed == convoManager.convos[7].LineCount()){
                    StartAssignOps();
                    timesPressed = 0;
                }
            break;
            case VarsObjectives.AfterAssignOps:
                if(timesPressed == convoManager.convos[9].LineCount()){
                    timesPressed = 0;
                    var exitpoint = GameObject.Find("exit-point").GetComponent<IActivate>();
                    exitpoint.activate();
                }
            break;
            default:
                timesPressed = 0;
            break;
        }
    }
    */
    
    void StartIntCall(){
        StartStage(0);

        //set enum mode
        currentObjective = VarsObjectives.IntCall;

        //set objective text
        objectiveText[0].text = ">Find the safest route";
        objectiveText[0].color = defaultColor;
    }

    void StartIntDeclare(){
        StartStage(1);

        //set enum mode
        currentObjective = VarsObjectives.IntDeclare;

        //set objective text
        objectiveText[0].text = ">Find the safe button";
        objectiveText[0].color = defaultColor;

        objectiveText[1].transform.gameObject.SetActive(true);
        objectiveText[1].text = ">Declare a variable";
        objectiveText[1].color = defaultColor;
    }

    void StartIntAssign(){
        StartStage(2);

        //set enum mode
        currentObjective = VarsObjectives.IntAssign;

        //set objective text
        objectiveText[0].text = ">Find the safe button";
        objectiveText[0].color = defaultColor;

        objectiveText[1].text = ">Reassign the given variable";
        objectiveText[1].color = defaultColor;
    }

    void StartMathOps(){
        StartStage(3);

        //set enum mode
        currentObjective = VarsObjectives.MathOps;

        //set objective text
        objectiveText[0].text = ">Reach the button";
        objectiveText[0].color = defaultColor;

        objectiveText[1].transform.gameObject.SetActive(false);
    }

    void StartAssignOps(){
        StartStage(4);

        //set enum mode
        currentObjective = VarsObjectives.AssignOps;

        //set objective text
        objectiveText[0].text = ">Reach the button";
        objectiveText[0].color = defaultColor;

        objectiveText[1].transform.gameObject.SetActive(true);
        objectiveText[1].text = ">Use only the given variable";
        objectiveText[1].color = defaultColor;
    }

    public enum VarsObjectives{
        Start,
        IntCall,
        AfterIntCall,
        IntDeclare,
        AfterIntDeclare,
        IntAssign,
        AfterIntAssign,
        MathOps,
        AfterMathOps,
        AssignOps,
        AfterAssignOps
    }
}
