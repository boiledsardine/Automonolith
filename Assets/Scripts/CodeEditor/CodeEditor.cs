using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using CodeEditorComponents;

public class CodeEditor : MonoBehaviour{
    const string tabString = "  ";

    //please for the love of god don't add $ or ^ or `
    //those chars are used for special purposes
    const string legalChars = "abcdefghijklmnopqrstuvwxyz 0.123456789+-/*=%&|,<>()[]{};'\"!`";

    float repeatSpeed = 0.1f;
    float keyDownDelay = 0.5f;
    public TMPro.TMP_Text editorMainText;
    public TMPro.TMP_Text editorLineNumbers;
    public TMPro.TMP_Text editorLineDividers;
    public SetEditorText defaultText;
    public Image caret;
    [TextArea(3,10)] public string code;
    public int lineIndex = 0;
    public int charIndex = 0;
    public float caretBlinkRate = 1;
    private float lastInputTime;
    private float caretBlinkTimer;
    List<KeyCode> registeredKeys = new List<KeyCode>();
    Dictionary<KeyCode, KeyState> keys = new Dictionary<KeyCode, KeyState>();
    public ColorizerTheme theme;
    public ScrollRect scrollView;
    bool keyPressed = false;
    public bool takeInputs = false;

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

        RegisterKey(KeyCode.Tab);
        RegisterKey(KeyCode.LeftArrow);
        RegisterKey(KeyCode.RightArrow);
        RegisterKey(KeyCode.UpArrow);
        RegisterKey(KeyCode.DownArrow);
    }

    void RegisterKey(KeyCode key){
        if(!registeredKeys.Contains(key)){
            registeredKeys.Add(key);
            KeyState state = new KeyState(key);
            keys.Add(key, state);
        }
    }

    private void Update(){
        keyPressed = false;
        if(editorMainText.transform.gameObject.activeInHierarchy && takeInputs){

            KeyPress();
            SetLineNumbers();
            SetCaret();

            if(keyPressed){
                scrollView.FocusOnItem(caret.rectTransform);
            }

            editorMainText.text = Colorize(code);
        }
    }

    private void KeyPress(){
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

                    //automatically add closing braces/quotes
                    /*
                    if(!(c == '(' || c == '{' || c == '[' || c == '\"' || c == '\'')){
                        return;
                    }

                    codeLines = code.Split('\n');
                    currentLine = codeLines[lineIndex];

                    if(c == '('){
                        codeLines[lineIndex] = currentLine.Insert(charIndex, ")");
                    } else if(c == '{'){
                        codeLines[lineIndex] = currentLine.Insert(charIndex, "}");
                    } else if(c == '['){
                        codeLines[lineIndex] = currentLine.Insert(charIndex, "]");
                    } else if(c == '\"'){
                        codeLines[lineIndex] = currentLine.Insert(charIndex, "\"");
                    } else if(c == '\''){
                        codeLines[lineIndex] = currentLine.Insert(charIndex, "\'");
                    }

                    code = "";
                    foreach(string s in codeLines){
                        code += s + "\n";
                    }
                    code = code.Substring(0, code.Length - 1);
                    */
                    keyPressed = true;
                }
                //return/enter
                else if(c == '\n' || c == '\r'){
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
                    keyPressed = true;
                }
                //backspace
                else if(c == '\b'){
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
                    keyPressed = true;
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
            keyPressed = true;
        }

        if(GetKeyPress(KeyCode.Tab)){
            lastInputTime = Time.time;
            string[] codeLines = code.Split('\n');

            string currentLine = codeLines[lineIndex];

            codeLines[lineIndex] = currentLine.Insert(charIndex, "  ");
            charIndex += 2;
        
            code = "";
            foreach(string s in codeLines){
                code += s + "\n";
            }
            code = code.Substring(0, code.Length - 1);
            keyPressed = true;
        }

        if(GetKeyPress(KeyCode.LeftArrow)){
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
            keyPressed = true;
        }

        if(GetKeyPress(KeyCode.RightArrow)){
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
            keyPressed = true;
        }

        if(GetKeyPress(KeyCode.UpArrow)){
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
            keyPressed = true;
        }

        if(GetKeyPress(KeyCode.DownArrow)){
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
            keyPressed = true;
        }

        if(Input.GetKeyDown(KeyCode.Home)){
            charIndex = 0;
            keyPressed = true;
        }

        if(Input.GetKeyDown(KeyCode.End)){
            string[] codeLines = code.Split('\n');
            string currentLine = codeLines[lineIndex];
            int lineMaxIndex = currentLine.Length;

            charIndex = lineMaxIndex;
            keyPressed = true;
        }
    }

    bool GetKeyPress(KeyCode key){
        if(Input.GetKey(key)){
            KeyState state = keys[key];

            if(Input.GetKeyDown(key)){
                state.lastPressTime = Time.time;
                return true;
            }

            float timeSinceLastPress = Time.time - state.lastPressTime;
            if(timeSinceLastPress > keyDownDelay){
                float timeSinceLastTick = Time.time - state.lastPressTime;

                if(timeSinceLastTick > repeatSpeed){
                    state.lastTickTime = Time.time;
                    return true;
                }
            }
        }
        return false;
    }

    private void SetLineNumbers(){
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

    private void SetCaret(){
        //caret blinking
        caretBlinkTimer += Time.deltaTime;
        if(Time.time - lastInputTime < caretBlinkRate / 2){
            caret.enabled = true;
            caretBlinkTimer = 0;
        } else {
            caret.enabled = (caretBlinkTimer % caretBlinkRate < caretBlinkRate / 2);
        }

        //get line height, char width, and vertical/horizontal gaps
        editorMainText.text = "N";
        float charWidth = editorMainText.preferredWidth;
        float lineHeight = editorMainText.preferredHeight;

        editorMainText.text = "N\nN";
        float lineGapHeight = editorMainText.preferredHeight - (lineHeight * 2);

        editorMainText.text = "NN";
        float charGapWidth = editorMainText.preferredWidth - (charWidth * 2);

        //place caret
        var caretRect = caret.rectTransform.rect;
        caret.rectTransform.position = editorMainText.rectTransform.position;
        caret.rectTransform.localPosition += Vector3.up * (caretRect.height / 2);
        caret.rectTransform.localPosition += Vector3.left * ((caretRect.width / 2) - charGapWidth);
        caret.rectTransform.localPosition += Vector3.down * (((lineHeight + lineGapHeight) * lineIndex) + lineHeight);
        caret.rectTransform.localPosition += Vector3.right * (((charWidth + charGapWidth) * charIndex) + charWidth);
    }

    public string Colorize(string code){
        string colorizedCode = "";

        string[] lines = code.Split('\n');
        for(int i = 0; i < lines.Length; i++){
            colorizedCode += ColorizeCode(lines[i]);
            if(i != lines.Length - 1){
                colorizedCode += '\n';
            }
        }

        return colorizedCode;
    }

    public string ColorizeCode(string line){
        List<Color> colors = new List<Color>();
        List<int> stringStartIndices = new List<int>();
        List<int> stringEndIndices = new List<int>();

        string[] sections = CodeFormatter.Format(line).Split(' ');
        int nonSpace = 0;
        bool isComment = false;

        for(int i = 0; i < sections.Length; i++){
            Color color = Color.clear;
            string section = sections[i];

            if(section == "//" || isComment){
                color = theme.commentColor;
                isComment = true;
            } else if(section == "if" || section == "else"){
                color = theme.conditionalColor;
            } else if(section == "while"){
                color = theme.loopColor;
            } else if(ReservedConstants.allOperatorsArr.Contains(section)){
                color = theme.operatorColor;
            } else if(section == "(" || section == ")"){
                color = theme.braceColor1;
            } else if(section == "{" || section == "}"){
                color = theme.braceColor2;
            } else if(section == "[" || section == "]"){
                color = theme.braceColor3;
            } else if(section.Contains("\"") || section.Contains("\'")){
                color = theme.stringColor;
            } else if(int.TryParse(section, out _) || float.TryParse(section.Replace('.', ','), out _)){
                color = theme.numColor;
            } else if(ReservedConstants.keywords.Contains(section)){
                color = theme.keywordColor;  
            } else if(section == "Bot"){
                color = theme.botColor;
            } else if(FunctionHandler.builtInFunctions.Contains(section)){
                color = theme.funcColor;
            } else {
                color = theme.varColor;
            }

            if (color != Color.clear) {
                colors.Add (color);
                int endIndex = nonSpace + sections[i].Length;
                stringStartIndices.Add(nonSpace);
                stringEndIndices.Add(endIndex);
            }
            nonSpace += sections[i].Length;
        }

        if(colors.Count > 0){
            nonSpace = 0;
            int colorIndex = 0;
            int startIndex = -1;

            for(int i = 0; i <= line.Length; i++){
                if(nonSpace == stringStartIndices[colorIndex]){
                    startIndex = i;
                } else if(nonSpace == stringEndIndices[colorIndex]){
                    stringStartIndices[colorIndex] = startIndex;
                    stringEndIndices[colorIndex] = i;

                    colorIndex++;
                    if(colorIndex >= colors.Count){
                        break;
                    }
                    i--;
                    continue;
                }

                if(i < line.Length){
                    char c = line[i];
                    if(c != ' '){
                        nonSpace++;
                    }
                }
            }
        }

        for(int i = colors.Count - 1; i >= 0; i--){
            var col = colors[i];
            string colorString = ColorUtility.ToHtmlStringRGB(col);

            line = line.Insert(stringEndIndices[i], "</color>");
            line = line.Insert(stringStartIndices[i], "<color=#" + colorString + ">");
        }

        return line;
    }
}

public class KeyState{
    public KeyState(KeyCode key){
        this.key = key;
    }
    public KeyCode key;
    public float lastTickTime, lastPressTime;
}