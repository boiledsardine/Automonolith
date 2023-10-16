using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class DialogueSystemBase : MonoBehaviour{
    [SerializeField] protected Canvas dialogueCanvas;
    [SerializeField] protected TMPro.TMP_Text nameText;
    [SerializeField] protected TMPro.TMP_Text dialogueText;

    protected Queue<string> dialogueLines;
    protected Queue<string> quizLines;
    
    [SerializeField] protected Animator panelAnimator;
    [SerializeField] protected Animator dialogueBoxAnimator;
    [SerializeField] protected Image leftSprite;
    [SerializeField] protected Image rightSprite;

    public void Awake(){
        dialogueLines = new Queue<string>();
        quizLines = new Queue<string>();
    }

    public abstract void startDialogue(Conversation convoToLoad);

    public IEnumerator typeSentence(string sentence){
        dialogueText.text = "";
        foreach(char letter in sentence.ToCharArray()){
            dialogueText.text += letter;
            yield return null;
        }
    }

    //navigates to the next line on the list
    public abstract void nextLine();

    public void endDialogue(){
        dialogueBoxAnimator.SetBool("isOpen", false);
        panelAnimator.SetBool("isOpen", false);
        leftSprite.GetComponent<Image>().enabled = false;
        rightSprite.GetComponent<Image>().enabled = false;
        BroadcastMessage("DialogueEnd");
        Invoke("disableCanvas", 0.25f);
    }

    public void enableCanvas(){
        dialogueCanvas.gameObject.SetActive(true);
    }

    public void disableCanvas(){
        dialogueCanvas.gameObject.SetActive(false);
    }

    public void npcHighlight(Dialogue dialogue){
        if(dialogue.showNpc){
            Image ImageLeft = leftSprite.GetComponent<Image>();
            Image ImageRight = rightSprite.GetComponent<Image>();
            switch(dialogue.npcPos){
                case 'L':
                    ImageLeft.enabled = true;
                    ImageLeft.sprite = dialogue.npcSprite;
                    ImageLeft.color = Color.white;
                    ImageRight.color = Color.gray;
                    break;
                case 'R':
                    ImageRight.enabled = true;
                    ImageRight.sprite = dialogue.npcSprite;
                    ImageRight.color = Color.white;
                    ImageLeft.color = Color.gray;
                    break;
                case 'B':
                    ImageLeft.color = Color.white;
                    ImageRight.color = Color.white;
                    break;
                case 'C':
                    ImageLeft.enabled = false;
                    ImageRight.enabled = false;
                    break;
            }
        } else {
            leftSprite.GetComponent<Image>().color = Color.gray;
            rightSprite.GetComponent<Image>().color = Color.gray;
        }
    }
}
