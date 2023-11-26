using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorToggle : MonoBehaviour{
    public Canvas editorCanvas, dialogueCanvas;
    public Animator panelAnimator, controlAnimator;
    CodeEditor editor;
    public float invokeTime = 0.1f;

    void Start(){
        editor = gameObject.GetComponent<CodeEditor>();
    }

    public void openEditor(){
        if(editorCanvas.gameObject.activeInHierarchy || dialogueCanvas.gameObject.activeInHierarchy){
            return;
        }

        editorCanvas.gameObject.SetActive(true);
        panelAnimator.SetBool("isOpen", true);

        SendMessage("PlayOpenSound");

        Invoke("EnableInputs", invokeTime);
    }

    public void closeEditor(){
        if(!editorCanvas.gameObject.activeSelf || !panelAnimator.GetBool("isOpen") || dialogueCanvas.gameObject.activeInHierarchy){
            return;
        }

        editor.takeInputs = false;

        panelAnimator.SetBool("isOpen", false);
        controlAnimator.SetBool("isOpen", false);

        SendMessage("PlayCloseSound");

        Invoke("disableEditor", 0.25f);
    }

    public void ToggleControls(){
        if(!controlAnimator.GetBool("isOpen")){
            controlAnimator.SetBool("isOpen", true);
        } else {
            controlAnimator.SetBool("isOpen", false);
        }
    }

    public void EnableInputs(){
        editor.takeInputs = true;
    }

    public void disableEditor(){
        editorCanvas.gameObject.SetActive(false);
    }
}