using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeEditorComponents;
using UnityEngine.UI;

public abstract class TutorialBase : MonoBehaviour, IActivate{
    protected ConvoManager convoManager;
    protected GameObject cameraPivot;
    protected bool hintIsOpen = false;
    protected Color defaultColor = Color.white;

    public GameObject[] stages;
    public int activeStage;

    [SerializeField] protected SetEditorText defaultText;
    [SerializeField] protected TMPro.TMP_Text editorText;
    [SerializeField] protected CodeEditor editor;

    [SerializeField] protected TMPro.TMP_Text[] objectiveText;
    [SerializeField] protected float dialogueInvokeTime = 0.5f;
    [SerializeField] protected Color successColor = new Color(100f, 200f, 50f, 255f);
    [SerializeField] protected DialogueTrigger hintButton;
    [SerializeField] protected Conversation[] hints;
    [SerializeField] protected int dialogueEndCount = 0;

    protected bool buttonIsActivated = false;

    public Vector3[] cameraPos;
    
    [TextArea(3,30)]
    public string[] comments;
    public bool hasError = false;

    protected void Awake() {
        convoManager = gameObject.GetComponent<ConvoManager>();
        cameraPivot = GameObject.Find("Camera Pivot");
        hintButton.gameObject.GetComponent<Button>().onClick.AddListener(HintOpen);
    }

    protected void DeactivateAllStages(){        
        for(int i = 0; i < stages.Length; i++){
            stages[i].SetActive(false);
        }
    }

    //public abstract void NextLinePressed();
    public abstract void DialogueEnd();

    public void activate(){
        StartCoroutine(ActivateButton());
    }

    public void deactivate(){
        //do nothing
    }

    public void HintOpen(){
        Debug.Log("Hint opened!");
        hintIsOpen = true;
    }

    public abstract IEnumerator ActivateButton();

    protected void SetEditorComments(int commentIndex){
        defaultText.defaultInput = comments[commentIndex];
        editor.code = comments[commentIndex];
        editorText.text = comments[commentIndex];

        string[] codeLines = comments[commentIndex].Split('\n');
        editor.lineIndex = codeLines.Length - 1;
        string finalLine = codeLines[codeLines.Length - 1];
        editor.charIndex = finalLine.Length;
    }

    protected void MoveCamera(Vector3 transformPos){
        cameraPivot.transform.position = transformPos;
        cameraPivot.GetComponent<CameraRotation>().defaultPosition = transformPos;
        cameraPivot.GetComponent<CameraRotation>().ResetCameraRotation();
    }

    protected void StartStage(int num){
        //activates and deactivates stages
        DeactivateAllStages();
        stages[num].SetActive(true);
        activeStage = num;

        //sets editor comments and hints
        SetEditorComments(num);
        hintButton.convoToLoad = hints[num];
        
        //moves camera and player
        MoveCamera(cameraPos[num]);
        var playerChara = GameObject.Find("PlayerCharacter");
        var tpLocs = playerChara.GetComponent<MoveNugget3>().tpLocs;
        playerChara.transform.position = tpLocs[num].transform.position;
        Globals.Instance.moveIndex = num;

        //start conversation
        StartCoroutine(convoManager.StartDialogue(num * 2, dialogueInvokeTime));
    }
}
