using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConvoManager : MonoBehaviour{
    public Conversation[] convos;
    public Conversation[] failConvos;
    public DialogueManager dialogueManager;

    public IEnumerator StartDialogue(int convoIndex, float delay){
        yield return new WaitForSeconds(delay);
        dialogueManager.enableCanvas();
        dialogueManager.startDialogue(convos[convoIndex]);
    }

    public IEnumerator StartFailDialogue(int convoIndex, float delay){
        dialogueManager.ErrorDialogue = true;
        
        yield return new WaitForSeconds(delay);
        dialogueManager.enableCanvas();
        dialogueManager.startDialogue(failConvos[convoIndex]);
    }
}
