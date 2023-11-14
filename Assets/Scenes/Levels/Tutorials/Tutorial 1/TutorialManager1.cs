using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager1 : MonoBehaviour{
    private CameraRotation cameraRotation;
    [SerializeField] private TMPro.TMP_Text[] objectiveText;
    [SerializeField] private string[] obj1strings = new string[3];
    [SerializeField] private string[] obj2strings = new string[2];
    [SerializeField] private string obj3string;
    [SerializeField] private TMPro.TMP_Text mainObjText;
    public Color successColor = new Color(100f, 200f, 50f, 255f);
    private bool[] obj1bools = {false, false, false};
    private bool[] obj2bools = {false, false};
    private bool obj3bool = false;
    private int currentObjective = 0;
    public Image[] buttonGroup1, buttonGroup1Children;
    public GameObject buttonGroup2;
    private bool hintPressed = false;
    private EditorToggle editorToggle;
    private ConvoManager convoManager;
    public DialogueTrigger hintButton;
    public Conversation[] hints;
    public Button openEditor;
    public GameObject ffButtons;

    private void Awake(){
        cameraRotation = FindObjectOfType<CameraRotation>();
        editorToggle = FindObjectOfType<EditorToggle>(true);
        convoManager = gameObject.GetComponent<ConvoManager>();
    }

    private void Start(){
        StartCoroutine(convoManager.StartDialogue(0, 1.5f));
    }

    private void Update(){
        if(currentObjective == 1){
            TutorialObjective1();
        } else if(currentObjective == 2){
            TutorialObjective2();
        } else if(currentObjective == 3){
            TutorialObjective3();
        } else {
            //do nothing
        }
    }

    //first: check for left right and middle mouse clicks with delta
    private bool secondConvoActive = false;
    public void TutorialObjective1(){
        if(obj1bools[0]){
            objectiveText[0].color = successColor;
        }
        if(obj1bools[1]){
            objectiveText[1].color = successColor;
        }
        if(obj1bools[2]){
            objectiveText[2].color = successColor;
        }

        if((obj1bools[0] && obj1bools[1] && obj1bools[2])){
            if(!secondConvoActive){
                StartCoroutine(convoManager.StartDialogue(1, 0.5f));
                secondConvoActive = true;
            }
        } else {
            CheckCamState();
        }
    }

    public void CheckCamState(){
        Vector2 delta = cameraRotation.delta;
        if(cameraRotation.isMoving && (delta.x > 0f || delta.y > 0f)){
            obj1bools[0] = true;
        }
        if(cameraRotation.isRotating && (delta.x > 0f || delta.y > 0f)){
            obj1bools[1] = true;
        }
        if(Input.GetMouseButton(2)){
            obj1bools[2] = true;
        }
    }

    //second: check for hint and help presses
    //especially when their respective box closes
    private bool thirdConvoActive = false;
    public void TutorialObjective2(){
        if(obj2bools[0]){
            objectiveText[0].color = successColor;
        }
        if(obj2bools[1]){
            objectiveText[1].color = successColor;
        }
        
        if(obj2bools[0] && obj2bools[1]){
            if(!thirdConvoActive){
                timesPressed = 0;
                StartCoroutine(convoManager.StartDialogue(2, 0.5f));
                thirdConvoActive = true;
            }
        }
    }

    //called by Hint button
    public void HintOpen(){
        hintPressed = true;
    }

    //called by nextLine button in dialogue box
    //activates only if Hint button was pressed beforehand
    public void HintDone(){
        if(hintPressed && currentObjective >= 2){
            hintPressed = false;
            obj2bools[0] = true;
            timesPressed--;
        }
    }

    //called by close help button
    public void HelpDone(){
        if(currentObjective == 2){
            obj2bools[1] = true;
        }
    }

    //third: check if the editor gets opened
    //route editor controls here to lock it until Obj 3
    //called by space button in PlayerInput
    private bool fourthConvoActive = false;
    public void TutorialObjective3(){
        if(obj3bool){
            objectiveText[0].color = successColor;

            if(!fourthConvoActive){
                timesPressed = 0;
                StartCoroutine(convoManager.StartDialogue(3, 0.5f));
                fourthConvoActive = true;
            }
        }
    }

    public void OpenEditor(){
        if(currentObjective >= 3){
            editorToggle.openEditor();
            obj3bool = true;
        }
    }

    public int timesPressed = 0;
    public void NextLinePressed(){
        timesPressed++;

        if(currentObjective == 0 && timesPressed == convoManager.convos[0].LineCount()){
            mainObjText.transform.gameObject.SetActive(true);
            
            objectiveText[0].transform.gameObject.SetActive(true);
            objectiveText[1].transform.gameObject.SetActive(true);
            objectiveText[2].transform.gameObject.SetActive(true);
            
            objectiveText[0].text = obj1strings[0];
            objectiveText[1].text = obj1strings[1];
            objectiveText[2].text = obj1strings[2];
            
            timesPressed = 0;
            currentObjective++;
        }

        if(currentObjective == 1 && timesPressed == 1){
            buttonGroup1[0].enabled = true;
            buttonGroup1[1].enabled = true;

            buttonGroup1Children[0].enabled = true;
            buttonGroup1Children[1].enabled = true;

            ffButtons.SetActive(true);
        }

        if(currentObjective == 1 && timesPressed == convoManager.convos[1].LineCount()){
            objectiveText[2].transform.gameObject.SetActive(false);

            objectiveText[0].text = obj2strings[0];
            objectiveText[1].text = obj2strings[1];

            objectiveText[0].color = Color.white;
            objectiveText[1].color = Color.white;

            timesPressed = 0;
            currentObjective++;
        }
        
        if(currentObjective == 2 && timesPressed == convoManager.convos[2].LineCount()){
            hintButton.convoToLoad = hints[0]; 
            
            objectiveText[1].transform.gameObject.SetActive(false);

            objectiveText[0].text = obj3string;
            objectiveText[0].color = Color.white;

            timesPressed = 0;
            currentObjective++;
        }

        if(currentObjective == 3 && timesPressed == 3){
            buttonGroup2.SetActive(true);
            openEditor.gameObject.SetActive(true);
        }

        if(currentObjective == 3 && timesPressed == convoManager.convos[3].LineCount()){
            hintButton.convoToLoad = hints[1];

            objectiveText[0].text = ">Reach the green button";
            objectiveText[0].color = Color.white;
            
            timesPressed = 0;
            currentObjective++;
        }
    }

    //fourth: actual gameplay
    //Nothing to it, just check for level finish
    //Also route the hint button here
    //Change the convo
}