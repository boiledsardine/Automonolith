using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using CodeEditorComponents;

public class CodeEditor : MonoBehaviour{
    const string indentString = "  ";

    //please for the love of god don't add $ or ^ or `
    //those chars are used for special purposes
    const string legalChars = "abcdefghijklmnopqrstuvwxyz 0.123456789+-/*=%&|,<>()[]{};'\"!`";

    public TMPro.TMP_Text editorMainText;
    public TMPro.TMP_Text editorLineNumbers;
    public TMPro.TMP_Text editorLineDividers;
    public SetEditorText defaultText;
    public Image caret;
    [TextArea(3,10)] public string code;
    public int lineIndex = 0;
    public int charIndex = 0;
    public float caretBlinkRate = 1;
    public SpecialKeys specialKeys;
    private float lastInputTime;
    private float caretBlinkTimer;

    private void Start(){
        code = editorMainText.text;

        if(string.IsNullOrEmpty(code)){
            code = "";
        }

        string[] codeLines = code.Split('\n');
        string finalLine = codeLines[codeLines.Length - 1];
        int lineMaxIndex = finalLine.Length;

        lineIndex = codeLines.Length - 1;
        charIndex = lineMaxIndex;

        specialKeys.registerSpecialKey(KeyCode.Backspace);
        specialKeys.registerSpecialKey(KeyCode.LeftArrow);
        specialKeys.registerSpecialKey(KeyCode.RightArrow);
        specialKeys.registerSpecialKey(KeyCode.UpArrow);
        specialKeys.registerSpecialKey(KeyCode.DownArrow);
    }

    private void LateUpdate(){
        if(gameObject.activeSelf){
            keyPress();
            setLineNumbers();
            setCaret();

            editorMainText.text = code;
        }
    }

