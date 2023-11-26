using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using CodeEditorComponents;

public class CodeEditor : MonoBehaviour{
    public static CodeEditor Instance { get; private set; }
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
    public ScrollRect textScroll, methodsScroll;
    bool keyPressed = false;
    public bool takeInputs = false;
    public GameObject methodsPanelViewport, methodsPanelSet;
    AudioSource source;
    System.Random rnd;

    public bool KeyPressed {
        get { return keyPressed; }
        set { keyPressed = value; }
    }

    void Awake(){
        if(Instance == null){
            Instance = this;
        } else {
            Destroy(gameObject);
        }

        source = transform.GetChild(0).GetComponent<AudioSource>();
        rnd = new System.Random();
    }

    void SetTypeSource(){
        source = transform.Find("TypingSource").GetComponent<AudioSource>();

        float globalVolume = GlobalSettings.Instance.sfxVolume;
        float multiplier = AudioPicker.Instance.typingVolume;
        source.volume = globalVolume * multiplier;
    }

    void SetSpecialSource(){
        source = transform.Find("SpecialSource").GetComponent<AudioSource>();
        
        source.volume = GlobalSettings.Instance.sfxVolume;
    }

    void PlayKeySound(){
        SetTypeSource(); //also sets volume

        source.clip = AudioPicker.Instance.keyPress[rnd.Next(3)];
        source.Play();
    }

    void PlaySpecialKeySound(){
        SetTypeSource(); //also sets volume

        source.clip = AudioPicker.Instance.keyPress[rnd.Next(3,5)];
        source.Play();
    }

    void PlayCompileSound(){
        SetSpecialSource(); //also sets volume

        source.clip = AudioPicker.Instance.playClick;
        source.Play();
    }

    void PlayClearSound(){
        SetSpecialSource(); //also sets volume

        source.clip = AudioPicker.Instance.resetEditor;
        source.Play();
    }

    private void Start(){
        var panelSet = Instantiate(methodsPanelSet);
        panelSet.transform.SetParent(methodsPanelViewport.transform);
        panelSet.transform.localScale = new Vector3(1,1,1);
        methodsScroll.content = panelSet.GetComponent<RectTransform>();

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
            EventSystem.current.SetSelectedGameObject(null);

            KeyPress();
            SetLineNumbers();
            SetCaret();

            FocusCaret(keyPressed);

            editorMainText.text = Colorize(code);
        }
    }

    public void FocusCaret(bool go){
        if(!go) return;
        textScroll.FocusOnItem(caret.rectTransform);
    }

    public void AddLine(string input){
        List<string> codeLines = code.Split('\n').ToList();
        string currentLine = codeLines[lineIndex];

        codeLines[lineIndex] = currentLine.Insert(charIndex, input);
        charIndex += input.Length;
        
        currentLine = codeLines[lineIndex];
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

    //scuffed fix?
    /*
        public void AddLine(string input){
        List<string> codeLines = code.Split('\n').ToList();
        string currentLine = codeLines[lineIndex];

        if(string.IsNullOrWhiteSpace(codeLines[lineIndex])){
            codeLines[lineIndex] = currentLine.Insert(charIndex, input);
        } else {
            codeLines[lineIndex] = currentLine.Insert(charIndex, "\n" + input);
            charIndex = 0;
            lineIndex++;
        }
        charIndex += input.Length;
        
        currentLine = codeLines[lineIndex];
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
    */

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
                    keyPressed = true;
                    
                    PlayKeySound();
                }
                //return/enter
                else if(c == '\n' || c == '\r'){
                    lastInputTime = Time.time;            
            
                    List<string> codeLines = code.Split('\n').ToList<string>();
                    string currentLine = codeLines[lineIndex];
                    int lineMaxIndex = currentLine.Length;

                    if(charIndex == lineMaxIndex){
                        codeLines[lineIndex] += "\n";

                        //this is a non-breaking space
                        //kind of a bad fix considering it adds an extra character
                        //and it kinda screws up backspacing large amounts of empty space
                        //plus messes up the line numbering when it's removed from an empty line
                        //codeLines[lineIndex] += "\n\u00A0";
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

                    PlaySpecialKeySound();
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

                    PlayKeySound();
                }
            }
        } else {
            if(Input.GetKey(KeyCode.C)){
                Compiler.Instance.Run();
                EditorToggle toggle = gameObject.GetComponent<EditorToggle>();
                toggle.closeEditor();
                
                //play execute sound
                PlayCompileSound();
            } else if(Input.GetKey(KeyCode.R)){
                code = defaultText.defaultInput;

                string[] codeLines = code.Split('\n');
                string finalLine = codeLines[codeLines.Length - 1];
                int lineMaxIndex = finalLine.Length;

                lineIndex = codeLines.Length - 1;
                charIndex = lineMaxIndex;
                
                //play reset sound
                PlayClearSound();
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

            PlaySpecialKeySound();
        }

        if(GetKeyPress(KeyCode.LeftArrow)){
            if(code.Length > 0){
                lastInputTime = Time.time;
                if(charIndex > 0){
                    charIndex = Mathf.Max(0, charIndex - 1);
                }

                //check for line above
                else if(charIndex == 0 && lineIndex > 0){
                    lineIndex--;

                    string[] codeLines = code.Split('\n');
                    string currentLine = codeLines[lineIndex];
                    int lineMaxIndex = currentLine.Length;
                    
                    charIndex = lineMaxIndex;
                }
            }
            keyPressed = true;

            PlayKeySound();
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
                else if(charIndex == lineMaxIndex && lineIndex < codeLines.Length - 1){
                    lineIndex++;

                    codeLines = code.Split('\n');
                    currentLine = codeLines[lineIndex];
                    lineMaxIndex = currentLine.Length;
                    
                    charIndex = 0;
                }
            }
            keyPressed = true;

            PlayKeySound();
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

            PlayKeySound();
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
            
            PlayKeySound();
        }

        if(Input.GetKeyDown(KeyCode.Home)){
            charIndex = 0;
            keyPressed = true;

            PlaySpecialKeySound();
        }

        if(Input.GetKeyDown(KeyCode.End)){
            string[] codeLines = code.Split('\n');
            string currentLine = codeLines[lineIndex];
            int lineMaxIndex = currentLine.Length;

            charIndex = lineMaxIndex;
            keyPressed = true;

            PlaySpecialKeySound();
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

        string[] numLines = code.Split('\n');
        for(int i = 0; i < numLines.Length; i++){
            numbers += (i + 1) + "\n";
            dividers += "|\n";
        }
        
        editorLineNumbers.text = numbers;
        editorLineDividers.text = dividers;
    }

    private int CountChars(string s, char c){
        int total = 0;
        foreach(char x in s.ToCharArray()){
            if(x == c){
                total++;
            }
        }
        return total;
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
            colorizedCode += CodeColorizer.ColorizeCode(lines[i], theme);
            if(i != lines.Length - 1){
                colorizedCode += '\n';
            }
        }

        return colorizedCode;
    }
}

public class KeyState{
    public KeyState(KeyCode key){
        this.key = key;
    }
    public KeyCode key;
    public float lastTickTime, lastPressTime;
}