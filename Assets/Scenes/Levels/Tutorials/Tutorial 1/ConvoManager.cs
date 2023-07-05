using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConvoManager : MonoBehaviour{
    public Conversation[] convos;

    public IEnumerator StartDialogue(int convoIndex, float delay){
        yield return new WaitForSeconds(delay);
        DialogueManager.Instance.enableCanvas();
        DialogueManager.Instance.startDialogue(convos[convoIndex]);
    }
}
