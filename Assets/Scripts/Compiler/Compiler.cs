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
    Dictionary<string, VariableInfo> _allVars;
    Dictionary<string, int> _intVars;
    Dictionary<string, string> _strVars;
    Dictionary<string, bool> _boolVars;

    //Condition dictionary
    Dictionary<int, ConditionBlocks> conditionByIndex;

    public Dictionary<string, VariableInfo> allVars{
        get { return _allVars; }
    }
    public Dictionary<string, int> intVars{
        get { return _intVars; }
    }
    public Dictionary<string, string> strVars{
        get { return _strVars; }
    }
    public Dictionary<string, bool> boolVars{
        get { return _boolVars; }
    }

    private int currentIndex = 0;
    private bool hasError = false;
    private bool hasArgError = false;
    public int linesCount = 0;

    public void Awake(){
        if(Instance == null){
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    private void Start(){
        _allVars = new Dictionary<string, VariableInfo>();
        _intVars = new Dictionary<string, int>();
        _strVars = new Dictionary<string, string>();
        _boolVars = new Dictionary<string, bool>();
        conditionByIndex = new Dictionary<int, ConditionBlocks>();

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
            linesCount = 0;
            //Debug.Log("Exterminatus successful");
            preprocessCode(tmpInput.text);
            StartCoroutine(firstPass(0, codeLines.Length));
        }
    }
    
    private void preprocessCode(string rawCode){
        //reserves the $read name for the read() function
        allVars.Add("$read", new VariableInfo(VariableInfo.Type.strVar, true));
        strVars.Add("$read", null);
        
        code = rawCode;

        //forces curly braces on their own lines
        code = code.Replace("{", "\n{\n");
        code = code.Replace("}", "\n}\n");

        //format lines
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

            //checks for the Read() function
            if(sections.Length > 1 && sections.Contains("read")){
                formattedLine = CheckForRead(sections);
            }

            //Debug.Log(formattedLine);

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

        /*foreach(string s in codeLines){
            Debug.Log(s);
        }*/

        //create condition block info
        List<ConditionBlocks> conditionBlocks = new List<ConditionBlocks>();
        int blockDepth = 0;
        for(int i = 0; i < codeLines.Length; i++){
            string section = codeLines[i].Split(' ')[0];
            var condition = new ConditionBlocks(){ lineIndex = i, depth = blockDepth};

            switch(section){
                case "if":
                    condition.type = ConditionBlocks.Type.If;
                    conditionBlocks.Add(condition);
                break;
                case "else":
                    //check next token if it's if or not
                    if(codeLines[i].Split(' ').Length == 1){
                        //else
                        condition.type = ConditionBlocks.Type.Else;
                        conditionBlocks.Add(condition);
                    } else {
                        //there's something else in here
                        if(codeLines[i].Split(' ')[1] == "if"){
                            condition.type = ConditionBlocks.Type.ElseIf;
                            conditionBlocks.Add(condition);
                        } else {
                            //error
                        }
                    }
                break;
                case "{":
                    blockDepth++;
                break;
                case "}":
                    blockDepth--;
                break;
            }
        }

        //create links between chained conditions
        for(int i = 0; i < conditionBlocks.Count; i++){
            var condition = conditionBlocks[i];
            conditionByIndex.Add(condition.lineIndex, condition);
            //find else/else if attached to this block
            if(condition.type == ConditionBlocks.Type.If || condition.type == ConditionBlocks.Type.ElseIf){
                for(int j = i + 1; j < conditionBlocks.Count; j++){
                    var nextCondition = conditionBlocks[j];

                    //at same depth - continuation of chain
                    if(nextCondition.depth == condition.depth){
                        //if starts a new chain
                        if(nextCondition.type == ConditionBlocks.Type.If){
                            break;
                        }
                        condition.nextBlock = nextCondition;
                        nextCondition.lastBlock = condition;
                        break;
                    }

                    //out of scope - end of chain
                    else if(nextCondition.depth < condition.depth){
                        break;
                    }
                }
            }
        }

        //count non-empty lines
        foreach(string s in codeLines){
            if(!string.IsNullOrWhiteSpace(s)){
                linesCount++;
            }
        }

        lineCount.text = string.Format("Lines: {0}", linesCount);
    }

    private IEnumerator firstPass(int lineIndex, int stopIndex){
        currentIndex = lineIndex;
        
        string currentLine = codeLines[lineIndex];
        string[] sections = currentLine.Split(' ');

        //enable once everything is ready
        LineChecker lineChecker = new LineChecker(currentLine, lineIndex + 1, errorChecker);
        LineType lineType = lineChecker.lineType;
        
        //Debug for figuring out lineTypes:
        //Debug.Log(lineType);

        /*if(lineChecker.hasError){
            this.hasError = true;
        } else {
            sections = semicolonRemover(sections);
            currentLine = arrayToString(sections, 0);
        }*/

        //remove this line once error checker is active again
        lineChecker.hasError = false;

        if(!lineChecker.hasError && lineType == LineType.varInitialize){
            initializeVariable(currentLine);
        }

        //argument errors should be checked here
        if(!lineChecker.hasError && lineType == LineType.functionCall){
            functionHandler.initializeHandler(currentLine, lineIndex + 1);
            hasArgError = functionHandler.hasError;
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
        //makes the lineIndex global (somewhat)
        currentIndex = lineIndex;
        
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

        //test if line is a conditional
        if(conditionByIndex.ContainsKey(lineIndex)){
            Debug.Log("conditional detected");
            var condition = conditionByIndex[lineIndex];
            bool runCondition = GetConditionResult(currentLine);

            if(condition.type == ConditionBlocks.Type.Else){
                runCondition = true;
            }

            if(condition.type != ConditionBlocks.Type.If){
                var lastBlock = condition.lastBlock;
                while(lastBlock != null){
                    if(lastBlock.lastEval == true){
                        runCondition = false;
                        break;
                    }
                    lastBlock = lastBlock.lastBlock;
                }
            }

            condition.lastEval = runCondition;
            Debug.Log(runCondition);
            HandleCondition(runCondition, lineIndex, stopIndex);
            yield break;
        }

        //test if line is a while loop
        if(currentLine.Split(' ')[0] == "while"){
            bool runLoop = GetConditionResult(currentLine);
            int loopEndIndex = BlockEndLineIndex(lineIndex);
            
            //run while true
            if(runLoop){
                Debug.Log("Looping");
                //run until block end index
                //then go back to start of block
                yield return StartCoroutine(secondPass(lineIndex + 1, loopEndIndex));
                yield return StartCoroutine(secondPass(lineIndex, lineIndex));
                yield break;
            } else {
                //run line AFTER block end index
                StartCoroutine(secondPass(loopEndIndex + 1, stopIndex));
                yield break;
            }
        }

        //if this is in first pass, variables aren't added in runtime
        //making dynamically changing variables in runtime impossible
        //as the variable takes its last value at the end of the code after 1st pass
        //keep this here
        //add error checking mechanism to second pass
        if(lineType == LineType.varAssign){
            assignVariable(currentLine);
        }

        if((lineIndex + 1) >= codeLines.Length || (lineIndex + 1) == stopIndex){
            Debug.LogWarning("Execution finished!");
        } else {
            yield return StartCoroutine(secondPass(lineIndex + 1, stopIndex));
        }
    }
   
    private void initializeVariable(string line){
        string[] sections = line.Split(' ');
        string varName = sections[1];
        string varType = sections[0];

        switch(varType){
            case "int":
                if(!allVars.ContainsKey(varName) && !intVars.ContainsKey(varName)){
                    allVars.Add(varName, new VariableInfo(VariableInfo.Type.intVar, false));
                    intVars.Add(varName, 0);
                } else {
                    errorChecker.variableDefinedError(currentIndex + 1, varName);
                    return;
                }
                break;
            case "int[]":
                if(!allVars.ContainsKey(varName) && !intVars.ContainsKey(varName)){
                    allVars.Add(varName, new VariableInfo(VariableInfo.Type.intVarArr, false));
                    intVars.Add(varName, 0);
                } else {
                    errorChecker.variableDefinedError(currentIndex + 1, varName);
                }
                break;
            case "string":
                if(!allVars.ContainsKey(varName) && !strVars.ContainsKey(varName)){
                    allVars.Add(varName, new VariableInfo(VariableInfo.Type.strVar, false));
                    strVars.Add(varName, null);
                } else {
                    errorChecker.variableDefinedError(currentIndex + 1, varName);
                    return;
                }
                break;
            case "bool":
                if(!allVars.ContainsKey(varName) && !boolVars.ContainsKey(varName)){
                    allVars.Add(varName, new VariableInfo(VariableInfo.Type.boolVar, false));
                    boolVars.Add(varName, false);
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

        switch(allVars[varName].type){
            case VariableInfo.Type.intVar:
                IntExpression intEval = new IntExpression(expression);
                if(intEval.isInt){
                    int intValue = intEval.evaluate();
                    intVars[varName] = intValue;
                    allVars[varName].isSet = true;
                } else {
                    errorChecker.wrongTypeError(currentIndex + 1, varType);
                }
                break;
            case VariableInfo.Type.intVarArr:
                //gets the line after the first "{" and the line of the last "}"
                int arrayStartIndex = currentIndex + 2;
                int arrayEndIndex = BlockEndLineIndex(currentIndex);
                
                //iterate through lines, adding every entry into the array
                //step 1: put all values in the same string, assuming they're on newlines
                List<string> arrValTempString = new List<string>();
                for(int i = arrayStartIndex; i < arrayEndIndex; i++){
                    arrValTempString.Add(codeLines[i]);
                }
                string arrayString = arrayToString(arrValTempString.ToArray(), 0).Trim();
                //Debug.Log(arrayString);

                //step 2: isolate values from spaces and commas
                string[] arrVals = arrayString.Split(' ');
                arrVals = arrayToString(arrVals, 0).Split(',');
                for(int i = 0; i < arrVals.Length; i++){
                    arrVals[i] = arrVals[i].Trim();
                }
                /*foreach(string s in arrVals){
                    Debug.Log(s);
                }*/

                //step 3: evaluate and add to list of ints
                List<int> values = new List<int>();
                for(int i = 0; i < arrVals.Length; i++){
                    var intArrEval = new IntExpression(arrVals[i]);
                    values.Add(intArrEval.evaluate());
                }
                /*foreach(int a in values){
                    Debug.Log(a);
                }*/

                //step 4: add to the array via AddArray() function
                AddIntArray(varName, values.ToArray());

                //Debug to see if the array stored properly:
                foreach(KeyValuePair<string, int> entry in intVars){
                    Debug.Log(entry.Key + ": " + entry.Value);
                }
                
                break;
            case VariableInfo.Type.strVar:
                StringExpression strEval = new StringExpression(expression);
                string strTxt = strEval.removeQuotations();
                strVars[varName] = strTxt;
                allVars[varName].isSet = true;
                break;
            case VariableInfo.Type.boolVar:
                bool boolVal = GetConditionResult("( " + expression + " )");
                boolVars[varName] = boolVal;
                allVars[varName].isSet = true;
                break;
        }
    }

    void AddIntArray(string varName, int[] varValues){
        for(int i = 0; i < varValues.Length; i++){
            string arrVarName = varName + ReservedConstants.arrayIndexSeparator + i;
            allVars.Add(arrVarName, new VariableInfo(VariableInfo.Type.intVar, true));
            intVars.Add(arrVarName, varValues[i]);
        }
    }

    //IT WORKS NOW
    //TODO: Check for Bot. callstring
    private string CheckForRead(string[] sections){
        //recur to check
        //get index of bot
        //check following tokens
        //must be ., read, (, )
        //if it's a different function call, cannot convert void to type
        //if it's just Bot, error: is Type, not valid in current context

        List<string> sectionList = sections.ToList();
        int readIndex = sectionList.IndexOf("read");

        //something that checks for if "read()" is preceeded by "Bot."

        int depth = -1;
        List<string> readList = new List<string>();
        for(int i = readIndex; i < sectionList.Count; i++){
            if(sectionList[i] == "("){
                readList.Add(sectionList[i]);
                depth++;
            } else if(sectionList[i] == ")"){
                if(depth > 0){
                    readList.Add(sectionList[i]);
                    depth--;
                } else if(depth < 0){
                    //error
                    //too many closed parentheses
                } else if(depth == 0){
                    //properly closed
                    readList.Add(sectionList[i]);
                    break;
                }
            } else {
                readList.Add(sectionList[i]);
            }
        }

        //checks if found read() contains no parameters
        //shit only reads strings
        if(arrayToString(readList.ToArray(), 0).Equals("read ( )")){
            sectionList.RemoveRange(readIndex, 3);
            sectionList.Insert(readIndex, "$read");
            Interaction playerAct = GameObject.Find("PlayerCharacter").GetComponent<Interaction>();
            strVars["$read"] = playerAct.read();
        } else {
            Debug.Log("went wrong somewhere");
        }
        return arrayToString(sectionList.ToArray(), 0);
    }

    bool GetConditionResult(string line){
        Debug.Log(line);
        int startIndex = line.IndexOf('(') + 1;
        int endIndex = line.LastIndexOf(')');
        string conditionString = GetSubstring(line, startIndex, endIndex);
        Debug.Log(conditionString);
        List<string> sections = conditionString.Split(' ').ToList<string>();
        Debug.Log(arrayToString(sections.ToArray(), 0));

        //check if currentString has a boolean variable or value
        //preprocess code to replace stuff
        //way this works is it can't take booleans as-is
        //so convert booleans to numerical comparisons that evaluate as true or false
        for(int i = 0; i < sections.Count; i++){
            string s = sections[i];
            Debug.Log(s);
            //check if boolean value
            if(s.ToLower() == "true"){
                string[] arr = {"0", "==", "0"};
                sections.InsertRange(i, arr);
                sections.RemoveAt(i + 3);
                i += 3;
            }
            if(s.ToLower() == "false"){
                string[] arr = {"0", "!=", "0"};
                sections.InsertRange(i, arr);
                sections.RemoveAt(i + 3);
                i += 3;  
            }

            //check if boolean variable
            if(allVars.ContainsKey(s) && allVars[s].type == VariableInfo.Type.boolVar){
                if(boolVars[s] == true){
                    string[] arr = {"0", "==", "0"};
                    sections.InsertRange(i, arr);
                    sections.RemoveAt(i + 3);
                    i += 3;
                } else {
                    string[] arr = {"0", "!=", "0"};
                    sections.InsertRange(i, arr);
                    sections.RemoveAt(i + 3);
                    i += 3;
                }
            }
        }

        Debug.Log(arrayToString(sections.ToArray(), 0));
        
        var numValues = new List<float>();
        var operators = new List<string>();
        string currentString = "";

        var boolValues = new List<bool>();
        var boolOperators = new List<BoolExpression.Element>();

        //simplify condition by finding/evaluating numerical expressions within
        //store as list of values and operators
        Debug.Log(sections.Count);
        for(int i = 0; i < sections.Count; i++){
            string section = sections[i];
            Debug.Log(section);
            bool isConditionOperator = ReservedConstants.comparisonOperators.Contains(section);

            if(isConditionOperator || i == sections.Count - 1){
                //checks if condition operator
                if(isConditionOperator){
                    operators.Add(section);
                }

                //TODO: add something for boolean strings
                //TODO: add something for hybrid strings
                //TODO: something that converts variables into values
                //possible: separate conditions by type, evaluate as needed
                //like if (true || 10 > 5)

                //currentString is a group that isn't an operator
                //so like 10 + 5 > 20
                //currentString 1 is 10 + 5, then 20    
                //currentString is purged when iterator hits an operator

                if(!string.IsNullOrEmpty(currentString)){
                    //if not a boolean, assume numerical comparison
                    var expression = new IntExpression(currentString);
                    numValues.Add(expression.evaluate());
                    currentString = "";
                }
            } else {                
                currentString += section + " ";
            }
        }

        //evaluate comparisons to bools
        for(int i = 0; i < operators.Count; i++){
            float a = numValues[i];
            float b = numValues[i + 1];
            string op = operators[i];

            Debug.Log(a + " " + op + " " + b);
            switch(op){
                case "<":
                    boolValues.Add(a < b);
                    boolOperators.Add(BoolExpression.Element.Value);
                break;
                case "<=":
                    boolValues.Add(a <= b);
                    boolOperators.Add(BoolExpression.Element.Value);
                break;
                case ">":
                    boolValues.Add(a > b);
                    boolOperators.Add(BoolExpression.Element.Value);
                break;
                case ">=":
                    boolValues.Add(a >= b);
                    boolOperators.Add(BoolExpression.Element.Value);
                break;
                case "==":
                    boolValues.Add(a == b);
                    boolOperators.Add(BoolExpression.Element.Value);
                break;
                case "!=":
                    boolValues.Add(a != b);
                    boolOperators.Add(BoolExpression.Element.Value);
                break;
                case "&&":
                    boolOperators.Add(BoolExpression.Element.And);
                break;
                case "||":
                    boolOperators.Add(BoolExpression.Element.Or);
                break;
            }
        }

        /*
        for(int i = 0; i < numValues.Count; i++){
            Debug.Log(numValues[i]);
        }
        */

        var boolExpression = new BoolExpression(boolValues, boolOperators);
        bool result = boolExpression.Evaluate();
        return result;
    }

    private void HandleCondition(bool condition, int lineIndex, int stopIndex){
        if(condition){
            StartCoroutine(secondPass(lineIndex + 1, stopIndex));
        } else {
            int conditionStopIndex = BlockEndLineIndex(lineIndex);
            StartCoroutine(secondPass(conditionStopIndex + 1, stopIndex));
        }
    }

    int BlockEndLineIndex(int blockStartIndex){
        bool startIndexFound = false;
        int depth = 0;
        int blockEndIndex = blockStartIndex;

        for(int i = blockStartIndex; i < codeLines.Length; i++){
            string line = codeLines[i];
            if(line.Contains("{")){
                startIndexFound = true;
                depth++;
            }
            if(line.Contains("}")){
                depth--;
            }
            if(depth == 0 && startIndexFound){
                blockEndIndex = i;
                break;
            }
        }
        return blockEndIndex;
    }

    public static string GetSubstring(string s, int startIndex, int endIndex){
        int length = endIndex - startIndex;
        if(length <= 0){
            return "";
        }
        return s.Substring(startIndex, length);
    }
    
    private string formatOperator(string[] array, string opChar){        
        return string.Format("{0} = {0} {1} ( {2} )", array[0], opChar, arrayToString(array, 2));
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
        boolVars.Clear();
        conditionByIndex.Clear();
    }

    //called by anything that should stop execution
    //invalid direction errors, stepping on a trap etc
    public void terminateExecution(){
        clearDictionaries();
        Invoke("killTimer", Globals.Instance.timePerStep);
        Debug.Log("Execution terminated");
    }

    //delays the global coroutine stop by the global time to move
    //allows motion animations to finish before terminating
    private void killTimer(){
        foreach(var o in FindObjectsOfType<MonoBehaviour>()){
            o.StopAllCoroutines();
        }
    }
}

public class ConditionBlocks{
    public enum Type { If, ElseIf, Else }
    public Type type;
    public bool lastEval;
    public int lineIndex;
    public ConditionBlocks lastBlock;
    public ConditionBlocks nextBlock;
    public int depth;
}

public class VariableInfo{
    public enum Type { intVar, intVarArr, strVar, boolVar }
    public VariableInfo(Type type, bool isSet){
        this.type = type;
        this.isSet = isSet;
    }
    public Type type;
    public bool isSet;
}