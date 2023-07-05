using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizTrigger : MonoBehaviour{
    [SerializeField] public Quiz quiz;
    [SerializeField] private Canvas dialogueCanvas;
    public void startDialogue(){
        dialogueCanvas.gameObject.SetActive(true);
        QuizManager.Instance.startQuiz(quiz);
    }
}
