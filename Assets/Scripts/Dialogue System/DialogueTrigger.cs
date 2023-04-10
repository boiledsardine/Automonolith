using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour{
    [SerializeField] private Conversation convoToLoad;
    [SerializeField] private Canvas dialogueCanvas;
    public void startDialogue(){
        dialogueCanvas.gameObject.SetActive(true);
        DialogueManager.Instance.startDialogue(convoToLoad);
    }
}