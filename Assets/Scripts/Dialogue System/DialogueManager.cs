using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : DialogueSystemBase{
    public static DialogueManager Instance;
    
    [SerializeField] protected Dialogue[] npcDialogueBlocks;

    void Awake(){
        if(Instance == null){
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    public override void startDialogue(int npcBlockIndex, int blockLineIndex){
        dialogueBoxAnimator.SetBool("isOpen", true);
        
        Dialogue dialogue = npcDialogueBlocks[npcBlockIndex];
        currentBlock = npcBlockIndex;

        Debug.Log("Starting conversation with " + dialogue.name);
        dialogueLines.Clear();

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
            }
        } else {
            leftSprite.GetComponent<RawImage>().color = Color.gray;
            rightSprite.GetComponent<RawImage>().color = Color.gray;
        }

        nameText.text = dialogue.name;

        foreach(string s in dialogue.lines){
            dialogueLines.Add(s);
        }

        string line = dialogueLines[blockLineIndex];
        currentLine = blockLineIndex;
        StopAllCoroutines();
        StartCoroutine(typeSentence(line));
    }

    public override void nextLine(){
        if(currentLine != dialogueLines.Count){
            currentLine++;
        }

        if(currentLine == dialogueLines.Count){
            if(currentBlock + 1 != npcDialogueBlocks.Length){
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
    }
}
