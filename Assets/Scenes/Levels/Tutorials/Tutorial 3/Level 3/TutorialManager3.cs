using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeEditorComponents;

public class TutorialManager3 : MonoBehaviour, IActivate{
    public Tutorial3Objectives currentObjective;
    private ConvoManager convoManager;
    private GameObject cameraPivot;
    bool hintIsOpen = false;

    [SerializeField] SetEditorText defaultText;
    [SerializeField] TMPro.TMP_Text editorText;
    [SerializeField] CodeEditor editor;

    [SerializeField] TMPro.TMP_Text[] objectiveText;
    [SerializeField] private float dialogueInvokeTime = 0.5f;
    [SerializeField] private Color successColor = new Color(100f, 200f, 50f, 255f);
    [SerializeField] private DialogueTrigger hintButton;
    [SerializeField] private Conversation[] hints;

    private GameObject playerCharacter;

    [TextArea(3,10)]
    public string[] comments;    

    void Awake() {
        convoManager = gameObject.GetComponent<ConvoManager>();
        cameraPivot = GameObject.Find("Camera Pivot");
    }

    void Start(){
        switch(currentObjective){
            case Tutorial3Objectives.Start:                
                StartCoroutine(convoManager.StartDialogue(0, dialogueInvokeTime));
            break;
            case Tutorial3Objectives.AfterVarsIntro:                
                StartVarsPlay();
            break;
            case Tutorial3Objectives.AfterVarsPlay:
                StartVarsInit();
            break;
            case Tutorial3Objectives.AfterVarsInit:
                StartInitPlay();
            break;
            case Tutorial3Objectives.AfterInitPlay:
                StartExpressions();
            break;
            case Tutorial3Objectives.AfterExpressions:
                StartFinal();
            break;
            default:
            break;
        }
    }

    public void activate(){
        switch(currentObjective){
            case Tutorial3Objectives.VariablesIntro:
                objectiveText[0].color = successColor;
                currentObjective = Tutorial3Objectives.AfterVarsIntro;
                StartCoroutine(convoManager.StartDialogue(1, dialogueInvokeTime));
            break;
            case Tutorial3Objectives.VariablesPlay:
                objectiveText[0].color = successColor;
                currentObjective = Tutorial3Objectives.AfterVarsPlay;
                StartCoroutine(convoManager.StartDialogue(3, dialogueInvokeTime));
            break;
            case Tutorial3Objectives.VarsInitialize:
                objectiveText[0].color = successColor;
                currentObjective = Tutorial3Objectives.AfterVarsInit;
                StartCoroutine(convoManager.StartDialogue(5, dialogueInvokeTime));
            break;
            case Tutorial3Objectives.VarsInitPlay:
                objectiveText[0].color = successColor;
                currentObjective = Tutorial3Objectives.AfterInitPlay;
                StartCoroutine(convoManager.StartDialogue(7, dialogueInvokeTime));
            break;
            case Tutorial3Objectives.VarExpressions:
                objectiveText[0].color = successColor;
                currentObjective = Tutorial3Objectives.AfterExpressions;
                StartCoroutine(convoManager.StartDialogue(9, dialogueInvokeTime));
            break;
            default:
            break;
        }
    }

