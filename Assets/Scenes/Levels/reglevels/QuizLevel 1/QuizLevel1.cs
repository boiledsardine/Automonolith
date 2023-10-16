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
    int scoreThreshold1, scoreThreshold2, scoreThreshold3;

    float dialogueInvokeTime = 0.5f;

    //triggers dialogue
    void Awake(){
        scoreThreshold1 = quiz.threshold1;
        scoreThreshold2 = quiz.threshold2;
        scoreThreshold3 = quiz.threshold3;
        
        convoManager = GetComponent<ConvoManager>();

        convoManager.convos[0] = quiz.startConvo;
        convoManager.convos[1] = quiz.passConvo;
        convoManager.convos[2] = quiz.failConvo;
    }

    void Start(){
        StartCoroutine(convoManager.StartDialogue(0, dialogueInvokeTime));
        levelIndex = LevelSaveLoad.Instance.indexHolder;
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
        QuizManager.Instance.startQuiz(quiz, quiz.items);
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

        if(QuizManager.Instance.quizScore >= scoreThreshold3){
            checkThree = true;
        }

        if(state == QuizState.Pass){
            openNext = true;
        }

        LevelSaveLoad.Instance.EndLevelSave(levelIndex, checkOne, checkTwo, checkThree, openNext);

        //destroy persistents
        DontDestroy[] persistents = FindObjectsOfType<DontDestroy>();
        foreach(DontDestroy obj in persistents){
            Destroy(obj.gameObject);
        }

        SceneManager.LoadScene("Main Menu");
    }

    public enum QuizState{
        Start, Pass, Fail
    }
}