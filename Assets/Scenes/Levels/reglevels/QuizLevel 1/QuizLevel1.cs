using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    bool hintIsOpen = false;
    public QuizManager quizMgr;
    public TMPro.TMP_Text scoreText;
    AudioSource source;

    //triggers dialogue
    void Awake(){
        scoreThreshold1 = quiz.threshold1;
        scoreThreshold2 = quiz.threshold2;
        scoreThreshold3 = quiz.threshold3;
        
        convoManager = GetComponent<ConvoManager>();

        convoManager.convos[0] = quiz.startConvo;
        convoManager.convos[1] = quiz.passConvo;
        convoManager.convos[2] = quiz.failConvo;

        source = GetComponent<AudioSource>();
    }

    void Start(){
        StartCoroutine(convoManager.StartDialogue(0, dialogueInvokeTime));
        levelIndex = LevelSaveLoad.Instance.indexHolder;
    }

    int dialogueCounter = 0;
    bool quizStarted = false;
    public void DialogueEnd(){
        if(hintIsOpen){
            hintIsOpen = false;
            return;
        }
        
        dialogueCounter++;
        switch(dialogueCounter){
            case 1:
                if(quizStarted) return;
                quizStarted = true;
                Invoke("StartQuiz", 0.5f);
            break;
            case 3:
                LevelComplete();
            break;
            default: break;
        }
    }

    public void HintOpen(){
        hintIsOpen = true;
    }

    void StartQuiz(){
        quizMgr.startQuiz(quiz, quiz.items);
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
        bool checkOne = QuizManager.Instance.quizScore >= scoreThreshold1;
        bool checkTwo = QuizManager.Instance.quizScore >= scoreThreshold2;
        bool checkThree = QuizManager.Instance.quizScore >= scoreThreshold3;
        bool openNext = state == QuizState.Pass;

        LevelSaveLoad.Instance.EndLevelSave(levelIndex, checkOne, checkTwo, checkThree, openNext);

        var postlevelCanvas = FindObjectOfType<PostlevelCanvas>(true);
        postlevelCanvas.gameObject.SetActive(true);
        PostlevelCanvas.Instance.OpenCanvas();
        PostlevelCanvas.Instance.SetStars(checkOne, checkTwo, checkThree);

        scoreText.text = QuizManager.Instance.quizScore.ToString() + "/10";

        if(!checkOne){
            PlayFailSound();
        } else {
            PlayPassSound();
        }
    }

    void PlayPassSound(){
        source.outputAudioMixerGroup = AudioPicker.Instance.sfxMaster;
        source.clip = AudioPicker.Instance.passSting;
        source.Play();
    }

    void PlayFailSound(){
        source.outputAudioMixerGroup = AudioPicker.Instance.sfxMaster;
        source.clip = AudioPicker.Instance.failSting;
        source.Play();
    }

    public enum QuizState{
        Start, Pass, Fail
    }
}