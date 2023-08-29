using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ArrayTutorial : TutorialBase, IActivate{
    public Objectives currentObjective;
    void Start(){
        switch(currentObjective){
            case Objectives.Start:
                StartIntro();
            break;
            case Objectives.LoopingArray:
                StartLooping();
            break;
            case Objectives.ChangeIndex:
                StartChangeIndex();
            break;
            case Objectives.NewDeclaration:
                StartNewDeclaration();
            break;
        }
    }

    int latches = 0;
    public void activate(){
        switch(currentObjective){
            case Objectives.IntroIndices:
                latches++;
                if(latches == 3){
                    objectiveText[0].color = successColor;
                    currentObjective = Objectives.AfterIntroIndices;
                    StartCoroutine(convoManager.StartDialogue(1, dialogueInvokeTime));
                    Compiler.Instance.terminateExecution();
                    latches = 0;
                }
            break;
            case Objectives.LoopingArray:
                latches++;
                if(latches == 5){
                    objectiveText[0].color = successColor;
                    currentObjective = Objectives.AfterLoopingArray;
                    StartCoroutine(convoManager.StartDialogue(3, dialogueInvokeTime));
                    Compiler.Instance.terminateExecution();
                    latches = 0;
                }
            break;
            case Objectives.ChangeIndex:
                objectiveText[0].color = successColor;
                objectiveText[1].color = successColor;
                currentObjective = Objectives.AfterChangeIndex;
                StartCoroutine(convoManager.StartDialogue(5, dialogueInvokeTime));
                Compiler.Instance.terminateExecution();
                latches = 0;
            break;
            case Objectives.NewDeclaration:
                //check if only one array is used and it's the given variable
                var ci = Compiler.Instance;
                var cis = ci.strArrs["arr"];
                bool failOne = !ci.allVars["arr"].isSet || !cis.Contains("Open") || !cis.Contains("Akeru") || !cis.Contains("Ouvrir");
                bool failTwo = ci.strArrs.Count != 2 && !ci.allVars.ContainsKey("arr");

                if(failOne && failTwo){
                    objectiveText[0].color = successColor;
                    objectiveText[1].color = Color.red;
                    objectiveText[2].color = Color.red;
                    StartCoroutine(convoManager.StartFailDialogue(0, dialogueInvokeTime));                    
                } else if(failOne){
                    objectiveText[0].color = successColor;
                    objectiveText[1].color = Color.red;
                    objectiveText[2].color = successColor;
                    StartCoroutine(convoManager.StartFailDialogue(1, dialogueInvokeTime));
                } else if(failTwo){
                    objectiveText[0].color = successColor;
                    objectiveText[1].color = successColor;
                    objectiveText[2].color = Color.red;
                    StartCoroutine(convoManager.StartFailDialogue(2, dialogueInvokeTime));
                } else {
                    objectiveText[0].color = successColor;
                    objectiveText[1].color = successColor;
                    objectiveText[2].color = successColor;
                    currentObjective = Objectives.AfterNewDeclaration;
                    StartCoroutine(convoManager.StartDialogue(7, dialogueInvokeTime));
                }
                Compiler.Instance.terminateExecution();
                latches = 0;
            break;
        }
    }

    public void ResetLatches(){
        latches = 0;
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
            case Objectives.AfterIntroIndices:
                if(timesPressed == convoManager.convos[1].LineCount()){
                    StartLooping();
                    timesPressed = 0;
                }
            break;
            case Objectives.AfterLoopingArray:
                if(timesPressed == convoManager.convos[3].LineCount()){
                    StartChangeIndex();
                    timesPressed = 0;
                }
            break;
            case Objectives.AfterChangeIndex:
                if(timesPressed == convoManager.convos[5].LineCount()){
                    StartNewDeclaration();
                    timesPressed = 0;
                }
            break;
            case Objectives.AfterNewDeclaration:
                if(timesPressed == convoManager.convos[7].LineCount()){
                    timesPressed = 0;
                    GameObject.Find("exit-point").GetComponent<IActivate>().activate();
                }
            break;
            default:
                timesPressed = 0;
            break;
        }
    }

    void StartIntro(){
        StartStage(0);

        //set enum mode
        currentObjective = Objectives.IntroIndices;

        //set objective text
        objectiveText[0].text = ">Find the real buttons";
        objectiveText[0].color = Color.black;
    }

    void StartLooping(){
        StartStage(1);

        //set enum mode
        currentObjective = Objectives.LoopingArray;

        //set objective text
        objectiveText[0].text = ">Find the real buttons";
        objectiveText[0].color = Color.black;
    }

    void StartChangeIndex(){
        StartStage(2);

        //set enum mode
        currentObjective = Objectives.ChangeIndex;

        //set objective text
        objectiveText[0].text = ">Reach the button";
        objectiveText[0].color = Color.black;

        objectiveText[1].transform.gameObject.SetActive(true);
        objectiveText[1].text = ">Change the array's values";
        objectiveText[1].color = Color.black;
    }

    void StartNewDeclaration(){
        StartStage(3);

        //set enum mode
        currentObjective = Objectives.NewDeclaration;

        //set objective text
        objectiveText[0].text = ">Reach the button";
        objectiveText[0].color = Color.black;

        objectiveText[1].transform.gameObject.SetActive(true);
        objectiveText[1].text = ">Assign values to the given array";
        objectiveText[1].color = Color.black;

        objectiveText[2].transform.gameObject.SetActive(true);
        objectiveText[2].text = ">Use only the given array";
        objectiveText[2].color = Color.black;
    }

    public void deactivate(){
        //do nothing
    }

    public enum Objectives{
        Start,
        IntroIndices,
        AfterIntroIndices,
        LoopingArray,
        AfterLoopingArray,
        ChangeIndex,
        AfterChangeIndex,
        NewDeclaration,
        AfterNewDeclaration
    }
}
