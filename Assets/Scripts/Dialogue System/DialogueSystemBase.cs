using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class DialogueSystemBase : MonoBehaviour{
    [SerializeField] protected TMPro.TMP_Text nameText;
    [SerializeField] protected TMPro.TMP_Text dialogueText;

    protected Queue<string> dialogueLines;
    
    [SerializeField] protected Animator panelAnimator;
    [SerializeField] protected Animator dialogueBoxAnimator;
    [SerializeField] protected RawImage leftSprite;
    [SerializeField] protected RawImage rightSprite;
    protected int currentLine = -1;
    protected int currentBlock = -1;
    protected int stopIndex;

    public void Awake(){
        dialogueLines = new Queue<string>();
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
        leftSprite.GetComponent<RawImage>().enabled = false;
        rightSprite.GetComponent<RawImage>().enabled = false;
        Debug.Log("End of conversation");
        Invoke("disableCanvas", 0.25f);
    }

    public void disableCanvas(){
        gameObject.transform.parent.gameObject.SetActive(false);
    }

    public void npcHighlight(Dialogue dialogue){
        if(dialogue.showNpc){
            RawImage rawImageLeft = leftSprite.GetComponent<RawImage>();
            RawImage rawImageRight = rightSprite.GetComponent<RawImage>();
            switch(dialogue.npcPos){
                case 'L':
                    rawImageLeft.enabled = true;
                    rawImageLeft.texture = dialogue.npcSprite;
                    rawImageLeft.color = Color.white;
                    rawImageRight.color = Color.gray;
                    break;
                case 'R':
                    rawImageRight.enabled = true;
                    rawImageRight.texture = dialogue.npcSprite;
                    rawImageRight.color = Color.white;
                    rawImageLeft.color = Color.gray;
                    break;
                case 'B':
                    rawImageLeft.color = Color.white;
                    rawImageRight.color = Color.white;
                    break;
                case 'C':
                    rawImageLeft.enabled = false;
                    rawImageRight.enabled = false;
                    break;
            }
        } else {
            leftSprite.GetComponent<RawImage>().color = Color.gray;
            rightSprite.GetComponent<RawImage>().color = Color.gray;
        }
    }
}
