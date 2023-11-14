using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeEditorComponents;

public class BoolLoopTutorial : TutorialBase{
    public BoolLoopObjectives currentObjective;

    void Start(){
        switch(currentObjective){
            case BoolLoopObjectives.Start:
                StartTrueFalse();
            break;
            case BoolLoopObjectives.CompareOps:
                StartCompareOps();
            break;
            case BoolLoopObjectives.BooleanOps:
                StartBooleanOps();
            break;
            default: break;
        }
    }

    public override IEnumerator ActivateButton(){
        hasError = false;
        yield return new WaitForSeconds(1);
        if(hasError){
            hasError = false;
            yield break;
        }

        switch(currentObjective){            
            case BoolLoopObjectives.TrueFalse:
                //check dictionaries
                var ci = Compiler.Instance;
                if(ci.allVars.Count >= ci.reservedVars + 2 && ci.boolVars.Count >= ci.reservedBools + 2){
                    objectiveText[0].color = successColor;
                    objectiveText[1].color = successColor;
                    currentObjective = BoolLoopObjectives.AfterTrueFalse;
                    StartCoroutine(convoManager.StartDialogue(1, 0));
                } else {
                    objectiveText[0].color = successColor;
                    objectiveText[1].color = Color.red;
                    StartCoroutine(convoManager.StartFailDialogue(0, 0));
                }
            break;
            case BoolLoopObjectives.CompareOps:
                objectiveText[0].color = successColor;
                currentObjective = BoolLoopObjectives.AfterCompareOps;
                StartCoroutine(convoManager.StartDialogue(3, 0));
            break;
            case BoolLoopObjectives.BooleanOps:
                objectiveText[0].color = successColor;
                currentObjective = BoolLoopObjectives.AfterBooleanOps;
                StartCoroutine(convoManager.StartDialogue(5, 0));
            break;
            default: break;
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
                StartCompareOps();
            break;
            case 4:
                StartBooleanOps();
            break;
            case 6:
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
            case BoolLoopObjectives.AfterTrueFalse:
                if(timesPressed == convoManager.convos[1].LineCount()){
                    StartCompareOps();
                    timesPressed = 0;
                }
            break;
            case BoolLoopObjectives.AfterCompareOps:
                if(timesPressed == convoManager.convos[3].LineCount()){
                    StartBooleanOps();
                    timesPressed = 0;
                }
            break;
            case BoolLoopObjectives.AfterBooleanOps:
                if(timesPressed == convoManager.convos[5].LineCount()){
                    GameObject.Find("exit-point").GetComponent<IActivate>().activate();
                    timesPressed = 0;
                }
            break;

            default:
                timesPressed = 0;
            break;
        }
    }
    */

    void StartTrueFalse(){
        Debug.Log("Starting TrueFalse");

        StartStage(0);

        //set enum mode
        currentObjective = BoolLoopObjectives.TrueFalse;

        //set objective text
        objectiveText[0].text = ">Step on the button";
        objectiveText[0].color = defaultColor;

        objectiveText[1].transform.gameObject.SetActive(true);
        objectiveText[1].text = ">Declare two bool variables";
        objectiveText[1].color = defaultColor;
    }

    void StartCompareOps(){
        Debug.Log("Starting CompareOps");

        StartStage(1);

        //set enum mode
        currentObjective = BoolLoopObjectives.CompareOps;

        //set objective text
        objectiveText[0].text = ">Step on the button";
        objectiveText[0].color = defaultColor;

        objectiveText[1].transform.gameObject.SetActive(false);
    }

    void StartBooleanOps(){
        Debug.Log("Starting BoolOps");

        StartStage(2);

        //set enum mode
        currentObjective = BoolLoopObjectives.BooleanOps;

        //set objective text
        objectiveText[0].text = ">Step on the button";
        objectiveText[0].color = defaultColor;
    }

    public enum BoolLoopObjectives{
        Start,
        TrueFalse,
        AfterTrueFalse,
        CompareOps,
        AfterCompareOps,
        BooleanOps,
        AfterBooleanOps,
    }
}