    private int timesPressed = 0;
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
            case Tutorial3Objectives.Start:
                if(timesPressed == convoManager.convos[0].LineCount()){
                    StartVarsIntro();
                    timesPressed = 0;
                }
            break;
            case Tutorial3Objectives.AfterVarsIntro:
                if(timesPressed == convoManager.convos[1].LineCount()){
                    StartVarsPlay();
                    timesPressed = 0;
                }
            break;
            case Tutorial3Objectives.AfterVarsPlay:
                if(timesPressed == convoManager.convos[3].LineCount()){
                    StartVarsInit();
                    timesPressed = 0;
                }
            break;
            case Tutorial3Objectives.AfterVarsInit:
                if(timesPressed == convoManager.convos[5].LineCount()){
                    StartInitPlay();
                    timesPressed = 0;
                }
            break;
            case Tutorial3Objectives.AfterInitPlay:
                if(timesPressed == convoManager.convos[7].LineCount()){
                    StartExpressions();
                    timesPressed = 0;
                }
            break;
            case Tutorial3Objectives.AfterExpressions:
                if(timesPressed == convoManager.convos[9].LineCount()){
                    StartInitPlay();
                    timesPressed = 0;
                }
            break;
            default:
                timesPressed = 0;
            break;
        }
    }

    private void StartVarsIntro(){
        SetEditorComments(0);
        
        //change enum mode to MethodsPlay
        currentObjective = Tutorial3Objectives.VariablesIntro;

        //set objective text
        objectiveText[0].text = ">Press the correct button";
        objectiveText[0].color = Color.black;

        hintButton.convoToLoad = hints[0];
    }

    private void StartVarsPlay(){
        //sets editor comments
        SetEditorComments(1);
        
        //moves camera and player
        MoveCamera(new Vector3(2240f, 0f, 260f));
        var playerChara = GameObject.Find("PlayerCharacter");
        var tpLocs = playerChara.GetComponent<MoveNugget3>().tpLocs;
        playerChara.transform.position = tpLocs[1].transform.position;

        //start conversation for VarsPlay
        StartCoroutine(convoManager.StartDialogue(2, dialogueInvokeTime));

        //change enum mode to MethodsPlay
        currentObjective = Tutorial3Objectives.VariablesPlay;

        //set objective text
        objectiveText[0].text = ">Find the safe tiles";
        objectiveText[0].color = Color.black;

        hintButton.convoToLoad = hints[1];
    }

    private void StartVarsInit(){
        //sets editor comments
        SetEditorComments(2);
        
        //moves camera and player
        MoveCamera(new Vector3(4150f, 0f, 260f));
        var playerChara = GameObject.Find("PlayerCharacter");
        var tpLocs = playerChara.GetComponent<MoveNugget3>().tpLocs;
        playerChara.transform.position = tpLocs[2].transform.position;

        //start conversation for VarsPlay
        StartCoroutine(convoManager.StartDialogue(4, dialogueInvokeTime));

        //change enum mode to MethodsPlay
        currentObjective = Tutorial3Objectives.VarsInitialize;

        //set objective text
        objectiveText[0].text = ">Find the safe tile";
        objectiveText[0].color = Color.black;

        hintButton.convoToLoad = hints[2];
    }

    private void StartInitPlay(){
        //sets editor comments
        SetEditorComments(3);
        
        //moves camera and player
        MoveCamera(new Vector3(6240f, 0f, 65f));
        var playerChara = GameObject.Find("PlayerCharacter");
        var tpLocs = playerChara.GetComponent<MoveNugget3>().tpLocs;
        playerChara.transform.position = tpLocs[3].transform.position;

        //start conversation for VarsPlay
        StartCoroutine(convoManager.StartDialogue(6, dialogueInvokeTime));

        //change enum mode to MethodsPlay
        currentObjective = Tutorial3Objectives.VarsInitPlay;

        //set objective text
        objectiveText[0].text = ">Find the safe tile";
        objectiveText[0].color = Color.black;

        hintButton.convoToLoad = hints[3];
    }

    private void StartExpressions(){
        //sets editor comments
        SetEditorComments(4);
        
        //moves camera and player
        MoveCamera(new Vector3(8150f, 0f, 260f));
        var playerChara = GameObject.Find("PlayerCharacter");
        var tpLocs = playerChara.GetComponent<MoveNugget3>().tpLocs;
        playerChara.transform.position = tpLocs[4].transform.position;

        //start conversation for VarsPlay
        StartCoroutine(convoManager.StartDialogue(8, dialogueInvokeTime));

        //change enum mode to MethodsPlay
        currentObjective = Tutorial3Objectives.VarExpressions;

        //set objective text
        objectiveText[0].text = ">Find the safe tile";
        objectiveText[0].color = Color.black;

        hintButton.convoToLoad = hints[4];
    }
    
    private void StartFinal(){
        //sets editor comments
        SetEditorComments(5);
        
        //moves camera and player
        MoveCamera(new Vector3(10150f, 0f, 250f));
        var playerChara = GameObject.Find("PlayerCharacter");
        var tpLocs = playerChara.GetComponent<MoveNugget3>().tpLocs;
        playerChara.transform.position = tpLocs[5].transform.position;

        //start conversation for VarsPlay
        StartCoroutine(convoManager.StartDialogue(10, dialogueInvokeTime));

        //change enum mode to MethodsPlay
        currentObjective = Tutorial3Objectives.Final;

        //set objective text
        objectiveText[0].text = ">Find the safe tile";
        objectiveText[0].color = Color.black;

        hintButton.convoToLoad = hints[5];
    }

    public void deactivate(){
        //do nothing
    }

    public void HintOpen(){
        hintIsOpen = true;
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

    private void MoveCamera(Vector3 transformPos){
        cameraPivot.transform.position = transformPos;
        cameraPivot.GetComponent<CameraRotation>().defaultPosition = transformPos;
        cameraPivot.GetComponent<CameraRotation>().ResetCameraRotation();
    }

    public enum Tutorial3Objectives{
        Start,
        VariablesIntro,
        AfterVarsIntro,
        VariablesPlay,
        AfterVarsPlay,
        VarsInitialize,
        AfterVarsInit,
        VarsInitPlay,
        AfterInitPlay,
        VarExpressions,
        AfterExpressions,
        Final,
    }
}
