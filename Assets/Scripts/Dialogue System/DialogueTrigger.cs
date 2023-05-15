using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour{
    [SerializeField] public Conversation convoToLoad;
    [SerializeField] private Canvas dialogueCanvas;
    public void startDialogue(){
        dialogueCanvas.gameObject.SetActive(true);
        DialogueManager.Instance.startDialogue(convoToLoad);
    }
}