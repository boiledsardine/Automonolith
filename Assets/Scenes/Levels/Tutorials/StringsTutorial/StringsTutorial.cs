using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringsTutorial : TutorialBase, IActivate{
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

    public void activate(){
        var ci = Compiler.Instance;
        switch(currentObjective){
            case StringsObjectives.StartString:
                objectiveText[0].color = successColor;
                currentObjective = StringsObjectives.AfterString;
                StartCoroutine(convoManager.StartDialogue(1, dialogueInvokeTime));
            break;
            case StringsObjectives.StringDeclare:
                //check compiler dictionaries
                if(ci.allVars.Count >= ci.reservedVars && ci.strVars.Count >= ci.reservedStrings){
                    objectiveText[0].color = successColor;
                    objectiveText[1].color = successColor;
                    currentObjective = StringsObjectives.AfterStringDeclare;
                    StartCoroutine(convoManager.StartDialogue(3, dialogueInvokeTime));
                } else {
                    objectiveText[0].color = successColor;
                    objectiveText[1].color = Color.red;
                    StartCoroutine(convoManager.StartFailDialogue(0, dialogueInvokeTime));
                }
            break;
            case StringsObjectives.MoveTo:
                if(ci.linesCount <= 3){
                    objectiveText[0].color = successColor;
                    objectiveText[1].color = successColor;
                    currentObjective = StringsObjectives.AfterMoveTo;
                    StartCoroutine(convoManager.StartDialogue(5, dialogueInvokeTime));
                } else {
                    objectiveText[0].color = successColor;
                    objectiveText[1].color = Color.red;
                    StartCoroutine(convoManager.StartFailDialogue(1, dialogueInvokeTime));
                }
            break;
        }
    }

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

    void StartString(){
        StartStage(0);

        //set enum mode
        currentObjective = StringsObjectives.StartString;

        //set objective text
        objectiveText[0].text = ">Reach the button";
        objectiveText[0].color = Color.black;
    }

    void StartDeclare(){
        StartStage(1);

        //set enum mode
        currentObjective = StringsObjectives.StringDeclare;

        //set objective text
        objectiveText[0].text = ">Reach the button";
        objectiveText[0].color = Color.black;

        objectiveText[1].transform.gameObject.SetActive(true);
        objectiveText[1].text = ">Use a string variable";
        objectiveText[1].color = Color.black;
    }

    void StartMoveTo(){
        StartStage(2);

        //set enum mode
        currentObjective = StringsObjectives.MoveTo;

        //set objective text
        objectiveText[0].text = ">Reach the button";
        objectiveText[0].color = Color.black;

        objectiveText[1].transform.gameObject.SetActive(true);
        objectiveText[1].text = ">Use 3 or less statements";
        objectiveText[1].color = Color.black;
    }

    public void deactivate(){}

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