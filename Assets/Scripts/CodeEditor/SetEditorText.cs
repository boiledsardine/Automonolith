using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CodeEditorComponents{

public class SetEditorText : MonoBehaviour {
    public TMPro.TMP_Text textField;
    public CodeEditor editor;

    [TextArea(10,30)]
    public string defaultInput;

    void Awake (){
        textField.text = defaultInput;
        editor.code = defaultInput;
    }
}

}