 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DialogueManager : DialogueSystemBase, IPointerClickHandler{
    public static DialogueManager Instance;
    private Queue<Dialogue> dialogueBlocks;
    public Button nextButton;
    public bool allowSkipping;
    string currentSentence;

    new void Awake(){
        base.Awake();

        if(Instance == null){
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }

        dialogueBlocks = new Queue<Dialogue>();
    }

    public override void startDialogue(Conversation convoToLoad){
        foreach(Dialogue d in convoToLoad.dialogueBlocks){
            dialogueBlocks.Enqueue(d);
        }
        
        dialogueBoxAnimator.SetBool("isOpen", true);
        panelAnimator.SetBool("isOpen", true);

        loadDialogue();

        StopAllCoroutines();
        currentSentence = dialogueLines.Dequeue();
        StartCoroutine(typeSentence(currentSentence));
    }

    public IEnumerator typeSentence(string sentence){
        dialogueText.text = "";
        foreach(char letter in CodeColorizer.Colorize(sentence, false, theme).ToCharArray()){
            dialogueText.text += letter;
            yield return null;
        }
        nextButton.gameObject.SetActive(true);
    }

    public override void nextLine(){
        if(dialogueLines.Count == 0){
            //check if there's another block in the queue
            //if yes, end dialogue
            //if no, load the next one
            if(dialogueBlocks.Count == 0){
                endDialogue();
                return;
            } else {
                loadDialogue();
            }
        }
        nextButton.gameObject.SetActive(false);

        StopAllCoroutines();
        currentSentence = dialogueLines.Dequeue();
        StartCoroutine(typeSentence(currentSentence));
    }

    public void loadDialogue(){
        Dialogue dialogue = dialogueBlocks.Dequeue();
        dialogueLines.Clear();

        npcHighlight(dialogue);

        nameText.text = dialogue.npcName;

        foreach(string s in dialogue.lines){
            dialogueLines.Enqueue(s);
        }
    }

    void ForceDialogueComplete(){
        StopAllCoroutines();
        dialogueText.text = CodeColorizer.Colorize(currentSentence, false, theme);
        nextButton.gameObject.SetActive(true);
    }

    public void OnPointerClick(PointerEventData eventData){
        if(!allowSkipping) return;

        if(nextButton.gameObject.activeSelf){
            nextButton.onClick.Invoke();
        } else {
            ForceDialogueComplete();
        }
    }
}
