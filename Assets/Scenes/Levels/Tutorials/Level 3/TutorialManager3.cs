using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager3 : MonoBehaviour, IActivate{
    public Tutorial3Objectives currentObjective;
    private ConvoManager convoManager;
    private GameObject cameraPivot;
    [SerializeField] TMPro.TMP_Text[] objectiveText;
    [SerializeField] private float dialogueInvokeTime = 0.5f;
    [SerializeField] private Color successColor = new Color(100f, 200f, 50f, 255f);
    [SerializeField] private DialogueTrigger hintButton;
    [SerializeField] private Conversation[] hints;

    private GameObject playerCharacter;
    

    void Awake() {
        convoManager = gameObject.GetComponent<ConvoManager>();
        cameraPivot = GameObject.Find("Camera Pivot");
    }

    void Start(){
        switch(currentObjective){
            case Tutorial3Objectives.Start:
                StartCoroutine(convoManager.StartDialogue(0, dialogueInvokeTime));
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
            default:
            break;
        }
    }

    private int timesPressed = 0;
    void NextLinePressed(){
        timesPressed++;
        
        switch(currentObjective){
            case Tutorial3Objectives.Start:
                if(timesPressed == convoManager.convos[0].LineCount()){
                    StartVarsIntro();
                    timesPressed = 0;
                }
            break;
            case Tutorial3Objectives.AfterVarsIntro:
                if(timesPressed == convoManager.convos[1].LineCount()){
                    //set to next objective
                    timesPressed = 0;
                }
            break;
            default:
            break;
        }
    }

    private void StartVarsIntro(){
        currentObjective = Tutorial3Objectives.VariablesIntro;
    }

    public void InvokeAddVars(){
        Invoke("AddVars", 0.03f);
    }

    public void AddVars(){
        Compiler.Instance.allVars.Add("num", new VariableInfo(VariableInfo.Type.intVar, true));
        Compiler.Instance.intVars.Add("num", 3);
    }

    public void deactivate(){
        //do nothing
    }

    public enum Tutorial3Objectives{
        Start,
        VariablesIntro,
        AfterVarsIntro,
    }
}
