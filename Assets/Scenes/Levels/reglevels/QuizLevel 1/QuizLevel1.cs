using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuizLevel1 : MonoBehaviour{
    //list of Convo objects
    private ConvoManager convoManager;
    //quiz item
    public Quiz quiz;
    public int levelIndex;
    public QuizState state;
    public int scoreThreshold1;
    public int scoreThreshold2;
    public int scoreThreshold3;

    float dialogueInvokeTime = 0.5f;

    //triggers dialogue
    void Start(){
        convoManager = GetComponent<ConvoManager>();
        StartCoroutine(convoManager.StartDialogue(0, dialogueInvokeTime));
    }

    public int timesPressed = 0;
    //count times pressed
    public void NextPressed(){
        timesPressed++;
        switch(state){
            case QuizState.Start:
                if(timesPressed == convoManager.convos[0].LineCount()){
                    //start quiz proper
                    Invoke("StartQuiz", 0.5f);
                    timesPressed = 0;
                }
            break;
            case QuizState.Pass:
                if(timesPressed == convoManager.convos[1].LineCount()){
                    timesPressed = 0;
                    LevelComplete();
                }
            break;
            case QuizState.Fail:
                if(timesPressed == convoManager.convos[2].LineCount()){
                    timesPressed = 0;
                    LevelComplete();
                }
            break;
        }
    }

    void StartQuiz(){
        QuizManager.Instance.startQuiz(quiz,5);
    }

    public void QuizEnd(){
        if(QuizManager.Instance.quizScore >= scoreThreshold1){
            state = QuizState.Pass;
            StartCoroutine(convoManager.StartDialogue(1, dialogueInvokeTime));
        } else {
            state = QuizState.Fail;
            StartCoroutine(convoManager.StartDialogue(2, dialogueInvokeTime));
        }
    }

    //saves data
    public void LevelComplete(){
        bool checkOne = false;
        bool checkTwo = false;
        bool checkThree = false;
        bool openNext = false;

        if(QuizManager.Instance.quizScore >= scoreThreshold1){
            checkOne = true;
        }

        if(QuizManager.Instance.quizScore >= scoreThreshold2){
            checkTwo = true;
        }

        if(QuizManager.Instance.quizScore == scoreThreshold3){
            checkThree = true;
        }

        if(state == QuizState.Pass){
            openNext = true;
        }

        LevelSaveLoad.Instance.EndLevelSave(levelIndex, checkOne, checkTwo, checkThree, openNext);
        SceneManager.LoadScene("Main Menu");
    }

    public enum QuizState{
        Start, Pass, Fail
    }
}