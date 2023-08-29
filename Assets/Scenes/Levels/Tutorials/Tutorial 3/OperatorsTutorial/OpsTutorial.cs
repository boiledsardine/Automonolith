using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeEditorComponents;

public class OpsTutorial : MonoBehaviour, IActivate{
    public OpsObjectives currentObjective;
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
    
    public Vector3[] cameraPos;
    public string[] objective;

    private OpsObjectives[] opsObj = {
        OpsObjectives.MathOps,
        OpsObjectives.IncOps,
        OpsObjectives.AssignOps
    };

    [TextArea(3,10)]
    public string[] comments; 

    void Awake() {
        convoManager = gameObject.GetComponent<ConvoManager>();
        cameraPivot = GameObject.Find("Camera Pivot");
    }

    void Start(){
        switch(currentObjective){
            case OpsObjectives.Start:
                StartStage(0);
            break;
            case OpsObjectives.AfterMathOps:
                StartStage(1);
            break;
            case OpsObjectives.AfterIncOps:
                StartStage(2);
            break;
        }
    }

    int timesPressed = 0;
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
            case OpsObjectives.AfterMathOps:
                if(timesPressed == convoManager.convos[1].LineCount()){
                    StartStage(1);
                    timesPressed = 0;
                }
            break;
            case OpsObjectives.AfterIncOps:
                if(timesPressed == convoManager.convos[2].LineCount()){
                    StartStage(2);
                    timesPressed = 0;
                }
            break;
            default:
                timesPressed = 0;
            break;
        }
    }

    public void activate(){
        switch(currentObjective){
            case OpsObjectives.MathOps:
                objectiveText[0].color = successColor;
                currentObjective = OpsObjectives.AfterMathOps;
                StartCoroutine(convoManager.StartDialogue(1, dialogueInvokeTime));
            break;
            case OpsObjectives.IncOps:
                objectiveText[0].color = successColor;
                currentObjective = OpsObjectives.AfterIncOps;
                StartCoroutine(convoManager.StartDialogue(3, dialogueInvokeTime));
            break;
        }
    }

    public void HintOpen(){
        hintIsOpen = true;
    }

    void StartStage(int num){
        //sets editor comments
        SetEditorComments(num);
        
        //moves camera and player
        MoveCamera(cameraPos[num]);
        var playerChara = GameObject.Find("PlayerCharacter");
        var tpLocs = playerChara.GetComponent<MoveNugget3>().tpLocs;
        playerChara.transform.position = tpLocs[num].transform.position;
        Globals.Instance.moveIndex = num;

        //start conversation
        StartCoroutine(convoManager.StartDialogue(num * 2, dialogueInvokeTime));

        //change enum mode
        currentObjective = opsObj[num];

        //set objective text
        objectiveText[0].text = objective[num];
        objectiveText[0].color = Color.black;

        hintButton.convoToLoad = hints[num];
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

    public void deactivate(){
        //do nothing
    }

    public enum OpsObjectives{
        Start,
        MathOps,
        AfterMathOps,
        IncOps,
        AfterIncOps,
        AssignOps,
        AfterAssignOps,
        Final
    }
}
