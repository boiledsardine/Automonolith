using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class QuizManager : DialogueSystemBase, IPointerClickHandler{
    public static QuizManager Instance;

    [SerializeField] private Text[] choiceText;
    [SerializeField] private Animator[] buttonAnimators;
    [SerializeField] private Button buttonA, buttonB, buttonC;
    [SerializeField] Color wrongColor, correctColor;
    private Queue<QuizItem> quizItems;
    private QuizItem currentItem;
    private bool isQuestion;
    public int quizScore = 0;

    public QuizLevel1 quizManager;
    public Button nextButton;
    public TMPro.TMP_Text numText;
    AudioSource source;

    new void Awake(){
        if(Instance == null){
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }

        quizItems = new Queue<QuizItem>();
        source = GetComponent<AudioSource>();
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
        PlayOpenSound();

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
        isQuestion = true;
        PlayQuestionAskSound();
        ToggleButtons(true);
        ResetButtonColor();

        //hide right button and show numtext
        nextButton.GetComponent<Image>().enabled = false;
        numText.transform.gameObject.SetActive(true);
        int remaining = 10 - quizItems.Count;
        numText.text = remaining.ToString() + "/10";

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
        
        //show right button and hide numtext
        nextButton.GetComponent<Image>().enabled = true;
        numText.transform.gameObject.SetActive(false);
    }

    public void loadIncorrect(){
        buttonAnimators[0].SetBool("isOpen", false);
        buttonAnimators[1].SetBool("isOpen", false);
        buttonAnimators[2].SetBool("isOpen", false);

        dialogueText.text = "That is incorrect.";

        //show right button and hide numtext
        nextButton.GetComponent<Image>().enabled = true;
        numText.transform.gameObject.SetActive(false);
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
        Image buttonImage = buttonA.GetComponent<Image>();
        if(checkAnswer(Letters.A)){
            buttonImage.color = correctColor;
        } else {
            buttonImage.color = wrongColor;
        }
    }
    
    public void clickB(){
        Image buttonImage = buttonB.GetComponent<Image>();
        if(checkAnswer(Letters.B)){
            buttonImage.color = correctColor;
        } else {
            buttonImage.color = wrongColor;
        }
    }

    public void clickC(){
        Image buttonImage = buttonC.GetComponent<Image>();
        if(checkAnswer(Letters.C)){
            buttonImage.color = correctColor;
        } else {
            buttonImage.color = wrongColor;
        }
    }

    void ToggleButtons(bool interactable){
        buttonA.interactable = interactable;
        buttonB.interactable = interactable;
        buttonC.interactable = interactable;
    }

    void ResetButtonColor(){
        Image buttonImageA = buttonA.GetComponent<Image>();
        Image buttonImageB = buttonB.GetComponent<Image>();
        Image buttonImageC = buttonC.GetComponent<Image>();
        
        buttonImageA.color = Color.white;
        buttonImageB.color = Color.white;
        buttonImageC.color = Color.white;
    }

    public bool checkAnswer(Letters answer){
        ToggleButtons(false);
        isQuestion = false;
        Invoke(nameof(loadQuizItem), 1.5f);

        if(answer == currentItem.correctAnswer){
            quizScore++;
            PlayCorrectSound();
            return true;
        } else {
            PlayWrongSound();
            return false;
        }
    }

    void PlayCorrectSound(){
        source.volume = GlobalSettings.Instance.sfxVolume;
        source.clip = AudioPicker.Instance.correctAnswer;
        source.Play();
    }

    void PlayWrongSound(){
        source.volume = GlobalSettings.Instance.sfxVolume;
        source.clip = AudioPicker.Instance.wrongAnswer;
        source.Play();
    }

    void PlayNextLineLockSound(){
        source.volume = GlobalSettings.Instance.sfxVolume;
        source.clip = AudioPicker.Instance.nextLocked;
        source.Play();
    }

    void PlayQuestionAskSound(){
        source.volume = GlobalSettings.Instance.sfxVolume;
        System.Random rnd = new System.Random();
        int maxIndex = AudioPicker.Instance.question.Length;
        source.clip = AudioPicker.Instance.question[rnd.Next(maxIndex)];
        source.Play();
    }

    public override void nextLine() {
        if(currentItem.hasChoice && isQuestion){
            Debug.Log("Answer the question to proceed");

            PlayNextLineLockSound();

            return;
        }

        CancelInvoke(nameof(loadQuizItem));
        loadQuizItem();
    }

    public void OnPointerClick(PointerEventData eventData){
        nextButton.onClick.Invoke();
    }
}
