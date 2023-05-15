using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConvoManager : MonoBehaviour{
    [SerializeField] private Conversation[] convos;
    [SerializeField] private Canvas dialogueCanvas;

    public IEnumerator StartDialogue(int convoIndex, float delay){
        yield return new WaitForSeconds(delay);
        dialogueCanvas.gameObject.SetActive(true);
        DialogueManager.Instance.startDialogue(convos[convoIndex]);
    }
}
