using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IfElseTutorial : TutorialBase, IActivate{
    public Objectives currentObjective;
    void Start(){
        switch(currentObjective){
            case Objectives.Start:
                StartIf();
            break;
            case Objectives.Else:
                StartElse();
            break;
            case Objectives.PlayElse:
                StartPlayElse();
            break;
            case Objectives.ElseIf:
                StartElseIf();
            break;
            case Objectives.PlayElseIf:
                StartPlayElseIf();
            break;
        }
    }

    public void activate(){
        var ci = Compiler.Instance;
        switch(currentObjective){
            case Objectives.If:
                //check for an if statement
                bool ifFound = false;
                foreach(KeyValuePair<int,ConditionBlocks> kv in ci.conditionByIndex){
                    if(kv.Value.type == ConditionBlocks.Type.If){
                        ifFound = true;
                    }
                }
                if(ifFound){
                    objectiveText[0].color = successColor;
                    objectiveText[1].color = successColor;
                    currentObjective = Objectives.AfterIf;
                    StartCoroutine(convoManager.StartDialogue(1, dialogueInvokeTime));
                    Compiler.Instance.terminateExecution();
                } else {
                    objectiveText[0].color = successColor;
                    objectiveText[1].color = Color.red;
                    StartCoroutine(convoManager.StartFailDialogue(0, dialogueInvokeTime));
                    Compiler.Instance.terminateExecution();
                }
            break;
            case Objectives.Else:
                //check for an else statement
                bool elseFound = false;
                foreach(KeyValuePair<int,ConditionBlocks> kv in ci.conditionByIndex){
                    if(kv.Value.type == ConditionBlocks.Type.Else){
                        elseFound = true;
                    }
                }
                if(elseFound){
                    objectiveText[0].color = successColor;
                    objectiveText[1].color = successColor;
                    currentObjective = Objectives.AfterElse;
                    StartCoroutine(convoManager.StartDialogue(3, dialogueInvokeTime));
                    Compiler.Instance.terminateExecution();
                } else {
                    objectiveText[0].color = successColor;
                    objectiveText[1].color = Color.red;
                    StartCoroutine(convoManager.StartFailDialogue(1, dialogueInvokeTime));
                    Compiler.Instance.terminateExecution();
                }
            break;
            case Objectives.PlayElse:
                //check for an else statement
                bool elseFound2 = false;
                foreach(KeyValuePair<int,ConditionBlocks> kv in ci.conditionByIndex){
                    if(kv.Value.type == ConditionBlocks.Type.Else){
                        elseFound2 = true;
                    }
                }
                if(elseFound2){
                    objectiveText[0].color = successColor;
                    objectiveText[1].color = successColor;
                    currentObjective = Objectives.AfterPlayElse;
                    StartCoroutine(convoManager.StartDialogue(5, dialogueInvokeTime));
                    Compiler.Instance.terminateExecution();
                } else {
                    objectiveText[0].color = successColor;
                    objectiveText[1].color = Color.red;
                    StartCoroutine(convoManager.StartFailDialogue(1, dialogueInvokeTime));
                    Compiler.Instance.terminateExecution();
                }
            break;
            case Objectives.ElseIf:
                //check for an else-if statmenet                
                bool elseIfFound = false;
                foreach(KeyValuePair<int,ConditionBlocks> kv in ci.conditionByIndex){
                    if(kv.Value.type == ConditionBlocks.Type.ElseIf){
                        elseIfFound = true;
                    }
                }
                if(elseIfFound){
                    objectiveText[0].color = successColor;
                    objectiveText[1].color = successColor;
                    currentObjective = Objectives.AfterElseIf;
                    StartCoroutine(convoManager.StartDialogue(7, dialogueInvokeTime));
                    Compiler.Instance.terminateExecution();
                } else {
                    objectiveText[0].color = successColor;
                    objectiveText[1].color = Color.red;
                    StartCoroutine(convoManager.StartFailDialogue(2, dialogueInvokeTime));
                    Compiler.Instance.terminateExecution();
                }
            break;
            case Objectives.PlayElseIf:
                //check for an else-if statmenet                
                bool elseIfFound2 = false;
                foreach(KeyValuePair<int,ConditionBlocks> kv in ci.conditionByIndex){
                    if(kv.Value.type == ConditionBlocks.Type.ElseIf){
                        elseIfFound2 = true;
                    }
                }
                if(elseIfFound2){
                    objectiveText[0].color = successColor;
                    objectiveText[1].color = successColor;
                    currentObjective = Objectives.AfterPlayElseIf;
                    StartCoroutine(convoManager.StartDialogue(9, dialogueInvokeTime));
                    Compiler.Instance.terminateExecution();
                } else {
                    objectiveText[0].color = successColor;
                    objectiveText[1].color = Color.red;
                    StartCoroutine(convoManager.StartFailDialogue(2, dialogueInvokeTime));
                    Compiler.Instance.terminateExecution();
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
            case Objectives.AfterIf:
                if(timesPressed == convoManager.convos[1].LineCount()){
                    StartElse();
                    timesPressed = 0;
                }
            break;
            case Objectives.AfterElse:
                if(timesPressed == convoManager.convos[3].LineCount()){
                    StartPlayElse();
                    timesPressed = 0;
                }
            break;
            case Objectives.AfterPlayElse:
                if(timesPressed == convoManager.convos[5].LineCount()){
                    StartElseIf();
                    timesPressed = 0;
                }
            break;
            case Objectives.AfterElseIf:
                if(timesPressed == convoManager.convos[7].LineCount()){
                    StartPlayElseIf();
                    timesPressed = 0;
                }
            break;
            case Objectives.AfterPlayElseIf:
                if(timesPressed == convoManager.convos[9].LineCount()){
                    timesPressed = 0;
                    GameObject.Find("exit-point").GetComponent<IActivate>().activate();
                }
            break;
            default:
                timesPressed = 0;
            break;
        }
    }

    void StartIf(){
        StartStage(0);

        //set enum mode
        currentObjective = Objectives.If;

        //set objective text
        objectiveText[0].text = ">Reach the button";
        objectiveText[0].color = defaultColor;

        objectiveText[1].transform.gameObject.SetActive(true);
        objectiveText[1].text = ">Use an if statement";
        objectiveText[1].color = defaultColor;
    }

    void StartElse(){
        StartStage(1);

        //set enum mode
        currentObjective = Objectives.Else;

        //set objective text
        objectiveText[0].text = ">Reach the button";
        objectiveText[0].color = defaultColor;

        objectiveText[1].transform.gameObject.SetActive(true);
        objectiveText[1].text = ">Use an else statement";
        objectiveText[1].color = defaultColor;
    }

    void StartPlayElse(){
        StartStage(2);

        //set enum mode
        currentObjective = Objectives.PlayElse;

        //set objective text
        objectiveText[0].text = ">Reach the button";
        objectiveText[0].color = defaultColor;

        objectiveText[1].transform.gameObject.SetActive(true);
        objectiveText[1].text = ">Use an else statement";
        objectiveText[1].color = defaultColor;
    }

    void StartElseIf(){
        StartStage(3);

        //set enum mode
        currentObjective = Objectives.ElseIf;

        //set objective text
        objectiveText[0].text = ">Reach the button";
        objectiveText[0].color = defaultColor;

        objectiveText[1].transform.gameObject.SetActive(true);
        objectiveText[1].text = ">Use an else if statement";
        objectiveText[1].color = defaultColor;
    }

    void StartPlayElseIf(){
        StartStage(4);

        //set enum mode
        currentObjective = Objectives.PlayElseIf;

        //set objective text
        objectiveText[0].text = ">Reach the button";
        objectiveText[0].color = defaultColor;

        objectiveText[1].transform.gameObject.SetActive(true);
        objectiveText[1].text = ">Use an else if statement";
        objectiveText[1].color = defaultColor;
    }

    public void deactivate(){}

    public enum Objectives{
        Start,
        If,
        AfterIf,
        Else,
        AfterElse,
        PlayElse,
        AfterPlayElse,
        ElseIf,
        AfterElseIf,
        PlayElseIf,
        AfterPlayElseIf
    }
}