    private void keyPress(){
        //triggers on legal character entry
        //or for a ctrl+C or ctrl+R combo
        if(!Input.GetKey(KeyCode.LeftControl)) {
            foreach(char c in Input.inputString){
                if(legalChars.Contains(c.ToString().ToLower())){
                    lastInputTime = Time.time;
                    string[] codeLines = code.Split('\n');

                    string currentLine = codeLines[lineIndex];

                    codeLines[lineIndex] = currentLine.Insert(charIndex, c.ToString());
                    charIndex++;
                
                    code = "";
                    foreach(string s in codeLines){
                        code += s + "\n";
                    }
                    code = code.Substring(0, code.Length - 1);
                }
            }
        } else {
            if(Input.GetKey(KeyCode.C)){
                Compiler.Instance.Run();
                EditorToggle toggle = gameObject.GetComponent<EditorToggle>();
                toggle.closeEditor();
            } else if(Input.GetKey(KeyCode.R)){
                code = defaultText.defaultInput;

                string[] codeLines = code.Split('\n');
                string finalLine = codeLines[codeLines.Length - 1];
                int lineMaxIndex = finalLine.Length;

                lineIndex = codeLines.Length - 1;
                charIndex = lineMaxIndex;
            }
        }

        //enter button
        if(Input.GetKeyDown(KeyCode.Return)){
            lastInputTime = Time.time;
            
            List<string> codeLines = code.Split('\n').ToList<string>();
            string currentLine = codeLines[lineIndex];
            int lineMaxIndex = currentLine.Length;

            if(charIndex == lineMaxIndex){
                codeLines[lineIndex] += "\n";
            } else {
                string lastLine = currentLine.Substring(0, charIndex);
                string nextLine = currentLine.Substring(charIndex, lineMaxIndex - charIndex);
                codeLines.Insert(lineIndex, lastLine);
                codeLines[lineIndex + 1] = nextLine;
            }
            
            charIndex = 0;
            lineIndex++;

            code = "";
            foreach(string s in codeLines){
                code += s + "\n";
            }
            code = code.Substring(0, code.Length - 1);
        }

        if(specialKeys.getKeyPress(KeyCode.Backspace)){
            if(code.Length == 0 || (charIndex == 0 && lineIndex == 0)){
                return;
            }
            
            lastInputTime = Time.time;
            List<string> codeLines = code.Split('\n').ToList<string>();
            string currentLine = codeLines[lineIndex];

            if(charIndex == 0 && lineIndex > 0){
                string lastLine = codeLines[lineIndex - 1];
                codeLines[lineIndex - 1] = lastLine + currentLine;
                codeLines.RemoveAt(lineIndex);
                charIndex = lastLine.Length;
                lineIndex--;
            } else {
                string start = currentLine.Substring(0, charIndex - 1);
                string end = currentLine.Substring(charIndex, currentLine.Length - charIndex);
                codeLines[lineIndex] = start + end;
                charIndex--;
            }

            code = "";
            foreach(string s in codeLines){
                code += s + "\n";
            }
            code = code.Substring(0, code.Length - 1);
        }

        if(specialKeys.getKeyPress(KeyCode.LeftArrow)){
            if(code.Length > 0){
                lastInputTime = Time.time;
                if(charIndex > 0){
                    charIndex = Mathf.Max(0, charIndex - 1);
                }

                //check for line above
                if(charIndex == 0 && lineIndex > 0){
                    lineIndex--;

                    string[] codeLines = code.Split('\n');
                    string currentLine = codeLines[lineIndex];
                    int lineMaxIndex = currentLine.Length;
                    
                    charIndex = lineMaxIndex;
                }
            }
        }

        if(specialKeys.getKeyPress(KeyCode.RightArrow)){
            if(code.Length > 0){
                lastInputTime = Time.time;
                string[] codeLines = code.Split('\n');
                string currentLine = codeLines[lineIndex];
                int lineMaxIndex = currentLine.Length;

                if(charIndex < lineMaxIndex){
                    charIndex = Mathf.Min(lineMaxIndex, charIndex + 1);
                }

                //check for line below
                if(charIndex == lineMaxIndex && lineIndex < codeLines.Length - 1){
                    lineIndex++;

                    codeLines = code.Split('\n');
                    currentLine = codeLines[lineIndex];
                    lineMaxIndex = currentLine.Length;
                    
                    charIndex = 0;
                }
            }
        }

        if(specialKeys.getKeyPress(KeyCode.UpArrow)){
            if(lineIndex > 0){
                lastInputTime = Time.time;
                string[] codeLines = code.Split('\n');
                string previousLine = codeLines[lineIndex - 1];
                int lineMaxIndex = previousLine.Length;
                
                if(charIndex > lineMaxIndex){
                    charIndex = lineMaxIndex;
                }
                
                lineIndex--;
            }
        }

        if(specialKeys.getKeyPress(KeyCode.DownArrow)){
            string[] codeLines = code.Split('\n');

            if(lineIndex < codeLines.Length - 1){
                lastInputTime = Time.time;
                string nextLine = codeLines[lineIndex + 1];
                int lineMaxIndex = nextLine.Length;

                if(charIndex > lineMaxIndex){
                    charIndex = lineMaxIndex;
                }

                lineIndex++;
            }
        }

        if(Input.GetKeyDown(KeyCode.Home)){
            charIndex = 0;
        }

        if(Input.GetKeyDown(KeyCode.End)){
            string[] codeLines = code.Split('\n');
            string currentLine = codeLines[lineIndex];
            int lineMaxIndex = currentLine.Length;

            charIndex = lineMaxIndex;
        }
    }

    private void setLineNumbers(){
        string numbers = "";
        string dividers = "";

        int numLines = code.Split('\n').Length;
        for(int i = 0; i < numLines; i++){
            numbers += (i + 1) + "\n";
            dividers += "|\n";
        }
        
        editorLineNumbers.text = numbers;
        editorLineDividers.text = dividers;
    }

    private void setCaret(){
        caretBlinkTimer += Time.deltaTime;
        if(Time.time - lastInputTime < caretBlinkRate / 2){
            caret.enabled = true;
            caretBlinkTimer = 0;
        } else {
            caret.enabled = (caretBlinkTimer % caretBlinkRate < caretBlinkRate / 2);
        }

        editorMainText.text = "N";
        float charWidth = editorMainText.preferredWidth;
        float lineHeight = editorMainText.preferredHeight;

        editorMainText.text = "N\nN";
        float lineGapHeight = editorMainText.preferredHeight - (lineHeight * 2);

        editorMainText.text = "NN";
        float charGapWidth = editorMainText.preferredWidth - (charWidth * 2);

        var caretRect = caret.rectTransform.rect;
        caret.rectTransform.position = editorMainText.rectTransform.position;
        caret.rectTransform.localPosition += Vector3.up * (caretRect.height / 2);
        caret.rectTransform.localPosition += Vector3.left * ((caretRect.width / 2) - charGapWidth);
        caret.rectTransform.localPosition += Vector3.down * (((lineHeight + lineGapHeight) * lineIndex) + lineHeight);
        caret.rectTransform.localPosition += Vector3.right * (((charWidth + charGapWidth) * charIndex) + charWidth);
    }
}