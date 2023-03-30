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

    void Awake(){
        if(Instance == null){
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    public override void startDialogue(int npcBlockIndex, int blockLineIndex){
        dialogueBoxAnimator.SetBool("isOpen", true);
        panelAnimator.SetBool("isOpen", true);
        
        quizItem = npcQuizBlocks[npcBlockIndex];
        currentBlock = npcBlockIndex;

        Debug.Log("Starting conversation with " + quizItem.name);
        dialogueLines.Clear();

        if(quizItem.showNpc){
            RawImage rawImageLeft = leftSprite.GetComponent<RawImage>();
            RawImage rawImageRight = rightSprite.GetComponent<RawImage>();
            switch(quizItem.npcPos){
                case 'L':
                    rawImageLeft.enabled = true;
                    rawImageLeft.texture = quizItem.npcSprite;
                    rawImageLeft.color = Color.white;
                    rawImageRight.color = Color.gray;
                    break;
                case 'R':
                    rawImageRight.enabled = true;
                    rawImageRight.texture = quizItem.npcSprite;
                    rawImageRight.color = Color.white;
                    rawImageLeft.color = Color.gray;
                    break;
            }
        } else {
            leftSprite.GetComponent<RawImage>().color = Color.gray;
            rightSprite.GetComponent<RawImage>().color = Color.gray;
        }

        nameText.text = quizItem.name;

        foreach(string s in quizItem.lines){
            dialogueLines.Add(s);
        }

        string line = dialogueLines[blockLineIndex];
        currentLine = blockLineIndex;
        StopAllCoroutines();
        StartCoroutine(typeSentence(line));
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
            if(currentBlock + 1 != npcQuizBlocks.Length){
                startDialogue(currentBlock + 1, 0);
            } else {
                currentLine = dialogueLines.Count - 1;
                endDialogue();
                return;
            }
        }

        string line = dialogueLines[currentLine];
        StopAllCoroutines();
        StartCoroutine(typeSentence(line));

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
