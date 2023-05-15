//BEHOLD, THE SAFB (slow as fuck, boi)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using CommandControl;
using System.Text.RegularExpressions;

public class Compiler : MonoBehaviour{
    public static Compiler Instance { get; private set; }

    string code, processedCode;
    string[] codeLines;
    
    //takes text from InputField instead of InputField's text component
    //Text component truncates text that isn't on screen at the moment
    //InputField's text property stores all the text lines
    [SerializeField] private TMPro.TMP_Text tmpInput;

    [SerializeField] private TMPro.TMP_Text lineCount;
    
    //External monobehaviour handler scripts
    FunctionHandler functionHandler;
    ErrorChecker errorChecker;
    
    //Variable dictionaries
    Dictionary<string, string> _allVars;
    Dictionary<string, int> _intVars;
    Dictionary<string, string> _strVars;

    public Dictionary<string, string> allVars{
        get { return _allVars; }
    }
    public Dictionary<string, int> intVars{
        get { return _intVars; }
    }
    public Dictionary<string, string> strVars{
        get { return _strVars; }
    }

    private int currentIndex = 0;
    private bool hasError = false;
    private bool hasArgError = false;

    public void Awake(){
        if(Instance == null){
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    private void Start(){
        _allVars = new Dictionary<string, string>();
        _intVars = new Dictionary<string, int>();
        _strVars = new Dictionary<string, string>();

        functionHandler = gameObject.GetComponent<FunctionHandler>();
        errorChecker = gameObject.GetComponent<ErrorChecker>();
    }

    public void Run(){
        errorChecker.errorConvo.dialogueBlocks = new List<Dialogue>();
        hasError = false;
        Exterminatus();
        Invoke("delayedRun", 0.05f);
    }

    private void delayedRun(){
        if(isResetSuccessful()){
            //Debug.Log("Exterminatus successful");
            preprocessCode(tmpInput.text);
            StartCoroutine(firstPass(0, codeLines.Length));
        }
    }
    
    private void preprocessCode(string rawCode){
        //reserves the $read name for the read() function
        allVars.Add("$read", "string");
        strVars.Add("$read", null);
        
        code = rawCode;

        code = code.Replace("{", "\n{\n");
        code = code.Replace("}", "\n}\n");

        string[] unformattedLines = code.Split('\n');
        List<string> formattedLines = new List<string>();
        foreach(string unformattedLine in unformattedLines){
            string formattedLine = CodeFormatter.Format(unformattedLine);
            string[] sections = formattedLine.Split(' ');

            //strips comments
            foreach(string s in sections){
                if(s == "//"){
                    formattedLine = formattedLine.Substring(0, formattedLine.IndexOf("//"));
                }
            }

            if(sections.Length > 1){
                switch(sections[1]){
                    case "+=":
                        formattedLine = formatOperator(sections, "+");
                        break;
                    case "-=":
                        formattedLine = formatOperator(sections, "-");
                        break;
                    case "*=":
                        formattedLine = formatOperator(sections, "*");
                        break;
                    case "/=":
                        formattedLine = formatOperator(sections, "/");
                        break;
                    case "++":
                        formattedLine = string.Format("{0} = {0} + 1", sections[0]);
                        break;
                    case "--":
                        formattedLine = string.Format("{0} = {0} - 1", sections[0]);
                        break;
                    default:
                        break;
                }
            }
            formattedLines.Add(formattedLine);
            processedCode += formattedLine + '\n';
        }
        codeLines = formattedLines.ToArray();
        lineCount.text = string.Format("Lines: {0}", codeLines.Length.ToString());
    }

    private IEnumerator firstPass(int lineIndex, int stopIndex){
        currentIndex = lineIndex;
        
        string currentLine = codeLines[lineIndex];
        string[] sections = currentLine.Split(' ');

        LineChecker lineChecker = new LineChecker(currentLine, lineIndex + 1, errorChecker);
        LineType lineType = lineChecker.lineType;

        if(lineChecker.hasError){
            this.hasError = true;
        } else {
            sections = semicolonRemover(sections);
            currentLine = arrayToString(sections, 0);
        }

        if(!lineChecker.hasError && lineType == LineType.varInitialize){
            initializeVariable(currentLine);
        }

        if(!lineChecker.hasError && lineType == LineType.varAssign){
            assignVariable(currentLine);
        }

        //argument errors should be checked here
        if(!lineChecker.hasError && lineType == LineType.functionCall){
            functionHandler.initializeHandler(currentLine, lineIndex + 1);
            hasArgError = functionHandler.hasError;
        }

        //checks for the Read() function
        if(sections.Length > 1 && sections.Contains("read")){
            currentLine = checkForRead(sections);
        }

        if((lineIndex + 1) >= codeLines.Length || (lineIndex + 1) == stopIndex){
            if(hasError || hasArgError){
                errorChecker.writeError();
            } else {
                Debug.LogWarning("No errors - starting second pass");
                StartCoroutine(secondPass(0, stopIndex));
            }
        } else {
            yield return StartCoroutine(firstPass(lineIndex + 1, stopIndex));
        }
    }

    //removes semicolons from the end of a line
    //assuming the line is formatted correctly
    //otherwise the code won't even reach here
    //bc semicolons screw with reading expressions and function arguments
    private string[] semicolonRemover(string[] sections){
        string[] newSect = new string[sections.Length - 1];
        for(int i = 0; i < sections.Length - 1; i++){
            newSect[i] = sections[i];
        }
        return newSect;
    }

    private IEnumerator secondPass(int lineIndex, int stopIndex){
        string currentLine = codeLines[lineIndex];
        
        LineChecker lineChecker = new LineChecker(currentLine, lineIndex + 1, errorChecker);
        LineType lineType = lineChecker.lineType;

        if(lineType == LineType.functionCall){
            functionHandler.initializeHandler(currentLine, lineIndex + 1);
            hasError = functionHandler.hasError;
            if(!functionHandler.hasError){
                yield return StartCoroutine(functionHandler.runFunction());   
            }
        }

        if((lineIndex + 1) >= codeLines.Length || (lineIndex + 1) == stopIndex){
            Debug.LogWarning("Execution finished!");
        } else {
            yield return StartCoroutine(secondPass(lineIndex + 1, stopIndex));
        }
    }


    private string checkForRead(string[] sections){
        List<string> sectionList = sections.ToList();
        int readIndex = sectionList.IndexOf("read");
        
        //finds the read() function and any parameters that it might contain
        //if any instance of "read" is not immediately followed by a "("
        //assumes "read" is a variable not not the read() function
        if(sectionList[readIndex + 1] == "("){
            int readDepth = -1;
            List<string> readList = new List<string>();
            for(int i = readIndex; i < sectionList.Count; i++){
                if(sectionList[i] == "("){
                    readList.Add(sectionList[i]);
                    readDepth++;
                } else if(sectionList[i] == ")" && readDepth > 0){
                    readList.Add(sectionList[i]);
                    readDepth--;
                } else if(sectionList[i] == ")" && readDepth < 0){
                    //error
                } else if(sectionList[i] == ")" && readDepth == 0){
                    readList.Add(sectionList[i]);
                     break;
                } else {
                    readList.Add(sectionList[i]);
                }
            }
            
            //checks if the found read() function contains no parameters
            if(arrayToString(readList.ToArray(), 0).Equals("read ( )")){
                sectionList.RemoveRange(readIndex, 3);
                sectionList.Insert(readIndex, "$read");
                Interaction playerAct = GameObject.Find("PlayerCharacter").GetComponent<Interaction>();
                strVars["$read"] = playerAct.read();
            } else {
                Debug.LogAssertion("went wrong somewhere");
            }
        }
        
        return arrayToString(sectionList.ToArray(), 0);
    }
   
    private void initializeVariable(string line){
        string[] sections = line.Split(' ');
        string varName = sections[1];
        string varType = sections[0];

        switch(varType){
            case "int":
                if(!allVars.ContainsKey(varName) && !intVars.ContainsKey(varName)){
                    allVars.Add(varName, "int");
                    intVars.Add(varName, 0);
                } else {
                    errorChecker.variableDefinedError(currentIndex + 1, varName);
                    return;
                }
                break;
            case "string":
                if(!allVars.ContainsKey(varName) && !strVars.ContainsKey(varName)){
                    allVars.Add(varName, "string");
                    strVars.Add(varName, null);
                } else {
                    errorChecker.variableDefinedError(currentIndex + 1, varName);
                    return;
                }
                break;
            default:
                errorChecker.nonexistentTypeError(currentIndex + 1, varType);
                break;
        }
    }

    //split a string by space to find the operator
    //token immediately before operator is the variable name
    //everything after the operator is the expression
    //everything before the variable name is the variable type
    //for now only does ints
    private void assignVariable(string line){
        string[] sections = line.Split(' ');
        int opIndex = Array.IndexOf(sections, "=");
        string varName = sections[opIndex - 1];
        string varType = opIndex != 1 ? sections[opIndex - 2] : null;
        string expression = line.Split('=')[1].Trim();

        if(!string.IsNullOrWhiteSpace(varType)){
            initializeVariable(line);
        }

        if(ReservedConstants.varTypes.Contains(varName) && varType == null){
            errorChecker.unnamedVariableError(currentIndex + 1, varName);
            return;
        } else if(ReservedConstants.reserved.Contains(varName)){
            errorChecker.reservedNameError(currentIndex + 1);
            return;
        } else if(!allVars.ContainsKey(varName)){
            errorChecker.noexistentVariableError(currentIndex + 1, varName);
            return;
        }

        switch(allVars[varName]){
            case "int":
                IntExpression intEval = new IntExpression(expression);
                if(intEval.isInt){
                    int intValue = intEval.evaluate();
                    intVars[varName] = intValue;
                } else {
                    errorChecker.wrongTypeError(currentIndex + 1, varType);
                }
                break;
            case "string":
                StringExpression strEval = new StringExpression(expression);
                string strTxt = strEval.removeQuotations();
                strVars[varName] = strTxt;
                break;
        }
    }
    
    private string formatOperator(string[] array, string opChar){        
        return string.Format("{0} = {0} {1} ( {2} )", array[0], opChar,
            arrayToString(array, 2));
    }

    public static string arrayToString(string[] array, int startIndex){
        string s = "";
        for(int i = startIndex; i < array.Length; i++){
            s += array[i];
            if(i != array.Length - 1){
                s += " ";
            }
        }
        return s;
    }

    public void Exterminatus(){
        //Debug.LogAssertion("Declaring Exterminatus");
        clearDictionaries();
        EditorSaveLoad.Instance.SaveEditorState();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public static bool isResetSuccessful(){
        if(SceneManager.GetActiveScene().isDirty){
            return false;
        }
        return true;
    }

    public void clearDictionaries(){
        allVars.Clear();
        intVars.Clear();
        strVars.Clear();
    }

    //called by anything that should stop execution
    //invalid direction errors, stepping on a trap etc
    public void terminateExecution(){
        clearDictionaries();
        Invoke("killTimer", Globals.Instance.timePerStep);
        Debug.LogAssertion("Execution terminated");
    }

    //delays the global coroutine stop by the global time to move
    //allows motion animations to finish before terminating
    private void killTimer(){
        foreach(var o in FindObjectsOfType<MonoBehaviour>()){
            o.StopAllCoroutines();
        }
    }
}