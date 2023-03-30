//BEHOLD, THE SAFB (slow as fuck, boi)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using CommandControl;

public class Compiler : MonoBehaviour{
    public static Compiler Instance { get; private set; }

    string code, processedCode;
    string[] codeLines;
    
    //takes text from InputField instead of InputField's text component
    //Text component truncates text that isn't on screen at the moment
    //InputField's text property stores all the text lines
    [SerializeField] private InputField input;
    
    //External monobehaviour handler scripts
    FunctionHandler functionHandler;
    
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

    public void Awake(){
        if(Instance == null){
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    private void Start(){
        _allVars = new Dictionary<string, string>();
        _intVars = new Dictionary<string, int>();
        _strVars = new Dictionary<string, string>();

        functionHandler = gameObject.GetComponent<FunctionHandler>();
    }

    public void Run(){
        Reset.Instance.Exterminatus();
        Invoke("delayedRun", 0.05f);
    }

    private void delayedRun(){
        if(Reset.isResetSuccessful()){
            //Debug.Log("Exterminatus successful");
            preprocessCode(input.text);
            StartCoroutine(runLines(0, codeLines.Length));
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
        foreach(string s in unformattedLines){
            if(string.IsNullOrWhiteSpace(s)){
                continue;
            }
            string formattedLine = CodeFormatter.Format(s);
            string[] sections = formattedLine.Split(' ');

            if(!string.IsNullOrWhiteSpace(formattedLine)){
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
        }
        codeLines = formattedLines.ToArray();
    }

    private IEnumerator runLines(int lineIndex, int stopIndex){

        //Debug.Log("running lines with index " + lineIndex);
        
        string currentLine = codeLines[lineIndex];
        string[] sections = currentLine.Split(' ');

        //checks for the Read() function
        if(sections.Length > 1 && sections.Contains("read")){
            currentLine = checkForRead(sections);
        }

        //checks for functions
        if(sections.Length > 1 && FunctionHandler.builtInFunctions.Contains(sections[0])){
            functionHandler.initializeHandler(currentLine);
            yield return StartCoroutine(functionHandler.runFunction());
        }

        //check for variable assignment
        if(sections.Length > 1 && sections.Contains("=")){
            assignVariable(currentLine);
        }

        if((lineIndex + 1) >= codeLines.Length || (lineIndex + 1) == stopIndex){
            //Debug.Log("execution complete!");
        } else {
            yield return StartCoroutine(runLines(lineIndex + 1, stopIndex));
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
                //Debug.Log("$read = " + strVars["$read"]);
            } else {
                //error
                Debug.LogAssertion("went wrong somewhere");
            }
        }
        
        return arrayToString(sectionList.ToArray(), 0);
    }

    //split a string by space to find the operator
    //token immediately before operator is the variable name
    //everything after the operator is the expression
    //everything before the variable name is the variable type
    //for now only does ints
    private void assignVariable(string line){
        string[] sections = line.Split(' ');
        int operatorIndex = Array.IndexOf(sections, "=");
        string varName = sections[operatorIndex - 1];
        string expression = line.Split('=')[1].Trim();
        string[] delim = {varName};
        string varType = line.Split(delim, StringSplitOptions.None)[0].Trim();

        if(!string.IsNullOrWhiteSpace(varType)){
            switch(varType){
                case "int":
                    if(!allVars.ContainsKey(varName) && !intVars.ContainsKey(varName)){
                        allVars.Add(varName, "int");
                        intVars.Add(varName, 0);
                        //Debug.Log("Created new int " + varName);
                    } else {
                        //Debug.LogAssertion("Variable \"" + varName + "\" already exists!");
                        return;
                    }
                    break;
                case "string":
                    if(!allVars.ContainsKey(varName) && !strVars.ContainsKey(varName)){
                        allVars.Add(varName, "string");
                        strVars.Add(varName, null);
                    } else {
                        return;
                    }
                    break;
            }
        }
        
        if(!allVars.ContainsKey(varName)){
            //Debug.Log("Variable " + varName + " does not exist!");
            return;
        }

        switch(allVars[varName]){
            case "int":
                IntExpression intEval = new IntExpression(expression);
                if(intEval.isInt){
                    int intValue = intEval.evaluate();
                    intVars[varName] = intValue;
                    //Debug.Log("Assigned " + intValue + " to int " + varName);
                } else {
                    //Debug.LogAssertion("Not an integer!");
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