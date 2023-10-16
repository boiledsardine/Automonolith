using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;

public class MinigameManager : MonoBehaviour{
    public static MinigameManager Instance { get; private set; }
    int levelIndex;
    public MinigameStageInfo stageInfo;
    
    public TMP_Text goalText, mainText;
    public TMP_Text dictName, dictText;

    public Image bottomPanel;
    public GameObject dragBoxHolder, dragSlot;

    List<DragSlot> dsList;

    public Canvas dialogueCanvas;
    Conversation startConvo, passConvo, failConvo;
    
    public HelpManager helpManager;
    public DialogueTrigger hintDialogue;
    public Sprite merlin;
    readonly string[] specialChars = {
        ";",
        "{",
        "}",
        "(",
        ")",
        "[",
        "]",
        "//",
        "_",
        "."
    };

    void Awake(){
        if(Instance == null){
            Instance = this;
        } else {
            Destroy(gameObject);
        }

        stageInfo = MinigameLoader.Instance.stageInfo;
        levelIndex = MinigameLoader.Instance.levelIndex;

        stageInfo.InitializeInfo();
    }


    void Start(){
        helpManager.helpArticle = stageInfo.help;
        hintDialogue.convoToLoad = stageInfo.hint;

        dsList = new List<DragSlot>();
        
        startConvo = (Conversation)ScriptableObject.CreateInstance("Conversation");
        passConvo = (Conversation)ScriptableObject.CreateInstance("Conversation");
        failConvo = (Conversation)ScriptableObject.CreateInstance("Conversation");

        startConvo.dialogueBlocks = stageInfo.startConvo;
        passConvo.dialogueBlocks = stageInfo.passConvo;
        failConvo.dialogueBlocks = new List<Dialogue>();

        Invoke("StartConvo", 0.5f);

        dictName.text = "";
        dictText.text = "Click on something to find out more about it!";

        goalText.text = stageInfo.levelGoal;
        mainText.text = FormatText(stageInfo.mainText);

        //instantiates draggable text boxes
        SpawnDragboxes();        

        //instantiate slots
        FindLinks();
    }

    void StartConvo(){
        dialogueCanvas.gameObject.SetActive(true);
        DialogueManager.Instance.startDialogue(startConvo);
    }

    //formats text by adding links to each word
    //adds the slot link to underscores
    //adds the key link to everything else
    int currWordIndex = 0;
    string FormatText(string text){
        string result = "";
        var linesArr = text.Split('\n');
        for(int i = 0; i < linesArr.Length; i++){
            if(string.IsNullOrWhiteSpace(linesArr[i])){
                result += "\n";
                continue;
            }

            string formattedLine = "";
            string[] wordsArr = linesArr[i].Split(' ');
            for(int j = 0; j < wordsArr.Length; j++){
                string currentWord = wordsArr[j];
                for(int k = 0; k < specialChars.Length; k++){
                    currentWord = currentWord.Replace(specialChars[k], " " + specialChars[k] + " ");
                }

                string[] spacedWord = currentWord.Split(' ');

                for(int k = 0; k < spacedWord.Length; k++){
                    //dev note: # is replaced by spaces by the formatter
                    //want to put spaces in a token? (like in strings) - use # in the scriptable object
                    spacedWord[k] = spacedWord[k].Replace("#", " ");
                    if(Regex.IsMatch(spacedWord[k], @"^_+$")){
                        string hiddenWord = stageInfo.hidWords[currWordIndex].hiddenText;
                        //string hiddenWord = "___";
                        spacedWord[k] = "<link=\"slot\">" + WordToUnderscores(hiddenWord) + "</link>";
                        currWordIndex++;
                    } else {
                        spacedWord[k] = "<link=\"key\">" + spacedWord[k] + "</link>";
                    }
                }
                formattedLine += ArrayToString(spacedWord) + " ";
            }
            result += formattedLine.Trim() + "\n";
        }
        currWordIndex = 0;
        return result;
    }

    string ArrayToString(string[] array){
        string s = "";
        foreach(string str in array){
            s += str;
        }
        return s;
    }

    string WordToUnderscores(string text){
        string result = "";
        for(int i = 0; i < text.Length; i++){
            result += "_";
        }
        return result;
    }

    void SpawnDragboxes(){
        var dragboxList = new List<GameObject>();
        for(int i = 0; i < stageInfo.wordBank.Count; i++){
            var box = Instantiate(dragBoxHolder);
            dragboxList.Add(box);

            var textBox = box.transform.GetChild(0).GetComponent<TextItem>();
            textBox.textToShow = stageInfo.wordBank[i];
            textBox.indexID = i;

            var slotScript = box.GetComponent<DragSlot>();
            slotScript.slotIndex = -1;
        }
        dragboxList.Shuffle();

        //rescales draggables
        foreach(var box in dragboxList){
            box.transform.SetParent(bottomPanel.transform);
            box.transform.localScale = new Vector3(1,1,1);
        }
    }

