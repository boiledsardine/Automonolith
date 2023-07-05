using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeEditorComponents;

public class TutorialManager2 : MonoBehaviour, IActivate{
    private ConvoManager convoManager;
    public Tutorial2Objectives currentObjective;
    [SerializeField] SetEditorText defaultText;
    [SerializeField] TMPro.TMP_Text[] objectiveText;
    [SerializeField] TMPro.TMP_Text editorText;
    [SerializeField] CodeEditor editor;
    [SerializeField] private float dialogueInvokeTime = 0.5f;
    public Color successColor = new Color(100f, 200f, 50f, 255f);
    public DialogueTrigger hintButton;
    public Conversation[] hints;
    private GameObject cameraPivot;
    private GameObject playerChara;
    
    [TextArea(3,10)]
    public string[] comments;

    void Awake(){
        cameraPivot = GameObject.Find("Camera Pivot");
        convoManager = GetComponent<ConvoManager>();
    }

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
            case Tutorial2Objectives.FinalStretch:
                StartFinalStretch();
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
                    currentObjective = Tutorial2Objectives.ParamsFail;
                    StartCoroutine(convoManager.StartDialogue(11, dialogueInvokeTime));
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
    private bool hintIsOpen = false;
    public void NextLinePressed(){
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
                    StartFinalStretch();
                    timesPressed = 0;
                }
            break;
            case Tutorial2Objectives.ParamsFail:
                if(timesPressed == convoManager.convos[11].LineCount()){
                    timesPressed = 0;
                    currentObjective = Tutorial2Objectives.ParamsIntro;
                }
            break;
            default:
                timesPressed = 0;
            break;
        }
    }

    public void HintOpen(){
        hintIsOpen = true;
    }

    private void StartMethodsPlay(){
        //sets editor comments
        SetEditorComments(1);

        //moves camera and player
        MoveCamera(new Vector3(2250f, 0f, 260f));
        var playerChara = GameObject.Find("PlayerCharacter");
        var tpLocs = playerChara.GetComponent<MoveNugget>().tpLocs;
        playerChara.transform.position = tpLocs[1].transform.position;

        //start conversation for MethodsPlay
        StartCoroutine(convoManager.StartDialogue(2, dialogueInvokeTime));

        //change enum mode to MethodsPlay
        currentObjective = Tutorial2Objectives.MethodsPlay;

        //set objective text
        objectiveText[0].text = ">Press the button";
        objectiveText[0].color = Color.black;

        hintButton.convoToLoad = hints[1];
    }

    private void StartParamsIntro(){
        //sets editor comments
        SetEditorComments(2);

        //moves camera and player
        MoveCamera(new Vector3(4300f, 0f, 250f));
        var playerChara = GameObject.Find("PlayerCharacter");
        var tpLocs = playerChara.GetComponent<MoveNugget>().tpLocs;
        playerChara.transform.position = tpLocs[2].transform.position;

        //start conversation for ParamsIntro
        StartCoroutine(convoManager.StartDialogue(4, dialogueInvokeTime));

        //change enum mode to ParamsIntro
        currentObjective = Tutorial2Objectives.ParamsIntro;
        
        //set objective text
        objectiveText[0].text = ">Press the button";
        objectiveText[0].color = Color.black;

        objectiveText[1].gameObject.SetActive(true);
        objectiveText[1].text = ">Use 5 lines or less";
        objectiveText[0].color = Color.black;

        hintButton.convoToLoad = hints[2];
    }

    private void StartTurnInteract(){
        //sets editor comments
        SetEditorComments(3);

        //moves camera and player
        MoveCamera(new Vector3(6300f, 0f, 250f));
        var playerChara = GameObject.Find("PlayerCharacter");
        var tpLocs = playerChara.GetComponent<MoveNugget>().tpLocs;
        playerChara.transform.position = tpLocs[3].transform.position;

        //starts conversation for TurnInteract
        StartCoroutine(convoManager.StartDialogue(6, dialogueInvokeTime));

        //change enum mode to TurnInteract
        currentObjective = Tutorial2Objectives.TurnInteract;
        
        //set objective text
        objectiveText[0].text = ">Press the button";
        objectiveText[0].color = Color.black;
        objectiveText[1].gameObject.SetActive(false);

        hintButton.convoToLoad = hints[3];
    }

    private void StartTurnInteract2(){
        //sets editor comments
        SetEditorComments(4);

        //moves camera and player
        MoveCamera(new Vector3(8300f, 0f, 250f));
        var playerChara = GameObject.Find("PlayerCharacter");
        var tpLocs = playerChara.GetComponent<MoveNugget>().tpLocs;
        playerChara.transform.position = tpLocs[4].transform.position;

        //starts conversation for TurnInteract2
        StartCoroutine(convoManager.StartDialogue(8, dialogueInvokeTime));

        //change enum mode to TurnInteract2
        currentObjective = Tutorial2Objectives.TurnInteract2;

        //set objective text
        objectiveText[0].text = ">Press the button";
        objectiveText[0].color = Color.black;

        hintButton.convoToLoad = hints[4];
    }

    private void StartFinalStretch(){
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
        objectiveText[0].color = Color.black;

        hintButton.convoToLoad = hints[5];
    }

    private void MoveCamera(Vector3 transformPos){
        cameraPivot.transform.position = transformPos;
        cameraPivot.GetComponent<CameraRotation>().defaultPosition = transformPos;
        cameraPivot.GetComponent<CameraRotation>().ResetCameraRotation();
    }

    private void SetEditorComments(int commentIndex){
        defaultText.defaultInput = comments[commentIndex];
        editor.code = comments[commentIndex];
        editorText.text = comments[commentIndex];

        string[] codeLines = comments[commentIndex].Split('\n');
        editor.lineIndex = codeLines.Length - 1;
        string finalLine = codeLines[codeLines.Length - 1];
        editor.charIndex = finalLine.Length;
    }

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
    FinalStretch,
    ParamsFail
}