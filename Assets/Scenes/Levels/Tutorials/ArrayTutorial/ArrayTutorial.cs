using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ArrayTutorial : TutorialBase{
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

    public override IEnumerator ActivateButton(){
        hasError = false;
        yield return new WaitForSeconds(1);
        if(hasError){
            hasError = false;
            yield break;
        }
        
        switch(currentObjective){
            case Objectives.IntroIndices:
                LatchCounter.Instance.latch++;
                if(LatchCounter.Instance.latch == 3){
                    objectiveText[0].color = successColor;
                    currentObjective = Objectives.AfterIntroIndices;
                    StartCoroutine(convoManager.StartDialogue(1, 0));
                    Compiler.Instance.terminateExecution();
                    LatchCounter.Instance.latch = 0;
                }
            break;
            case Objectives.LoopingArray:
                LatchCounter.Instance.latch++;
                if(LatchCounter.Instance.latch == 5){
                    objectiveText[0].color = successColor;
                    currentObjective = Objectives.AfterLoopingArray;
                    StartCoroutine(convoManager.StartDialogue(3, 0));
                    Compiler.Instance.terminateExecution();
                    LatchCounter.Instance.latch = 0;
                }
            break;
            case Objectives.ChangeIndex:
                //check if only one array is used and it's the given variable
                var compiler = Compiler.Instance;
                bool ciFailOne = false;
                bool ciFailTwo = false;

                ciFailOne = !compiler.allVars["arr"].isSet || compiler.intVars["arr`0"] == 1 || compiler.intVars["arr`1"] == 2 || compiler.intVars["arr`2"] == 3;
                ciFailTwo = compiler.intArrs.Count != 2 && !compiler.allVars.ContainsKey("arr");

                if(ciFailOne && ciFailTwo){
                    objectiveText[0].color = successColor;
                    objectiveText[1].color = Color.red;
                    objectiveText[2].color = Color.red;
                    StartCoroutine(convoManager.StartFailDialogue(3, 0));                    
                } else if(ciFailOne){
                    objectiveText[0].color = successColor;
                    objectiveText[1].color = Color.red;
                    objectiveText[2].color = successColor;
                    StartCoroutine(convoManager.StartFailDialogue(4, 0));
                } else if(ciFailTwo){
                    objectiveText[0].color = successColor;
                    objectiveText[1].color = successColor;
                    objectiveText[2].color = Color.red;
                    StartCoroutine(convoManager.StartFailDialogue(5, 0));
                } else {
                    objectiveText[0].color = successColor;
                    objectiveText[1].color = successColor;
                    objectiveText[2].color = successColor;
                    currentObjective = Objectives.AfterChangeIndex;
                    StartCoroutine(convoManager.StartDialogue(5, 0));
                }
                Compiler.Instance.terminateExecution();
                LatchCounter.Instance.latch = 0;
            break;
            case Objectives.NewDeclaration:
                //check if only one array is used and it's the given variable
                var ci = Compiler.Instance;
                var cis = ci.allVars.ContainsKey("arr") ? ci.strArrs["arr"] : null;
                bool failOne = false;
                bool failTwo = false;
                if(cis != null){
                    failOne = !ci.allVars["arr"].isSet || !cis.Contains("Open") || !cis.Contains("Akeru") || !cis.Contains("Ouvrir");
                    failTwo = ci.strArrs.Count != 2 && !ci.allVars.ContainsKey("arr");
                } else {
                    failOne = true;
                    failTwo = true;
                }

                if(failOne && failTwo){
                    objectiveText[0].color = successColor;
                    objectiveText[1].color = Color.red;
                    objectiveText[2].color = Color.red;
                    StartCoroutine(convoManager.StartFailDialogue(0, 0));                    
                } else if(failOne){
                    objectiveText[0].color = successColor;
                    objectiveText[1].color = Color.red;
                    objectiveText[2].color = successColor;
                    StartCoroutine(convoManager.StartFailDialogue(1, 0));
                } else if(failTwo){
                    objectiveText[0].color = successColor;
                    objectiveText[1].color = successColor;
                    objectiveText[2].color = Color.red;
                    StartCoroutine(convoManager.StartFailDialogue(2, 0));
                } else {
                    objectiveText[0].color = successColor;
                    objectiveText[1].color = successColor;
                    objectiveText[2].color = successColor;
                    currentObjective = Objectives.AfterNewDeclaration;
                    StartCoroutine(convoManager.StartDialogue(7, 0));
                }
                Compiler.Instance.terminateExecution();
                LatchCounter.Instance.latch = 0;
            break;
        }
    }

    public void ResetLatches(){
        LatchCounter.Instance.latch = 0;
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
                StartLooping();
            break;
            case 4:
                StartChangeIndex();
            break;
            case 6:
                StartNewDeclaration();
            break;
            case 8: 
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
    */

    void StartIntro(){
        StartStage(0);

        //set enum mode
        currentObjective = Objectives.IntroIndices;

        //set objective text
        objectiveText[0].text = ">Step on the correct buttons";
        objectiveText[0].color = defaultColor;
    }

    void StartLooping(){
        StartStage(1);

        //set enum mode
        currentObjective = Objectives.LoopingArray;

        //set objective text
        objectiveText[0].text = ">Step on the correct buttons";
        objectiveText[0].color = defaultColor;
    }

    void StartChangeIndex(){
        StartStage(2);

        //set enum mode
        currentObjective = Objectives.ChangeIndex;

        //set objective text
        objectiveText[0].text = ">Step on the button";
        objectiveText[0].color = defaultColor;

        objectiveText[1].transform.gameObject.SetActive(true);
        objectiveText[1].text = ">Change the given array's values";
        objectiveText[1].color = defaultColor;

        objectiveText[2].transform.gameObject.SetActive(true);
        objectiveText[2].text = ">Use only the given array";
        objectiveText[2].color = defaultColor;
    }

    void StartNewDeclaration(){
        StartStage(3);

        //set enum mode
        currentObjective = Objectives.NewDeclaration;

        //set objective text
        objectiveText[0].text = ">Step on the button";
        objectiveText[0].color = defaultColor;

        objectiveText[1].transform.gameObject.SetActive(true);
        objectiveText[1].text = ">Assign values to the given array";
        objectiveText[1].color = defaultColor;

        objectiveText[2].transform.gameObject.SetActive(true);
        objectiveText[2].text = ">Use only the given array";
        objectiveText[2].color = defaultColor;
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
