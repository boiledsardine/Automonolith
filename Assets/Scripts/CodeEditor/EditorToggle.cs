using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorToggle : MonoBehaviour{
    public Canvas editorCanvas;
    public Animator panelAnimator, controlAnimator;
    CodeEditor editor;
    public float invokeTime = 0.1f;

    void Start(){
        editor = gameObject.GetComponent<CodeEditor>();
    }

    public void openEditor(){
        editorCanvas.gameObject.SetActive(true);
        panelAnimator.SetBool("isOpen", true);
        Invoke("EnableInputs", invokeTime);
    }

    public void closeEditor(){
        editor.takeInputs = false;

        if(!editorCanvas.gameObject.activeSelf || !panelAnimator.GetBool("isOpen")){
            return;
        }

        panelAnimator.SetBool("isOpen", false);
        controlAnimator.SetBool("isOpen", false);
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