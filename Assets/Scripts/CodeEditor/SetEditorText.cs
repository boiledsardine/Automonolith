using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CodeEditorComponents{
    public class SetEditorText : MonoBehaviour {
    public InputField inputField;

    [Multiline (10)]
    public string defaultInput;

    void Start (){
            inputField.text = defaultInput;
        }
    }
}