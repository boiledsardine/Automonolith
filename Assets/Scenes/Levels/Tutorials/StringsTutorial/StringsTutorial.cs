using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringsTutorial : TutorialBase{
    public StringsObjectives currentObjective;
    void Start(){
        switch(currentObjective){
            case StringsObjectives.Start:
            goto case StringsObjectives.StartString;
            case StringsObjectives.StartString:
                StartString();
            break;
            case StringsObjectives.StringDeclare:
                StartDeclare();
            break;
            case StringsObjectives.MoveTo:
                StartMoveTo();
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
            case StringsObjectives.StartString:
                objectiveText[0].color = successColor;
                currentObjective = StringsObjectives.AfterString;
                StartCoroutine(convoManager.StartDialogue(1, 0));
            break;
            case StringsObjectives.StringDeclare:
                //check compiler dictionaries
                if(ci.allVars.Count >= ci.reservedVars && ci.strVars.Count >= ci.reservedStrings){
                    objectiveText[0].color = successColor;
                    objectiveText[1].color = successColor;
                    currentObjective = StringsObjectives.AfterStringDeclare;
                    StartCoroutine(convoManager.StartDialogue(3, 0));
                } else {
                    objectiveText[0].color = successColor;
                    objectiveText[1].color = Color.red;
                    StartCoroutine(convoManager.StartFailDialogue(0, 0));
                }
            break;
            case StringsObjectives.MoveTo:
                if(ci.linesCount <= 3){
                    objectiveText[0].color = successColor;
                    objectiveText[1].color = successColor;
                    currentObjective = StringsObjectives.AfterMoveTo;
                    StartCoroutine(convoManager.StartDialogue(5, 0));
                } else {
                    objectiveText[0].color = successColor;
                    objectiveText[1].color = Color.red;
                    StartCoroutine(convoManager.StartFailDialogue(1, 0));
                }
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
                StartDeclare();
            break;
            case 4:
                StartMoveTo();
            break;
            case 6:
                GameObject.Find("exit-point").GetComponent<IActivate>().activate();
            break;
            default: break;
        }
    }

    /*
    int timesPressed = 0;
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
            case StringsObjectives.AfterString:
                if(timesPressed == convoManager.convos[1].LineCount()){
                    StartDeclare();
                    timesPressed = 0;
                }
            break;
            case StringsObjectives.AfterStringDeclare:
                if(timesPressed == convoManager.convos[3].LineCount()){
                    StartMoveTo();
                    timesPressed = 0;
                }
            break;
            case StringsObjectives.AfterMoveTo:
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

    void StartString(){
        StartStage(0);

        //set enum mode
        currentObjective = StringsObjectives.StartString;

        //set objective text
        objectiveText[0].text = ">Step on the button";
        objectiveText[0].color = defaultColor;
    }

    void StartDeclare(){
        StartStage(1);

        //set enum mode
        currentObjective = StringsObjectives.StringDeclare;

        //set objective text
        objectiveText[0].text = ">Step on the button";
        objectiveText[0].color = defaultColor;

        objectiveText[1].transform.gameObject.SetActive(true);
        objectiveText[1].text = ">Use a string variable";
        objectiveText[1].color = defaultColor;
    }

    void StartMoveTo(){
        StartStage(2);

        //set enum mode
        currentObjective = StringsObjectives.MoveTo;

        //set objective text
        objectiveText[0].text = ">Step on the button";
        objectiveText[0].color = defaultColor;

        objectiveText[1].transform.gameObject.SetActive(true);
        objectiveText[1].text = ">Use 3 or less statements";
        objectiveText[1].color = defaultColor;
    }

    public enum StringsObjectives{
        Start,
        StartString,
        AfterString,
        StringDeclare,
        AfterStringDeclare,
        MoveTo,
        AfterMoveTo
    }
}