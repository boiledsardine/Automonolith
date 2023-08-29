using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : DialogueSystemBase{
    public static DialogueManager Instance;
    private Queue<Dialogue> dialogueBlocks;

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
        StartCoroutine(typeSentence(dialogueLines.Dequeue()));
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

        StopAllCoroutines();
        StartCoroutine(typeSentence(dialogueLines.Dequeue()));
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
}
