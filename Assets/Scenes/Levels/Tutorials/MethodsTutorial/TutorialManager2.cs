using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeEditorComponents;

public class TutorialManager2 : TutorialBase, IActivate{
    public Tutorial2Objectives currentObjective;

    void Start(){
        switch(currentObjective){
            case Tutorial2Objectives.Start:
                //sets editor comments
                SetEditorComments(0);

                objectiveText[0].text = ">Press the button";
                hintButton.convoToLoad = hints[0];
                StartCoroutine(convoManager.StartDialogue(0, 0.5f));
            break;
            case Tutorial2Objectives.MethodsPlay:
                StartMethodsPlay();
            break;
            case Tutorial2Objectives.ParamsIntro:
                StartParamsIntro();
            break;
            case Tutorial2Objectives.TurnInteract:
                StartTurnInteract();
            break;
            case Tutorial2Objectives.TurnInteract2:
                StartTurnInteract2();
            break;
        }
    }

    //stage 1: methods - what methods are; 2x moveUp
    //stage 3: parameters - enforce line limit
    //stage 3: turn and interact methods; get the rock to the button
    //stage 4: DIY level
    public void activate() {
        switch(currentObjective){
            case Tutorial2Objectives.MethodsIntro:
                objectiveText[0].color = successColor;
                currentObjective = Tutorial2Objectives.AfterMethodsIntro;
                StartCoroutine(convoManager.StartDialogue(1, dialogueInvokeTime));
            break;
            case Tutorial2Objectives.MethodsPlay:
                objectiveText[0].color = successColor;
                currentObjective = Tutorial2Objectives.AfterMethodsPlay;
                StartCoroutine(convoManager.StartDialogue(3, dialogueInvokeTime));
            break;
            case Tutorial2Objectives.ParamsIntro:
                objectiveText[0].color = successColor;

                //checks line count
                if(Compiler.Instance.linesCount <= 5){
                    objectiveText[1].color = successColor;
                    currentObjective = Tutorial2Objectives.AfterParamsIntro;
                    StartCoroutine(convoManager.StartDialogue(5, dialogueInvokeTime));
                } else {
                    objectiveText[1].color = Color.red;
                    StartCoroutine(convoManager.StartFailDialogue(0, dialogueInvokeTime));
                }
            break;
            case Tutorial2Objectives.TurnInteract:
                objectiveText[0].color = successColor;
                currentObjective = Tutorial2Objectives.AfterTurnInteract1;
                StartCoroutine(convoManager.StartDialogue(7, dialogueInvokeTime));
            break;
            case Tutorial2Objectives.TurnInteract2:
                objectiveText[0].color = successColor;
                currentObjective = Tutorial2Objectives.AfterTurnInteract2;
                StartCoroutine(convoManager.StartDialogue(9, dialogueInvokeTime));
            break;
        }
    }
    
    [SerializeField] private int timesPressed = 0;
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
            case Tutorial2Objectives.Start:
                if(timesPressed == convoManager.convos[0].LineCount()){
                    currentObjective = Tutorial2Objectives.MethodsIntro;
                    timesPressed = 0;
                }
            break;
            case Tutorial2Objectives.AfterMethodsIntro:
                if(timesPressed == convoManager.convos[1].LineCount()){
                    StartMethodsPlay();
                    timesPressed = 0;
                }
            break;
            case Tutorial2Objectives.AfterMethodsPlay:
                if(timesPressed == convoManager.convos[3].LineCount()){
                    StartParamsIntro();
                    timesPressed = 0;
                }
            break;
            case Tutorial2Objectives.AfterParamsIntro:
                if(timesPressed == convoManager.convos[5].LineCount()){
                    StartTurnInteract();
                    timesPressed = 0;
                }
            break;
            case Tutorial2Objectives.AfterTurnInteract1:
                if(timesPressed == convoManager.convos[7].LineCount()){
                    StartTurnInteract2();
                    timesPressed = 0;
                }
            break;
            case Tutorial2Objectives.AfterTurnInteract2:
                if(timesPressed == convoManager.convos[9].LineCount()){
                    GameObject.Find("exit-point").GetComponent<IActivate>().activate();
                    timesPressed = 0;
                }
            break;
            default:
                timesPressed = 0;
            break;
        }
    }

    void StartMethodsPlay(){
        StartStage(1);

        //set enum mode
        currentObjective = Tutorial2Objectives.MethodsPlay;

        //set objective text
        objectiveText[0].text = ">Find the real buttons";
        objectiveText[0].color = defaultColor;
    }

    private void StartParamsIntro(){
        StartStage(2);

        //set enum mode
        currentObjective = Tutorial2Objectives.MethodsPlay;
        
        //set objective text
        objectiveText[0].text = ">Press the button";
        objectiveText[0].color = Color.white;

        objectiveText[1].transform.gameObject.SetActive(true);
        objectiveText[1].text = ">Use 5 lines or less";
        objectiveText[0].color = Color.white;
    }

    private void StartTurnInteract(){
        StartStage(3);

        //change enum mode to TurnInteract
        currentObjective = Tutorial2Objectives.TurnInteract;
        
        //set objective text
        objectiveText[0].text = ">Press the button";
        objectiveText[0].color = Color.white;
        objectiveText[1].transform.gameObject.SetActive(false);
    }

    private void StartTurnInteract2(){
        StartStage(4);

        //set enum mode
        currentObjective = Tutorial2Objectives.MethodsPlay;

        //set objective text
        objectiveText[0].text = ">Press the button";
        objectiveText[0].color = Color.white;
    }

    /*private void StartFinalStretch(){
        //sets editor comments
        SetEditorComments(5);

        //moves camera and player
        MoveCamera(new Vector3(10175f, 0f, 310f));
        var playerChara = GameObject.Find("PlayerCharacter");
        var tpLocs = playerChara.GetComponent<MoveNugget>().tpLocs;
        playerChara.transform.position = tpLocs[5].transform.position;

        //starts conversation for TurnInteract2
        StartCoroutine(convoManager.StartDialogue(10, dialogueInvokeTime));

        //change enum mode to FinalStretch
        currentObjective = Tutorial2Objectives.FinalStretch;

        //set objective text
        objectiveText[0].text = ">Touch the exit totem";
        objectiveText[0].color = Color.white;

        hintButton.convoToLoad = hints[5];
    }*/

    public void deactivate() {
        //do nothing
    }
}

public enum Tutorial2Objectives{
    Start,
    MethodsIntro,
    AfterMethodsIntro,
    MethodsPlay,
    AfterMethodsPlay,
    ParamsIntro,
    AfterParamsIntro,
    TurnInteract,
    AfterTurnInteract1,
    TurnInteract2,
    AfterTurnInteract2,
}