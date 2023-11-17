using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeEditorComponents;

public class TutorialManager2 : TutorialBase{
    public Tutorial2Objectives currentObjective;

    void Start(){
        switch(currentObjective){
            case Tutorial2Objectives.Start:
                StartMethods();
            break;
            case Tutorial2Objectives.MethodsPlay:
                StartMethodsPlay();
            break;
            case Tutorial2Objectives.ParamsIntro:
                StartParamsIntro();
            break;
            /*
            case Tutorial2Objectives.TurnInteract:
                StartTurnInteract();
            break;
            */
            case Tutorial2Objectives.TurnInteract2:
                StartTurnInteract2();
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

        switch(currentObjective){
            case Tutorial2Objectives.MethodsIntro:
                objectiveText[0].color = successColor;
                currentObjective = Tutorial2Objectives.AfterMethodsIntro;
                StartCoroutine(convoManager.StartDialogue(1, 0));
            break;
            case Tutorial2Objectives.MethodsPlay:
                objectiveText[0].color = successColor;
                currentObjective = Tutorial2Objectives.AfterMethodsPlay;
                StartCoroutine(convoManager.StartDialogue(3, 0));
            break;
            case Tutorial2Objectives.ParamsIntro:
                objectiveText[0].color = successColor;

                //checks line count
                if(Compiler.Instance.linesCount <= 5){
                    objectiveText[1].color = successColor;
                    currentObjective = Tutorial2Objectives.AfterParamsIntro;
                    StartCoroutine(convoManager.StartDialogue(5, 0));
                } else {
                    objectiveText[1].color = Color.red;
                    StartCoroutine(convoManager.StartFailDialogue(0, 0));
                }
            break;
            /*
            case Tutorial2Objectives.TurnInteract:
                objectiveText[0].color = successColor;
                currentObjective = Tutorial2Objectives.AfterTurnInteract1;
                StartCoroutine(convoManager.StartDialogue(7, dialogueInvokeTime));
            break;
            */
            case Tutorial2Objectives.TurnInteract2:
                objectiveText[0].color = successColor;
                objectiveText[1].color = successColor;
                currentObjective = Tutorial2Objectives.AfterTurnInteract2;
                StartCoroutine(convoManager.StartDialogue(9, 0));
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
                StartMethodsPlay();
            break;
            case 4:
                StartParamsIntro();
            break;
            case 6:
                StartTurnInteract2();
            break;
            case 8:
                GameObject.Find("exit-point").GetComponent<IActivate>().activate();
            break;
            default: break;
        }
    }

    /*
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
    */

    void StartMethods(){
        StartStage(0);

        //set enum mode
        currentObjective = Tutorial2Objectives.MethodsIntro;

        //set objective text
        objectiveText[0].text = ">Move to the white button";
        objectiveText[0].color = defaultColor;
    }

    void StartMethodsPlay(){
        StartStage(1);

        //set enum mode
        currentObjective = Tutorial2Objectives.MethodsPlay;

        //set objective text
        objectiveText[0].text = ">Move to the white button";
        objectiveText[0].color = defaultColor;
    }

    private void StartParamsIntro(){
        StartStage(2);

        //set enum mode
        currentObjective = Tutorial2Objectives.ParamsIntro;
        
        //set objective text
        objectiveText[0].text = ">Move to the white button";
        objectiveText[0].color = Color.white;

        objectiveText[1].transform.gameObject.SetActive(true);
        objectiveText[1].text = ">Use 5 lines or less";
        objectiveText[0].color = Color.white;
    }

    private void StartTurnInteract(){
        //Immediately switch to Stage 4
        StartTurnInteract2();


        //CODE NOW OBSOLETE!
        /*StartStage(3);

        //change enum mode to TurnInteract
        currentObjective = Tutorial2Objectives.TurnInteract;
        
        //set objective text
        objectiveText[1].text = ">Press the wall button";
        objectiveText[1].color = Color.white;

        objectiveText[0].text = ">Press the floor button";
        objectiveText[0].color = Color.white;*/
    }

    private void StartTurnInteract2(){
        StartStage(4);

        //set enum mode
        currentObjective = Tutorial2Objectives.TurnInteract2;
        
        //set objective text
        objectiveText[0].text = ">Move pillar to brown button";
        objectiveText[0].color = Color.white;

        objectiveText[1].text = ">Press the button";
        objectiveText[1].color = Color.white;
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
}

public enum Tutorial2Objectives{
    Start,
    MethodsIntro,
    AfterMethodsIntro,
    MethodsPlay,
    AfterMethodsPlay,
    ParamsIntro,
    AfterParamsIntro,
    //TurnInteract,
    //AfterTurnInteract1,
    TurnInteract2,
    AfterTurnInteract2,
}