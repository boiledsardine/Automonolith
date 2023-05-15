using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuizManager : DialogueSystemBase{
    public static QuizManager Instance;

    [SerializeField] private Text[] choiceText;
    [SerializeField] private Animator[] buttonAnimators;
    [SerializeField] private QuizItem[] npcQuizBlocks;

    private QuizItem quizItem;

    new void Awake(){
        if(Instance == null){
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    public override void startDialogue(Conversation convoToLoad){
        dialogueBoxAnimator.SetBool("isOpen", true);
        panelAnimator.SetBool("isOpen", true);
        

        Debug.Log("Starting conversation with " + quizItem.npcName);
        dialogueLines.Clear();

        npcHighlight(quizItem);

        nameText.text = quizItem.npcName;

        foreach(string s in quizItem.lines){
            //dialogueLines.Add(s);
        }

        //string line = dialogueLines[0];
        currentLine = 0;
        StopAllCoroutines();
        //StartCoroutine(typeSentence(line));
    }

    public void checkIfChoice(){
        if(quizItem.hasChoice){
            Debug.Log("Answer the question to proceed");
            return;
        }
        nextLine();
    }

    public override void nextLine(){
        if(currentLine != dialogueLines.Count){
            currentLine++;
        }

        if(currentLine == dialogueLines.Count){
            if(currentBlock == stopIndex){
                Debug.Log("Current: " + currentBlock + "; Stopping at: " + stopIndex);
                endDialogue();
                return;
            } else if(currentBlock == npcQuizBlocks.Length - 1){
                currentLine = dialogueLines.Count - 1;
                endDialogue();
                return;
            } else {
                
                return;
            }
        }

        //string line = dialogueLines[currentLine];
        StopAllCoroutines();
        //StartCoroutine(typeSentence(line));

        //checks if current item has a choice
        //if it does, opens the buttons and sets their text
        //if it doesn't, closes any open buttons
        if(quizItem.hasChoice){
            for(int i = 0; i < buttonAnimators.Length; i++){
                buttonAnimators[i].SetBool("isOpen", true);
                choiceText[i].text = quizItem.choices[i];
            }
        } else {
            buttonAnimators[0].SetBool("isOpen", false);
            buttonAnimators[1].SetBool("isOpen", false);
            buttonAnimators[2].SetBool("isOpen", false);
        }
    }

    public void clickA(){
        checkAnswer('A');
    }
    
    public void clickB(){
        checkAnswer('B');
    }

    public void clickC(){
        checkAnswer('C');
    }

    public void checkAnswer(char answer){
        if(answer == quizItem.correctAnswer){
            nextLine();
        } else {
            StopAllCoroutines();
            StartCoroutine(typeSentence("Incorrect!"));
        }
    }
}
