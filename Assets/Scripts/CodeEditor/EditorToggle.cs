using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorToggle : MonoBehaviour{
    public Canvas editorCanvas;
    public Animator panelAnimator;

    public void openEditor(){
        editorCanvas.gameObject.SetActive(true);
        panelAnimator.SetBool("isOpen", true);
    }

    public void closeEditor(){
        if(!editorCanvas.gameObject.activeSelf || !panelAnimator.GetBool("isOpen")){
            return;
        }

        panelAnimator.SetBool("isOpen", false);
        Invoke("disableEditor", 0.25f);
    }

    public void disableEditor(){
        editorCanvas.gameObject.SetActive(false);
    }
}