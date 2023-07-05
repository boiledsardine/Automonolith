using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ErrorChecker : MonoBehaviour{
    [SerializeField] public Conversation errorConvo;
    [SerializeField] private Canvas DialogueCanvas;
    [SerializeField] public Sprite errorSprite;

    //possible to alter dialogue to be said
    //do this: errorConvo.dialogueBlocks[0].lines[0] = "whatever here";

    public void writeError(){
        DialogueCanvas.gameObject.SetActive(true);
        DialogueManager.Instance.startDialogue(errorConvo);
    }

    public void unnamedVariableError(int lineIndex, string varType){        
        DialogueCanvas.gameObject.SetActive(true);

        string line = string.Format("Line {0}: Add a variable name after \"{1}\".", lineIndex, varType);
        
        errorConvo.dialogueBlocks[0].lines[0] = line;
        DialogueManager.Instance.startDialogue(errorConvo);
    }

    public void noexistentVariableError(int lineIndex, string varName){
        DialogueCanvas.gameObject.SetActive(true);

        string line = string.Format("Line {0}: {1} is not defined.", lineIndex, varName);

        errorConvo.dialogueBlocks[0].lines[0] = line;
        DialogueManager.Instance.startDialogue(errorConvo);
    }

    public void reservedNameError(int lineIndex){
        DialogueCanvas.gameObject.SetActive(true);

        string line = string.Format("Line {0}: You can't use a variable type as an identifier.", lineIndex);

        errorConvo.dialogueBlocks[0].lines[0] = line;
        DialogueManager.Instance.startDialogue(errorConvo);
    }

    public void variableDefinedError(int lineIndex, string varName){
        DialogueCanvas.gameObject.SetActive(true);

        string line = string.Format("Line {0}: {1} is already defined.", lineIndex, varName);

        errorConvo.dialogueBlocks[0].lines[0] = line;
        DialogueManager.Instance.startDialogue(errorConvo);
    }

    public void nonexistentTypeError(int lineIndex, string varType){
        DialogueCanvas.gameObject.SetActive(true);

        string line = string.Format("Line {0}: {1} is not a valid type.", lineIndex, varType);

        errorConvo.dialogueBlocks[0].lines[0] = line;
        DialogueManager.Instance.startDialogue(errorConvo);
    }

    public void wrongTypeError(int lineIndex, string varType){
        DialogueCanvas.gameObject.SetActive(true);

        string line = string.Format("Line {0}: given value is not a(n) {1}.", lineIndex, varType);

        errorConvo.dialogueBlocks[0].lines[0] = line;
        DialogueManager.Instance.startDialogue(errorConvo);
    }

    public void invalidArgsError(int lineIndex, string funcName, int paramCount, string paramType){
        DialogueCanvas.gameObject.SetActive(true);

        string line = string.Format("Line {0}: {1} takes {2} arguments of type {3}.", lineIndex, funcName, paramCount, paramType);

        errorConvo.dialogueBlocks[0].lines[0] = line;
        DialogueManager.Instance.startDialogue(errorConvo);
    }
}
