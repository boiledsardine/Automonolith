using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour{
    public void startDialogue(){
        DialogueManager.Instance.startDialogue(0, 0);
    }

    public void startQuiz(){
        QuizManager.Instance.startDialogue(0, 0);
    }
}