    //find everything tagged with link="slot"
    //and instantiate slot objects on their locations
    int currentSlotIndex = 0;
    public void FindLinks(){
        DestroySlots();
        
        //says it's an error but is actually correct, don't mind it
        //VScode just being funny with TMP as usual
        mainText.ForceMeshUpdate();

        var links = mainText.textInfo.linkInfo;
        var textCanvas =  mainText.transform.GetComponentInParent<Canvas>();
        for(int i = 0; i < links.Length; i++){
            var entry = links[i];

            if(entry.GetLinkID() != "slot"){
                continue;
            }

            var currentChar = mainText.textInfo.characterInfo[entry.linkTextfirstCharacterIndex];
            
            int lastCharIndex = entry.linkTextLength + entry.linkTextfirstCharacterIndex - 1;
            var lastChar = mainText.textInfo.characterInfo[lastCharIndex];
            
            Vector2 firstCharCenter = GetLinkCenterPosition(currentChar);
            Vector2 lastCharCenter = GetLinkCenterPosition(lastChar);

            float width = lastCharCenter.x - firstCharCenter.x;
            float height = lastCharCenter.y - firstCharCenter.y;

            Vector2 centerPoint = firstCharCenter;
            centerPoint.x += width / 2;
            centerPoint.y += height / 2;
            
            var slot = Instantiate(dragSlot);
            var slotSizeScript = slot.GetComponent<SetSlotSize>();
            slotSizeScript.setText = entry.GetLinkText();
            
            slot.transform.position = new Vector3(centerPoint.x, centerPoint.y, 0);
            slot.transform.SetParent(textCanvas.transform);
            slot.transform.localScale = new Vector3(1,1,1);

            var slotScript = slot.GetComponent<DragSlot>();
            slotScript.slotIndex = currentSlotIndex;
            slotScript.correctAnswers = stageInfo.hidWords[currentSlotIndex].correctAnswers;
            dsList.Add(slotScript);
            currentSlotIndex++;
        }
    }

    void DestroySlots(){
        dsList.Clear();
        foreach(var slot in FindObjectsOfType<SetSlotSize>()){
            Destroy(slot.gameObject);
        }
    }

    Vector2 GetLinkCenterPosition(TMP_CharacterInfo currentChar){
        Transform m_transform = mainText.transform.gameObject.GetComponent<Transform>();

        float maxAscender = -Mathf.Infinity;
        float minDescender = Mathf.Infinity;

        maxAscender = Math.Max(maxAscender, currentChar.ascender);
        minDescender = Math.Min(minDescender, currentChar.descender);

        Vector3 bottomLeft = new Vector3(currentChar.bottomLeft.x, currentChar.descender, 0);
        bottomLeft = m_transform.TransformPoint(new Vector3(bottomLeft.x, minDescender, 0));
        Vector3 topRight = m_transform.TransformPoint(new Vector3(currentChar.topRight.x, maxAscender, 0));

        float width = topRight.x - bottomLeft.x;
        float height = topRight.y - bottomLeft.y;

        Vector2 centerPosition = bottomLeft;
        centerPosition.x += width/2;
        centerPosition.y += height/2;
        
        return centerPosition;
    }

    bool levelOver = false;

    public void CheckEntries(){
        bool allTrue = true;
        failConvo.dialogueBlocks.Clear();
        for(int i = 0; i < dsList.Count; i++){
            if(!dsList[i].isCorrect){
                allTrue = false;
                string[] msg = {stageInfo.hidWords[i].mistakeMessage};

                if(dsList[i].transform.childCount == 0){
                    msg[0] = string.Format("{0}: At least put something in there", i + 1);
                } else if(msg[0].Contains("___")){
                    string newMsg = dsList[i].transform.GetChild(0).GetComponent<TextItem>().textToShow;
                    msg[0] = msg[0].Replace("___", newMsg);
                }

                failConvo.dialogueBlocks.Add(new Dialogue(msg, "Merlin", 'R', true, merlin));
            }
        }

        if(allTrue){
            dialogueCanvas.gameObject.SetActive(true);
            DialogueManager.Instance.startDialogue(passConvo);

            //TODO: Add check for if the pass convo has been read (tutorial NextLinePressed-esque?)
            //save the level after the pass convo has been read
            levelOver = true;
        } else {
            dialogueCanvas.gameObject.SetActive(true);
            DialogueManager.Instance.startDialogue(failConvo);
        }
    }

    int timesPressed = 0;

    //could probably get away with a cheeky Broadcast here
    public void NextLinePressed(){
        if(!levelOver){
            return; 
        }

        timesPressed++;

        if(timesPressed == passConvo.LineCount()){
            timesPressed = 0;
            //end level
            LastSceneHolder.Instance.SetLastScene();

            DontDestroy[] persistents = FindObjectsOfType<DontDestroy>();
            foreach(DontDestroy obj in persistents){
                if(obj.transform.gameObject.tag != "Save Manager"){
                    Destroy(obj.gameObject);
                }
            }

            MinigameLoader.Instance.EndLevelSave(levelIndex, true);
            SceneManager.LoadScene("Main Menu");
        }
    }
}
