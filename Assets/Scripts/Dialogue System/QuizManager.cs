using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class QuizManager : DialogueSystemBase, IPointerClickHandler{
    public static QuizManager Instance;

    [SerializeField] private Text[] choiceText;
    [SerializeField] private Animator[] buttonAnimators;
    private Queue<QuizItem> quizItems;
    private QuizItem currentItem;
    private bool isRightOrWrong = false;
    public int quizScore = 0;

    public QuizLevel1 quizManager;
    public Button nextButton;

    new void Awake(){
        if(Instance == null){
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }

        quizItems = new Queue<QuizItem>();
    }

    public override void startDialogue(Conversation convoToLoad){
        //do nothing
    }

    public void startQuiz(Quiz quizToLoad, int questionCount){
        dialogueCanvas.gameObject.SetActive(true);

        List<QuizItem> quizRnd = new List<QuizItem>();
        foreach(QuizItem qq in quizToLoad.quizBlocks){
            quizRnd.Add(qq);
        }
        quizRnd.Shuffle();

        for(int i = 0; i < questionCount; i++){
            quizItems.Enqueue(quizRnd[i]);
        }

        Debug.Log(quizItems.Count);

        dialogueBoxAnimator.SetBool("isOpen", true);
        panelAnimator.SetBool("isOpen", true);

        loadQuizItem();
    }
    
    public void loadQuizItem(){
        //check if there's another block in the queue
        //if yes, end dialogue
        //if no, load the next one
        if(quizItems.Count == 0){
            //send broadcast
            quizManager.BroadcastMessage("QuizEnd");

            endDialogue();
            return;
        }

        currentItem = quizItems.Dequeue();

        nameText.text = currentItem.npcName;

        if(currentItem.hasChoice){
            for(int i = 0; i < buttonAnimators.Length; i++){
                buttonAnimators[i].SetBool("isOpen", true);
                choiceText[i].text = currentItem.choices[i];
            }
        } else {
            buttonAnimators[0].SetBool("isOpen", false);
            buttonAnimators[1].SetBool("isOpen", false);
            buttonAnimators[2].SetBool("isOpen", false);
        }

        dialogueText.text = CodeColorizer.Colorize(currentItem.question, false, theme);
    }

    public void loadCorrect(){
        buttonAnimators[0].SetBool("isOpen", false);
        buttonAnimators[1].SetBool("isOpen", false);
        buttonAnimators[2].SetBool("isOpen", false);

        dialogueText.text = "That is the correct answer.";
        isRightOrWrong = true;
    }

    public void loadIncorrect(){
        buttonAnimators[0].SetBool("isOpen", false);
        buttonAnimators[1].SetBool("isOpen", false);
        buttonAnimators[2].SetBool("isOpen", false);

        dialogueText.text = "That is incorrect.";
        isRightOrWrong = true;
    }

    char letter(){
        switch(currentItem.correctAnswer){
            case Letters.A:
                return 'A';
            case Letters.B:
                return 'B';
            case Letters.C:
                return 'C';
            default:
                return 'x';
        }
    }

    public void clickA(){
        checkAnswer(Letters.A);
    }
    
    public void clickB(){
        checkAnswer(Letters.B);
    }

    public void clickC(){
        checkAnswer(Letters.C);
    }

    public void checkAnswer(Letters answer){
        if(answer == currentItem.correctAnswer){
            quizScore++;
            loadCorrect();
        } else {
            loadIncorrect();
        }
    }

    public override void nextLine() {
        if(currentItem.hasChoice && !isRightOrWrong){
            Debug.Log("Answer the question to proceed");
            return;
        }

        isRightOrWrong = false;
        loadQuizItem();
    }

    public void OnPointerClick(PointerEventData eventData){
        nextButton.onClick.Invoke();
    }
}
