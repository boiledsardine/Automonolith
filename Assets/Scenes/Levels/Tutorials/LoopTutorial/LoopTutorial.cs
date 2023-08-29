using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopTutorial : TutorialBase, IActivate{
    public LoopObjectives currentObjective;
    void Start(){
        //Time.timeScale = 3;
        gameObject.GetComponent<Pathfinder>().enabled = false;
        switch(currentObjective){
            case LoopObjectives.Start:
                StartInfLoop1();
            break;
            case LoopObjectives.InfLoop2:
                StartInfLoop2();
            break;
            case LoopObjectives.FixedLoop:
                StartFixedLoop();
            break;
            case LoopObjectives.Acceptor1:
                StartAcceptor1();
            break;
            case LoopObjectives.Acceptor2:
                StartAcceptor2();
            break;
        }
    }

    int activatePresses = 0;
    void IActivate.activate(){
        switch(currentObjective){
            case LoopObjectives.InfLoop1:
                activatePresses++;
                if(activatePresses < 3){
                    break;
                }

                //check line count
                if(Compiler.Instance.linesCount <= 8){
                    objectiveText[0].color = successColor;
                    objectiveText[1].color = successColor;
                    currentObjective = LoopObjectives.AfterInfLoop1;
                    StartCoroutine(convoManager.StartDialogue(1, dialogueInvokeTime));
                    Compiler.Instance.terminateExecution();
                } else {
                    objectiveText[0].color = successColor;
                    objectiveText[1].color = Color.red;
                    StartCoroutine(convoManager.StartFailDialogue(0, dialogueInvokeTime));
                }
            break;
            case LoopObjectives.InfLoop2:
                //check line count
                if(Compiler.Instance.linesCount <= 5){
                    objectiveText[0].color = successColor;
                    objectiveText[1].color = successColor;
                    currentObjective = LoopObjectives.AfterInfLoop2;
                    StartCoroutine(convoManager.StartDialogue(3, dialogueInvokeTime));
                    Compiler.Instance.terminateExecution();
                } else {
                    objectiveText[0].color = successColor;
                    objectiveText[1].color = Color.red;
                    StartCoroutine(convoManager.StartFailDialogue(0, dialogueInvokeTime));
                }
            break;
            case LoopObjectives.FixedLoop:
                //check line count
                successTime = true;
                Invoke("CheckTime", 5f);
            break;
            case LoopObjectives.Acceptor1:
                //check line count
                if(Compiler.Instance.linesCount <= 10){
                    objectiveText[0].color = successColor;
                    objectiveText[1].color = successColor;
                    currentObjective = LoopObjectives.AfterAcceptor;
                    StartCoroutine(convoManager.StartDialogue(7, dialogueInvokeTime));
                    Compiler.Instance.terminateExecution();
                } else {
                    objectiveText[0].color = successColor;
                    objectiveText[1].color = Color.red;
                    StartCoroutine(convoManager.StartFailDialogue(0, dialogueInvokeTime));
                }
            break;
            case LoopObjectives.Acceptor2:
                //check line count
                if(Compiler.Instance.linesCount <= 15){
                    objectiveText[0].color = successColor;
                    objectiveText[1].color = successColor;
                    currentObjective = LoopObjectives.AfterAcceptor2;
                    StartCoroutine(convoManager.StartDialogue(9, dialogueInvokeTime));
                } else {
                    objectiveText[0].color = successColor;
                    objectiveText[1].color = Color.red;
                    StartCoroutine(convoManager.StartFailDialogue(0, dialogueInvokeTime));
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
            case LoopObjectives.AfterInfLoop1:
                if(timesPressed == convoManager.convos[1].LineCount()){
                    StartInfLoop2();
                    timesPressed = 0;
                }
            break;
            case LoopObjectives.AfterInfLoop2:
                if(timesPressed == convoManager.convos[3].LineCount()){
                    StartFixedLoop();
                    timesPressed = 0;
                }
            break;
            case LoopObjectives.AfterFixedLoop:
                if(timesPressed == convoManager.convos[5].LineCount()){
                    StartAcceptor1();
                    timesPressed = 0;
                }
            break;
            case LoopObjectives.AfterAcceptor:
                if(timesPressed == convoManager.convos[7].LineCount()){
                    StartAcceptor2();
                    timesPressed = 0;
                }
            break;
            case LoopObjectives.AfterAcceptor2:
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

    void StartInfLoop1(){
        StartStage(0);

        //set enum mode
        currentObjective = LoopObjectives.InfLoop1;

        //set objective text
        objectiveText[0].text = ">Press the button repeatedly";
        objectiveText[0].color = Color.black;

        objectiveText[1].transform.gameObject.SetActive(true);
        objectiveText[1].text = ">Use 8 lines or less";
        objectiveText[1].color = Color.black;
    }

    void StartInfLoop2(){
        StartStage(1);

        //set enum mode
        currentObjective = LoopObjectives.InfLoop2;

        //set objective text
        objectiveText[0].text = ">Press the button repeatedly";
        objectiveText[0].color = Color.black;

        objectiveText[1].transform.gameObject.SetActive(true);
        objectiveText[1].text = ">Use 5 lines or less";
        objectiveText[1].color = Color.black;
    }

    void StartFixedLoop(){
        StartStage(2);

        //set enum mode
        currentObjective = LoopObjectives.FixedLoop;

        //set objective text
        objectiveText[0].text = ">Stand on the button for a while";
        objectiveText[0].color = Color.black;

        objectiveText[1].transform.gameObject.SetActive(true);
        objectiveText[1].text = ">Use 10 lines or less";
        objectiveText[1].color = Color.black;
    }

    void StartAcceptor1(){
        StartStage(3);

        //set enum mode
        currentObjective = LoopObjectives.Acceptor1;

        //set objective text
        objectiveText[0].text = ">Fill the Acceptor";
        objectiveText[0].color = Color.black;

        objectiveText[1].transform.gameObject.SetActive(true);
        objectiveText[1].text = ">Use 10 lines or less";
        objectiveText[1].color = Color.black;
    }

    void StartAcceptor2(){
        StartStage(4);

        gameObject.GetComponent<Pathfinder>().enabled = true;

        //set enum mode
        currentObjective = LoopObjectives.Acceptor2;

        //set objective text
        objectiveText[0].text = ">Reach the button";
        objectiveText[0].color = Color.black;

        objectiveText[1].transform.gameObject.SetActive(true);
        objectiveText[1].text = ">Use 15 lines or less";
        objectiveText[1].color = Color.black;
    }

    bool successTime = false;
    void CheckTime(){
        if(!successTime){
            return;
        }
        if(Compiler.Instance.linesCount <= 10){
            objectiveText[0].color = successColor;
            objectiveText[1].color = successColor;
            currentObjective = LoopObjectives.AfterFixedLoop;
            StartCoroutine(convoManager.StartDialogue(5, dialogueInvokeTime));
            Compiler.Instance.terminateExecution();
        } else {
            objectiveText[0].color = successColor;
            objectiveText[1].color = Color.red;
            StartCoroutine(convoManager.StartFailDialogue(0, dialogueInvokeTime));
        }
    }

    void IActivate.deactivate(){
        if(currentObjective == LoopObjectives.FixedLoop){
            successTime = false;
        }
    }

    public enum LoopObjectives{
        Start,
        InfLoop1,
        AfterInfLoop1,
        InfLoop2,
        AfterInfLoop2,
        FixedLoop,
        AfterFixedLoop,
        Acceptor1,
        AfterAcceptor,
        Acceptor2,
        AfterAcceptor2
    }
}
