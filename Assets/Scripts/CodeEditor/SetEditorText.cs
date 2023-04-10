using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CodeEditorComponents{
    public class SetEditorText : MonoBehaviour {
    public TMPro.TMP_Text textField;

    [Multiline (10)]
    public string defaultInput;

    void Start (){
            textField.text = defaultInput;
        }
    }
}