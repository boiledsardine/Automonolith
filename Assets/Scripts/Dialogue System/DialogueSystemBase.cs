using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class DialogueSystemBase : MonoBehaviour{
    [SerializeField] protected Text nameText;
    [SerializeField] protected Text dialogueText;

    protected List<string> dialogueLines;
    [SerializeField] protected Animator dialogueBoxAnimator;
    [SerializeField] protected RawImage leftSprite;
    [SerializeField] protected RawImage rightSprite;
    protected int currentLine = -1;
    protected int currentBlock = -1;

    public void Start(){
        dialogueLines = new List<string>();
    }

    public abstract void startDialogue(int npcBlockIndex, int blockLineIndex);

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
        leftSprite.GetComponent<RawImage>().enabled = false;
        rightSprite.GetComponent<RawImage>().enabled = false;
        Debug.Log("End of conversation");
    }
}
