using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorToggle : MonoBehaviour{
    public CodeEditor editor;

    public void enableEditor(){
        editor.transform.gameObject.SetActive(true);
    }

    public void disableEditor(){
        editor.transform.gameObject.SetActive(false);
    }
}
