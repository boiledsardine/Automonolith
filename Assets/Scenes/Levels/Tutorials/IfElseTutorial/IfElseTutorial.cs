using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IfElseTutorial : TutorialBase, IActivate{
    public Objectives currentObjective;
    void Start(){
        Time.timeScale = 5;
        switch(currentObjective){
            case Objectives.Start:
                StartIf();
            break;
            case Objectives.Else:
                StartElse();
            break;
            case Objectives.ElseIf:
                StartElseIf();
            break;
            case Objectives.CheckCube:
                StartCheckCube();
            break;
            case Objectives.LoopIf:
                StartLoopIf();
            break;
        }
    }

    public void activate(){
        var ci = Compiler.Instance;
        switch(currentObjective){
            case Objectives.If:
                //check for an if statement
                bool ifFound = false;
                for(int i = 0; i < Compiler.Instance.getCodeLines.Length; i++){
                    if(ci.conditionByIndex.ContainsKey(i) && ci.conditionByIndex[i].type == ConditionBlocks.Type.If){
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
                for(int i = 0; i < Compiler.Instance.getCodeLines.Length; i++){
                    if(ci.conditionByIndex.ContainsKey(i) && ci.conditionByIndex[i].type == ConditionBlocks.Type.Else){
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
            case Objectives.ElseIf:
                //check for an else-if statmenet                
                bool elseIfFound = false;
                for(int i = 0; i < Compiler.Instance.getCodeLines.Length; i++){
                    if(ci.conditionByIndex.ContainsKey(i) && ci.conditionByIndex[i].type == ConditionBlocks.Type.ElseIf){
                        elseIfFound = true;
                    }
                }
                if(elseIfFound){
                    objectiveText[0].color = successColor;
                    objectiveText[1].color = successColor;
                    currentObjective = Objectives.AfterElseIf;
                    StartCoroutine(convoManager.StartDialogue(5, dialogueInvokeTime));
                    Compiler.Instance.terminateExecution();
                } else {
                    objectiveText[0].color = successColor;
                    objectiveText[1].color = Color.red;
                    StartCoroutine(convoManager.StartFailDialogue(2, dialogueInvokeTime));
                    Compiler.Instance.terminateExecution();
                }
            break;
            case Objectives.CheckCube:
                objectiveText[0].color = successColor;
                currentObjective = Objectives.AfterCheckCube;
                StartCoroutine(convoManager.StartDialogue(7, dialogueInvokeTime));
                Compiler.Instance.terminateExecution();
            break;
            case Objectives.LoopIf:
                objectiveText[0].color = successColor;
                currentObjective = Objectives.AfterLoopIf;
                StartCoroutine(convoManager.StartDialogue(9, dialogueInvokeTime));
                Compiler.Instance.terminateExecution();
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
                    StartElseIf();
                    timesPressed = 0;
                }
            break;
            case Objectives.AfterElseIf:
                if(timesPressed == convoManager.convos[5].LineCount()){
                    StartCheckCube();
                    timesPressed = 0;
                }
            break;
            case Objectives.AfterCheckCube:
                if(timesPressed == convoManager.convos[7].LineCount()){
                    StartLoopIf();
                    timesPressed = 0;
                }
            break;
            case Objectives.AfterLoopIf:
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
        objectiveText[0].color = Color.black;

        objectiveText[1].transform.gameObject.SetActive(true);
        objectiveText[1].text = ">Use an if statement";
        objectiveText[1].color = Color.black;
    }

    void StartElse(){
        StartStage(1);

        //set enum mode
        currentObjective = Objectives.Else;

        //set objective text
        objectiveText[0].text = ">Reach the button";
        objectiveText[0].color = Color.black;

        objectiveText[1].transform.gameObject.SetActive(true);
        objectiveText[1].text = ">Use an else statement";
        objectiveText[1].color = Color.black;
    }

    void StartElseIf(){
        StartStage(2);

        //set enum mode
        currentObjective = Objectives.ElseIf;

        //set objective text
        objectiveText[0].text = ">Reach the button";
        objectiveText[0].color = Color.black;

        objectiveText[1].transform.gameObject.SetActive(true);
        objectiveText[1].text = ">Use an else if statement";
        objectiveText[1].color = Color.black;
    }

    void StartCheckCube(){
        StartStage(3);

        //set enum mode
        currentObjective = Objectives.CheckCube;

        //set objective text
        objectiveText[1].transform.gameObject.SetActive(false);
        objectiveText[0].text = ">Put the cube in the proper acceptor";
        objectiveText[0].color = Color.black;
    }

    void StartLoopIf(){
        StartStage(4);

        //set enum mode
        currentObjective = Objectives.LoopIf;

        //set objective text
        objectiveText[0].text = ">Put the cubes in the proper acceptors";
        objectiveText[0].color = Color.black;
    }

    public void deactivate(){}

    public enum Objectives{
        Start,
        If,
        AfterIf,
        Else,
        AfterElse,
        ElseIf,
        AfterElseIf,
        CheckCube,
        AfterCheckCube,
        LoopIf,
        AfterLoopIf
    }
}