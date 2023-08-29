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
using UnityEditor.Build;
using Microsoft.SqlServer.Server;

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
    public FunctionHandler functionHandler;
    public ErrorChecker errorChecker;
    
    //Variable dictionaries
    Dictionary<string, VariableInfo> _allVars;
    Dictionary<string, int> _intVars;
    Dictionary<string, string> _strVars;
    Dictionary<string, bool> _boolVars;
    Dictionary<string, int[]> _intArrs;
    Dictionary<string, string[]> _strArrs;
    Dictionary<string, bool[]> _boolArrs;
    Dictionary<string, double> _doubleVars;
    Dictionary<string, char> _charVars;
    Dictionary<string, double[]> _doubleArrs;
    Dictionary<string, char[]> _charArrs;

    //Condition dictionary
    Dictionary<int, ConditionBlocks> _conditionByIndex;

    //Player scripts
    Interaction playerAct;
    Movement playerMove;

    //list of dictionaries used for variable scoping
    List<Dictionary<string, string>> scopeList;
    public List<List<string>> nestedVars;

    public Dictionary<int, ConditionBlocks> conditionByIndex{
        get { return _conditionByIndex; }
    }
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
    public Dictionary<string, double> doubleVars{
        get { return _doubleVars; }
    }
    public Dictionary<string, char> charVars{
        get { return _charVars; }
    }
    public Dictionary<string, int[]> intArrs{
        get { return _intArrs; }
    }
    public Dictionary<string, string[]> strArrs{
        get { return _strArrs; }
    }
    public Dictionary<string, bool[]> boolArrs{
        get { return _boolArrs; }
    }
    public Dictionary<string, double[]> doubleArrs{
        get { return _doubleArrs; }
    }
    public Dictionary<string, char[]> charArrs{
        get { return _charArrs; }
    }
    
    public string[] getCodeLines{
        get { return codeLines; }
    }

    public int currentIndex = 0;
    public bool hasError = false;
    private bool hasArgError = false;
    public int linesCount = 0;
    public int reservedVars = 0;

    private bool isLooping = false;
    private int loopEndIndex = 0;

    public void Awake(){
        if(Instance == null){
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    private void Start(){
        //Time.timeScale = 3;
        _allVars = new Dictionary<string, VariableInfo>();
        _intVars = new Dictionary<string, int>();
        _strVars = new Dictionary<string, string>();
        _boolVars = new Dictionary<string, bool>();
        _charVars = new Dictionary<string, char>();
        _doubleVars = new Dictionary<string, double>();
        _intArrs = new Dictionary<string, int[]>();
        _strArrs = new Dictionary<string, string[]>();
        _boolArrs = new Dictionary<string, bool[]>();
        _charArrs = new Dictionary<string, char[]>();
        _doubleArrs = new Dictionary<string, double[]>();
        _conditionByIndex = new Dictionary<int, ConditionBlocks>();
        scopeList = new List<Dictionary<string, string>>();
        nestedVars = new List<List<string>>();
        
        playerAct = GameObject.Find("PlayerCharacter").GetComponent<Interaction>();
        playerMove = GameObject.Find("PlayerCharacter").GetComponent<Movement>();
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
            preprocessCode(tmpInput.text);
            StartCoroutine(firstPass(0, codeLines.Length));
        }
    }

    private void preprocessCode(string rawCode){
        playerAct = GameObject.Find("PlayerCharacter").GetComponent<Interaction>();
        playerMove = GameObject.Find("PlayerCharacter").GetComponent<Movement>();
        
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
            for(int i = 0; i < sections.Length; i++){
                if(sections[i] == "//" && i > 0){
                    formattedLine = formattedLine.Substring(0, formattedLine.IndexOf("//")).Trim();
                    break;
                } else if(i == 0 && sections[i] == "//"){
                    formattedLine = "";
                    break;
                }
            }

            sections = formattedLine.Split(' ');

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
                        if(Array.LastIndexOf(sections, ";") == sections.Length - 1){
                            formattedLine = string.Format("{0} = {0} + 1 ;", sections[0]);
                        } else {
                            formattedLine = string.Format("{0} = {0} + 1", sections[0]);
                        }
                        break;
                    case "--":
                        if(Array.LastIndexOf(sections, ";") == sections.Length - 1){
                            formattedLine = string.Format("{0} = {0} - 1 ;", sections[0]);
                        } else {
                            formattedLine = string.Format("{0} = {0} - 1", sections[0]);
                        }
                        break;
                    default:
                        break;
                }
            }
            
            formattedLines.Add(formattedLine);
            processedCode += formattedLine + '\n';
        }
        
        //remove newlines before and after braces
        for(int i = 0; i < formattedLines.Count; i++){
            string s = formattedLines[i];
            string beforeS = i == 0 ? "notempty" : formattedLines[i - 1];
            string afterS = i == formattedLines.Count - 1 ? "notempty" : formattedLines[i + 1];

            if(s == "{" || s == "}"){
                if(string.IsNullOrEmpty(afterS)){
                    formattedLines.RemoveAt(i + 1);
                }
                if(string.IsNullOrEmpty(beforeS)){
                    formattedLines.RemoveAt(i - 1);
                    i--;
                }
            }

            if(s =="}"){
                afterS = i == formattedLines.Count - 1 ? "notempty" : formattedLines[i + 1];
                if(afterS == ";"){
                    formattedLines[i] = "} ;";
                    formattedLines.RemoveAt(i + 1);
                }
            }
        }

        formattedLines.Add("");
        codeLines = formattedLines.ToArray();

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
                //redundant loop
                //TODO: remove nesting for
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

    int braceDepth = 0;
    private IEnumerator firstPass(int lineIndex, int stopIndex){
        currentIndex = lineIndex;
        
        string currentLine = codeLines[lineIndex];
        string[] sections = currentLine.Split(' ');

        //replace indices
        currentLine = CheckForIndex(sections);

        //enable once everything is ready
        LineChecker lineChecker = new LineChecker(currentLine, lineIndex + 1, errorChecker);
        lineChecker.CheckLine();
        LineType lineType = lineChecker.lineType;
        
        //checks for the Read() functions
        if(!lineChecker.hasError && sections.Length > 1 && (sections.Contains("read") || sections.Contains("readInt") || sections.Contains("readBool"))){
            currentLine = CheckForRead(sections);
            sections = currentLine.Split(' ');
        }

        if(!lineChecker.hasError && sections.Length > 1 && (sections.Contains("ReadStringArr") || sections.Contains("ReadIntArr") || sections.Contains("ReadBoolArr"))){
            currentLine = CheckForReadArray(sections);
            sections = currentLine.Split(' ');
        }
        
        //checks for the CheckCube() function
        if(!lineChecker.hasError && sections.Length > 1 && sections.Contains("CheckCube")){
            currentLine = CheckForCube(sections, true);
            sections = currentLine.Split(' ');
        }

        //checks for an array length property
        if(!lineChecker.hasError && sections.Length > 1 && sections.Contains("Length")){
            currentLine = CheckForArrLength(sections);
            sections = currentLine.Split(' ');
        }
        //Debug for figuring out lineTypes:

        if(lineChecker.hasError){
            this.hasError = true;
        } else {
            if(sections.Length > 1){
                sections = semicolonRemover(sections);
            }
            currentLine = arrayToString(sections, 0);
        }

        //track bracket depth
        if(lineType == LineType.openBrace){
            scopeList.Add(new Dictionary<string,string>());
            nestedVars.Add(new List<string>());
            braceDepth++;
        } 
        if(lineType == LineType.closeBrace){
            int lastIndex = scopeList.Count - 1;
            foreach(KeyValuePair<string,string> kv in scopeList[lastIndex]){
                allVars.Remove(kv.Key);
                switch(kv.Value){
                    case "int":
                        intVars.Remove(kv.Key);
                    break;
                    case "string":
                        strVars.Remove(kv.Key);
                    break;
                    case "bool":
                        boolVars.Remove(kv.Key);
                    break;
                }
            }
            scopeList.RemoveAt(lastIndex);
            braceDepth--;
        }

        if(!lineChecker.hasError && lineType == LineType.varInitialize){
            initializeVariable(currentLine);
        }
        if(!lineChecker.hasError && lineType == LineType.varAssign){
            assignVariable(currentLine);
        }

        if((lineIndex + 1) >= codeLines.Length || (lineIndex + 1) == stopIndex){
            if(braceDepth > 0){
                Debug.LogWarning("Unclosed braces somewhere");
                hasError = true;
            }
            if(braceDepth < 0){
                Debug.LogWarning("Unopened braces somewhere");
                hasError = true;
            }

            //check if any else or elseif declarations don't have a preceding condition
            foreach(KeyValuePair<int, ConditionBlocks> entry in conditionByIndex){
                var con = entry.Value;
                bool hasLastBlock = con.lastBlock != null;
                if(con.type == ConditionBlocks.Type.ElseIf || con.type == ConditionBlocks.Type.Else){
                    if(!hasLastBlock){
                        //error
                        Debug.LogWarning("Else/ElseIf cannot start a conditional chain");
                        //hasError = true;
                    }
                }
            }

            if(hasError || hasArgError){
                errorChecker.writeError();
            } else {
                Debug.LogWarning("No errors - starting second pass");
                clearDictionaries(false);
                scopeList.Clear();
                braceDepth = 0;
                StartCoroutine(secondPass(0, stopIndex));
            }
        } else {
            yield return StartCoroutine(firstPass(lineIndex + 1, stopIndex));
        }
    }

    //removes semicolons from the end of a line
    //assuming the line is formatted correctly
    private string[] semicolonRemover(string[] sections){
        List<string> newSect = sections.ToList();
        if(newSect[newSect.Count - 1] == ";"){
            newSect.RemoveAt(newSect.Count - 1);
        }

        return newSect.ToArray();
    }

    private IEnumerator secondPass(int lineIndex, int stopIndex){
        //makes the lineIndex global (somewhat)
        currentIndex = lineIndex;
        
        string currentLine = codeLines[lineIndex];
        string[] sections = currentLine.Split(' ');

        //replace indices
        currentLine = CheckForIndex(sections);

        LineChecker lineChecker = new LineChecker(currentLine, lineIndex + 1, errorChecker);
        lineChecker.CheckLine();
        LineType lineType = lineChecker.lineType;

        if(lineChecker.hasError){
            this.hasError = true;
        } else {
            if(sections.Length > 1){
                sections = semicolonRemover(sections);
            }
            currentLine = arrayToString(sections, 0);
        }

        //checks for the Read() functions
        if(!lineChecker.hasError && sections.Length > 1 && (sections.Contains("read") || sections.Contains("readInt") || sections.Contains("readBool"))){
            currentLine = CheckForRead(sections);
            sections = currentLine.Split(' ');
        }

        if(!lineChecker.hasError && sections.Length > 1 && (sections.Contains("ReadStringArr") || sections.Contains("ReadIntArr") || sections.Contains("ReadBoolArr"))){
            currentLine = CheckForReadArray(sections);
            sections = currentLine.Split(' ');
        }

        //checks for the CheckCube() function
        if(!lineChecker.hasError && sections.Length > 1 && sections.Contains("CheckCube")){
            yield return new WaitForSeconds(Globals.Instance.timePerStep);
            currentLine = CheckForCube(sections, false);
            sections = currentLine.Split(' ');
        }

        //checks for an array length property
        if(!lineChecker.hasError && sections.Length > 1 && sections.Contains("Length")){
            currentLine = CheckForArrLength(sections);
            sections = currentLine.Split(' ');
        }

        //track bracket depth
        if(lineType == LineType.openBrace){
            scopeList.Add(new Dictionary<string,string>());
            nestedVars.Add(new List<string>());
            braceDepth++;
        } 
        if(lineType == LineType.closeBrace){
            int lastIndex = scopeList.Count - 1;
            foreach(KeyValuePair<string,string> kv in scopeList[lastIndex]){
                allVars.Remove(kv.Key);
                switch(kv.Value){
                    case "int":
                        intVars.Remove(kv.Key);
                    break;
                    case "string":
                        strVars.Remove(kv.Key);
                    break;
                    case "bool":
                        boolVars.Remove(kv.Key);
                    break;
                }
            }
            scopeList.RemoveAt(lastIndex);
            braceDepth--;
        }

        if(lineType == LineType.varInitialize){
            initializeVariable(currentLine);
        }
        if(lineType == LineType.varAssign){
            assignVariable(currentLine);
        }

        if(lineType == LineType.functionCall && currentLine.Split(' ')[0] == "Bot"){
            functionHandler.initializeHandler(currentLine, lineIndex + 1);
            hasError = functionHandler.hasError;
            if(!functionHandler.hasError){
                yield return StartCoroutine(functionHandler.runFunction());   
            }
        }

        //test if line is a conditional
        if(conditionByIndex.ContainsKey(lineIndex)){
            var condition = conditionByIndex[lineIndex];
            string expression = BoolExpression.ProcessBool(currentLine);
            bool runCondition = BoolExpression.GetConditionResult("( " + expression + " )");

            //checks for else
            if(condition.type == ConditionBlocks.Type.Else){
                runCondition = true;
            }
            
            //checks for else/else-if
            //sets lastBlock
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

            //tries to find the last block in chain
            int endOfChain = 0;
            //check if next block exists
            if(condition.nextBlock != null){
                //assign nextBlock
                var nextInChain = condition.nextBlock;
                //loop - keep going until nextBlock is null
                while(nextInChain.nextBlock != null){
                    nextInChain = nextInChain.nextBlock;
                }
                endOfChain = BlockEndLineIndex(nextInChain.lineIndex) + 1;
            } else {
                //condition is last in chain
                endOfChain = BlockEndLineIndex(condition.lineIndex) + 1;
            }
            condition.lastEval = runCondition;
            //Debug.Log(runCondition);
            yield return StartCoroutine(HandleCondition(runCondition, lineIndex, stopIndex, endOfChain));
            yield break;
        }

        //test if line is a while loop
        if(currentLine.Split(' ')[0] == "while"){
            string expression = BoolExpression.ProcessBool(currentLine);
            bool runLoop = BoolExpression.GetConditionResult("( " + expression + " )");
            loopEndIndex = BlockEndLineIndex(lineIndex);

            //run while true
            if(runLoop){
                //run until block end index then go back to start of block
                isLooping = true;
                yield return StartCoroutine(secondPass(lineIndex + 1, loopEndIndex + 1));
                yield return StartCoroutine(secondPass(lineIndex, lineIndex));
                yield break;
            } else {
                //run line AFTER block end index
                //when a nested loop ends, this runs
                //find index of nesting brace to prevent shenanigans
                //shenanigans being the loop reading EVERY line after the close brace regardless of nesting
                isLooping = false;
                //to find the terminator of the nesting loop,
                //for loop through codeLines starting at loopEndIndex
                //go until brace depth is -1
                int depth = 0;
                int nestEndIndex = 0;
                for(int i = loopEndIndex + 1; i < codeLines.Length; i++){
                    if(codeLines[i] == "{"){
                        depth++;
                    } else if(codeLines[i] == "}"){
                        depth--;
                        if(depth < 0){
                            nestEndIndex = i + 1;
                            break;
                        }
                    } else if(i == codeLines.Length - 1){
                        nestEndIndex = stopIndex;
                        break;
                    }
                }

                yield return StartCoroutine(secondPass(loopEndIndex + 1, nestEndIndex + 1));
                yield break;
            }
        }

        //test if line is a break statement
        if(lineType == LineType.loopBreak){
            if(isLooping){
                isLooping = false;
                StopAllCoroutines();
                StartCoroutine(secondPass(loopEndIndex + 1, stopIndex));
                yield break;
            } else {
                //if you get here, means you used a break statement outside a loop
                Debug.LogWarning("Uh oh!");
            }
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
                    if(braceDepth > 0){
                        int lastDict = scopeList.Count - 1;
                        scopeList[lastDict].Add(varName, "int");
                    }
                } else {
                    errorChecker.variableDefinedError(currentIndex + 1, varName);
                    return;
                }
                break;
            case "int[]":
                if(!allVars.ContainsKey(varName) && !intArrs.ContainsKey(varName)){
                    //redo and give int its own array dictionary
                    allVars.Add(varName, new VariableInfo(VariableInfo.Type.intVarArr, false));
                    intArrs.Add(varName, new int[0]);
                    if(braceDepth > 0){
                        int lastDict = scopeList.Count - 1;
                        scopeList[lastDict].Add(varName, "intArr");
                    }
                } else {
                    errorChecker.variableDefinedError(currentIndex + 1, varName);
                }
                break;
            case "string":
                if(!allVars.ContainsKey(varName) && !strVars.ContainsKey(varName)){
                    allVars.Add(varName, new VariableInfo(VariableInfo.Type.strVar, false));
                    strVars.Add(varName, null);
                    if(braceDepth > 0){
                        int lastDict = scopeList.Count - 1;
                        scopeList[lastDict].Add(varName, "string");
                    }
                } else {
                    errorChecker.variableDefinedError(currentIndex + 1, varName);
                    return;
                }
                break;
            case "string[]":
                if(!allVars.ContainsKey(varName) && !strArrs.ContainsKey(varName)){
                    //redo and give string its own array dictionary
                    allVars.Add(varName, new VariableInfo(VariableInfo.Type.strVarArr, false));
                    strArrs.Add(varName, new string[0]);
                    if(braceDepth > 0){
                        int lastDict = scopeList.Count - 1;
                        scopeList[lastDict].Add(varName, "strArr");
                    }
                } else {
                    errorChecker.variableDefinedError(currentIndex + 1, varName);
                }
                break;
            case "bool":
                if(!allVars.ContainsKey(varName) && !boolVars.ContainsKey(varName)){
                    allVars.Add(varName, new VariableInfo(VariableInfo.Type.boolVar, false));
                    boolVars.Add(varName, false);
                    if(braceDepth > 0){
                        int lastDict = scopeList.Count - 1;
                        scopeList[lastDict].Add(varName, "bool");
                    }
                } else {
                    errorChecker.variableDefinedError(currentIndex + 1, varName);
                    return;
                }
                break;
            case "bool[]":
                if(!allVars.ContainsKey(varName) && !boolArrs.ContainsKey(varName)){
                    //redo and give int its own array dictionary
                    allVars.Add(varName, new VariableInfo(VariableInfo.Type.boolVarArr, false));
                    boolArrs.Add(varName, new bool[0]);
                    if(braceDepth > 0){
                        int lastDict = scopeList.Count - 1;
                        scopeList[lastDict].Add(varName, "boolArr");
                    }
                } else {
                    errorChecker.variableDefinedError(currentIndex + 1, varName);
                }
                break;
            case "char":
                if(!allVars.ContainsKey(varName) && !charVars.ContainsKey(varName)){
                    allVars.Add(varName, new VariableInfo(VariableInfo.Type.charVar, false));
                    charVars.Add(varName, 'a');
                    if(braceDepth > 0){
                        int lastDict = scopeList.Count - 1;
                        scopeList[lastDict].Add(varName, "char");
                    }
                } else {
                    errorChecker.variableDefinedError(currentIndex + 1, varName);
                }
                break;
            default:
                errorChecker.nonexistentTypeError(currentIndex + 1, varType);
                break;
        }
    }

    private void assignVariable(string line){
        string[] sections = line.Split(' ');
        int opIndex = Array.IndexOf(sections, "=");
        
        string varName = sections[opIndex - 1];

        string varType = opIndex != 1 ? sections[opIndex - 2] : null;
        //it's this that's messing with the boolean expression
        string expression = arrayToString(sections, opIndex + 1);

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
                }
                break;
            case VariableInfo.Type.intVarArr:
                bool initializeArray = true;
                //check if array was already assigned beforehand
                if(allVars[varName].isSet){
                    ClearIntArray(varName);
                }

                //check if using new
                if(sections.Length >= 4 && sections[opIndex + 1] == "new" && sections[opIndex + 2].Contains("int[")){
                    //get value between []
                    string s = sections[opIndex + 2];
                    string indexString = GetSubstring(s, s.IndexOf("[") + 1, s.IndexOf("]"));
                    
                    if(!string.IsNullOrEmpty(indexString)){
                        //check if new is followed by initializer
                        //if not, initialize array with default values
                        if(codeLines[currentIndex + 1] != "{"){
                            initializeArray = false;
                            string formattedLine = CodeFormatter.Format(indexString);
                            int index = new IntExpression(formattedLine).evaluate();
                            intArrs[varName] = new int[index];
                        }
                    }
                }

                //check if being assigned an array variable instead
                List<int> intVals = new List<int>();
                if(intArrs.ContainsKey(expression)){
                    intVals = intArrs[expression].ToList();
                    intArrs[varName] = intVals.ToArray();
                } else if(initializeArray) {
                    string[] arrVals = PrepareArray();
                    for(int i = 0; i < arrVals.Length; i++){
                        var intArrEval = new IntExpression(arrVals[i]);
                        intVals.Add(intArrEval.evaluate());
                    }
                    intArrs[varName] = intVals.ToArray();
                }
                allVars[varName].isSet = true;
                AddArray(varName, intArrs[varName]);
                break;
            case VariableInfo.Type.strVar:
                StringExpression strEval = new StringExpression(expression);
                string strTxt = strEval.removeQuotations();

                //add an array of chars
                if(allVars[varName].isSet){
                    ClearCharArray(varName);
                }
                AddArray(varName, strTxt.ToCharArray());

                strVars[varName] = strTxt;
                allVars[varName].isSet = true;
                break;
            case VariableInfo.Type.strVarArr:
                bool initializeArray2 = true;
                //check if array was already assigned beforehand
                if(allVars[varName].isSet){
                    ClearStringArray(varName);
                }

                //check if using new
                if(sections.Length >= 4 && sections[opIndex + 1] == "new" && sections[opIndex + 2].Contains("string[")){
                    //get value between []
                    string s = sections[opIndex + 2];
                    string indexString = GetSubstring(s, s.IndexOf("[") + 1, s.IndexOf("]"));
                    
                    if(!string.IsNullOrEmpty(indexString)){
                        //check if new is followed by initializer
                        //if not, initialize array with default values
                        if(codeLines[currentIndex + 1] != "{"){
                            initializeArray2 = false;
                            string formattedLine = CodeFormatter.Format(indexString);
                            int index = new IntExpression(formattedLine).evaluate();
                            strArrs[varName] = new string[index];
                        }
                    }
                }

                //check if being assigned an array variable instead
                List<string> strVals = new List<string>();
                if(strArrs.ContainsKey(expression)){
                    strVals = strArrs[expression].ToList();
                    strArrs[varName] = strVals.ToArray();
                } else if(initializeArray2) {
                    string[] arrVals = PrepareArray();
                    for(int i = 0; i < arrVals.Length; i++){
                        string strArrTxt = new StringExpression(arrVals[i]).removeQuotations();
                        strVals.Add(strArrTxt);
                        strArrs[varName] = strVals.ToArray();
                    }
                }
                allVars[varName].isSet = true;
                AddArray(varName, strArrs[varName]);
                break;
            case VariableInfo.Type.boolVar:
                expression = BoolExpression.ProcessBool(expression);
                bool boolVal = BoolExpression.GetConditionResult("( " + expression + " )");
                boolVars[varName] = boolVal;
                allVars[varName].isSet = true;
                break;
            case VariableInfo.Type.boolVarArr:
                bool initializeArray3 = true;
                //check if array was already assigned beforehand
                if(allVars[varName].isSet){
                    ClearBoolArray(varName);
                }

                //check if using new
                if(sections.Length >= 4 && sections[opIndex + 1] == "new" && sections[opIndex + 2].Contains("bool[")){
                    //get value between []
                    string s = sections[opIndex + 2];
                    string indexString = GetSubstring(s, s.IndexOf("[") + 1, s.IndexOf("]"));
                    
                    if(!string.IsNullOrEmpty(indexString)){
                        //check if new is followed by initializer
                        //if not, initialize array with default values
                        if(codeLines[currentIndex + 1] != "{"){
                            initializeArray3 = false;
                            string formattedLine = CodeFormatter.Format(indexString);
                            int index = new IntExpression(formattedLine).evaluate();
                            boolArrs[varName] = new bool[index];
                        }
                    }
                }

                //check if being assigned an array variable instead
                List<bool> boolVals = new List<bool>();
                if(boolArrs.ContainsKey(expression)){
                    boolVals = boolArrs[expression].ToList();
                    boolArrs[varName] = boolVals.ToArray();
                } else if(initializeArray3) {
                    string[] arrVals = PrepareArray();
                    for(int i = 0; i < arrVals.Length; i++){
                        var boolValEval = BoolExpression.GetConditionResult("( " + arrVals[i] + " )");
                        boolVals.Add(boolValEval);
                    }
                    boolArrs[varName] = boolVals.ToArray();
                }
                allVars[varName].isSet = true;
                AddArray(varName, boolArrs[varName]);
                break;
            case VariableInfo.Type.charVar:
                //check if indexer and part of a string first
                if(varName.Contains(ReservedConstants.arrayIndexSeparator)){
                    string charVarName = varName.Substring(0, varName.IndexOf(ReservedConstants.arrayIndexSeparator));
                    if(strVars.ContainsKey(charVarName)){
                        addErr(string.Format("Line {0}: you can't change the characters of a string like this - strings are immutable!", currentIndex + 1));
                        hasError = true;
                        return;
                    }
                }

                //check if variable call
                char chara;
                if(charVars.ContainsKey(expression)){
                    chara = charVars[expression];
                } else {
                    chara = expression.Trim()[1];
                }
                charVars[varName] = chara;
                allVars[varName].isSet = true;
            break;
        }
    }

    string[] PrepareArray(){
        int arrayStartIndex = currentIndex + 2;
        int arrayEndIndex = BlockEndLineIndex(currentIndex);

        //iterate through lines, adding every entry into the array
        //step 1: put all values in the same string, assuming they're on newlines
        List<string> arrValTempString = new List<string>();
        for(int i = arrayStartIndex; i < arrayEndIndex; i++){
            arrValTempString.Add(codeLines[i]);
        }
        string arrayString = arrayToString(arrValTempString.ToArray(), 0).Trim();
        
        //step 2: isolate values from spaces and commas
        string[] arrVals = arrayString.Split(' ');
        arrVals = arrayToString(arrVals, 0).Split(',');
        for(int i = 0; i < arrVals.Length; i++){
            arrVals[i] = arrVals[i].Trim();
        }

        return arrVals;
    }

    void AddArray(string varName, int[] varValues){
        for(int i = 0; i < varValues.Length; i++){
            string arrVarName = varName + ReservedConstants.arrayIndexSeparator + i;
            allVars.Add(arrVarName, new VariableInfo(VariableInfo.Type.intVar, true));
            intVars.Add(arrVarName, varValues[i]);
        }
    }

    void ClearIntArray(string varName){
        for(int i = 0; i < intArrs[varName].Length; i++){
            string arrVarName = varName + ReservedConstants.arrayIndexSeparator + i;
            allVars.Remove(arrVarName);
            intVars.Remove(arrVarName);
        }
    }

    void AddArray(string varName, string[] varValues){
        for(int i = 0; i < varValues.Length; i++){
            string arrVarName = varName + ReservedConstants.arrayIndexSeparator + i;
            allVars.Add(arrVarName, new VariableInfo(VariableInfo.Type.strVar, true));
            strVars.Add(arrVarName, varValues[i]);
        }
    }

    void ClearStringArray(string varName){
        for(int i = 0; i < strArrs[varName].Length; i++){
            string arrVarName = varName + ReservedConstants.arrayIndexSeparator + i;
            allVars.Remove(arrVarName);
            strVars.Remove(arrVarName);
        }
    }

    void AddArray(string varName, bool[] varValues){
        for(int i = 0; i < varValues.Length; i++){
            string arrVarName = varName + ReservedConstants.arrayIndexSeparator + i;
            allVars.Add(arrVarName, new VariableInfo(VariableInfo.Type.boolVar, true));
            boolVars.Add(arrVarName, varValues[i]);
        }
    }
    
    void ClearBoolArray(string varName){
        for(int i = 0; i < boolArrs[varName].Length; i++){
            string arrVarName = varName + ReservedConstants.arrayIndexSeparator + i;
            allVars.Remove(arrVarName);
            boolVars.Remove(arrVarName);
        }
    }

    void AddArray(string varName, char[] varValues){
        for(int i = 0; i < varValues.Length; i++){
            string arrVarName = varName + ReservedConstants.arrayIndexSeparator + i;
            allVars.Add(arrVarName, new VariableInfo(VariableInfo.Type.charVar, true));
            charVars.Add(arrVarName, varValues[i]);
        }
    }

    void ClearCharArray(string varName){
        for(int i = 0; i < strVars[varName].Length; i++){
            string arrVarName = varName + ReservedConstants.arrayIndexSeparator + i;
            allVars.Remove(arrVarName);
            charVars.Remove(arrVarName);
        }
    }

    private string CheckForIndex(string[] sections){
        for(int i = 0; i < sections.Length; i++){
            if(sections[i].Contains("[") && sections[i].Contains("]")){
                //get substring of everything before "[]";
                string indexId = GetSubstring(sections[i], 0, sections[i].IndexOf("["));
                if(ReservedConstants.varTypes.Contains(indexId)){
                    return arrayToString(sections, 0);
                }

                //get substring of everything between "[]"
                string indexString = GetSubstring(sections[i], sections[i].IndexOf("[") + 1, sections[i].IndexOf("]"));

                //format index value string and evaluate to get the index number
                string formattedLine = CodeFormatter.Format(indexString);
                int index = new IntExpression(formattedLine).evaluate();

                //get the variable (array) name
                sections[i] = sections[i].Substring(0, sections[i].IndexOf("[")) + ReservedConstants.arrayIndexSeparator + index;

                //now check if it exists
                //if it doesn't, index is out of bounds
                if(!allVars.ContainsKey(sections[i])){
                    ///addErr(string.Format("Line {0}: array index is out of the array's bounds!", currentIndex + 1));
                    hasError = true;
                    killTimer();
                    errorChecker.writeError();
                }
            }
        }
        return arrayToString(sections, 0);
    }

    private string CheckForRead(string[] sections){
        //recur to check
        List<string> sectionList = sections.ToList();
        Dictionary<string, int> indices = new Dictionary<string, int>{
            { "read", sectionList.Contains("read") ? sectionList.IndexOf("read") : 999 },
            { "readInt", sectionList.Contains("readInt") ? sectionList.IndexOf("readInt") : 999 },
            { "readBool", sectionList.Contains("readBool") ? sectionList.IndexOf("readBool") : 999 },
            { "readChar", sectionList.Contains("readChar") ? sectionList.IndexOf("readChar") : 999 },
            { "readDouble", sectionList.Contains("readDouble") ? sectionList.IndexOf("readDouble") : 999 }
        };

        //gets the leftmost function if multiple read functions are detected
        var sortedDict = from entry in indices orderby entry.Value ascending select entry;
        var first = sortedDict.First();
        string funcType = first.Key;
        int index = first.Value;

        //checks that read is a function call and not just some random variable
        //or if this line has an error
        bool isInThirdIndex = index >= 2;
        bool isNotLastIndex = index < sections.Length - 1;
        bool matchesFormat = false;
        if(isInThirdIndex && isNotLastIndex){
            matchesFormat = sectionList[index - 2] == "Bot" && sectionList[index - 1] == "." && sectionList[index + 1] == "(";
        }

        if(!matchesFormat){
            return arrayToString(sections, 0);
        }

        int botIndex = index - 2;
        int depth = 0;
        int endIndex = 0;
        //find the where the read function closes
        for(int i = botIndex; i < sections.Length; i++){
            if(sections[i] == "("){
                depth++;
            } else if(sections[i] == ")"){
                depth--;
                if(depth == 0){                    
                    endIndex = i;
                    break;
                }
            }
        }

        //get the function in its entirety
        //then hand it over to functionHandler
        List<string> funcString = new List<string>();
        for(int i = botIndex; i <= endIndex; i++){
            funcString.Add(sections[i]);
        }
        functionHandler.initializeHandler(arrayToString(funcString.ToArray(), 0), currentIndex + 1);
        if(functionHandler.hasError){
            hasError = true;
        }

        switch(funcType){
            case "read":
                sectionList.RemoveRange(botIndex, endIndex - botIndex + 1);
                sectionList.Insert(botIndex, "\"" + playerAct.read() + "\"");
            break;
            case "readInt":
                sectionList.RemoveRange(botIndex, endIndex - botIndex + 1);
                sectionList.Insert(botIndex, playerAct.readInt().ToString());
            break;
            case "readBool":
                sectionList.RemoveRange(botIndex, endIndex - botIndex + 1);
                sectionList.Insert(botIndex, playerAct.readBool().ToString());
            break;
        }

        string result = arrayToString(sectionList.ToArray(), 0);
        if(result.Contains("read") || result.Contains("readInt") || result.Contains("readBool")){
            result = CheckForRead(result.Split(' '));
        }
        return result;
    }

    private string CheckForReadArray(string[] sections){
        //recur to check
        List<string> sectionList = sections.ToList();
        Dictionary<string, int> indices = new Dictionary<string, int>{
            { "ReadStringArr", sectionList.Contains("ReadStringArr") ? sectionList.IndexOf("ReadStringArr") : 999 },
            { "ReadIntArr", sectionList.Contains("ReadIntArr") ? sectionList.IndexOf("ReadIntArr") : 999 },
            { "ReadBoolArr", sectionList.Contains("ReadBoolArr") ? sectionList.IndexOf("ReadBoolArr") : 999 },
            { "ReadCharArr", sectionList.Contains("ReadCharArr") ? sectionList.IndexOf("ReadCharArr") : 999 },
            { "ReadDoubleArr", sectionList.Contains("ReadDoubleArr") ? sectionList.IndexOf("ReadDoubleArr") : 999 }
        };

        //gets the leftmost function if multiple read functions are detected
        var sortedDict = from entry in indices orderby entry.Value ascending select entry;
        var first = sortedDict.First();
        string funcType = first.Key;
        int index = first.Value;

        //checks that read is a function call and not just some random variable
        //or if this line has an error
        bool isInThirdIndex = index >= 2;
        bool isNotLastIndex = index < sections.Length - 1;
        bool matchesFormat = false;
        if(isInThirdIndex && isNotLastIndex){
            matchesFormat = sectionList[index - 2] == "Bot" && sectionList[index - 1] == "." && sectionList[index + 1] == "(";
        }

        if(!matchesFormat){
            return arrayToString(sections, 0);
        }

        int botIndex = index - 2;
        int depth = 0;
        int endIndex = 0;
        //find the where the read function closes
        for(int i = botIndex; i < sections.Length; i++){
            if(sections[i] == "("){
                depth++;
            } else if(sections[i] == ")"){
                depth--;
                if(depth == 0){                    
                    endIndex = i;
                    break;
                }
            }
        }

        //get the function in its entirety
        //then hand it over to functionHandler
        List<string> funcString = new List<string>();
        for(int i = botIndex; i <= endIndex; i++){
            funcString.Add(sections[i]);
        }
        functionHandler.initializeHandler(arrayToString(funcString.ToArray(), 0), currentIndex + 1);
        if(functionHandler.hasError){
            hasError = true;
        }

        switch(funcType){
            case "ReadStringArr":
                sectionList.RemoveRange(botIndex, endIndex - botIndex + 1);
                strArrs["$readArr"] = playerAct.ReadStringArr();
                sectionList.Insert(botIndex, "$readArr");
            break;
            case "ReadIntArr":
                sectionList.RemoveRange(botIndex, endIndex - botIndex + 1);
                intArrs["$readIntArr"] = playerAct.ReadIntArr();
                sectionList.Insert(botIndex, "$readIntArr");
            break;
            case "ReadBoolArr":
                sectionList.RemoveRange(botIndex, endIndex - botIndex + 1);
                boolArrs["$readBoolArr"] = playerAct.ReadBoolArr();
                sectionList.Insert(botIndex, "$readBoolArr");
            break;
        }

        string result = arrayToString(sectionList.ToArray(), 0);
        if(result.Contains("ReadStringArr") || result.Contains("ReadIntArr") || result.Contains("ReadBoolArr")){
            result = CheckForReadArray(result.Split(' '));
        }
        return result;
    }

    private string CheckForCube(string[] sections, bool isFirstPass){
        List<string> sectionList = sections.ToList();

        int callIndex = sectionList.IndexOf("CheckCube");

        //check if actual function call or just a var
        bool isInThirdIndex = callIndex >= 2;
        bool isNotLastIndex = callIndex < sections.Length - 1;
        bool matchesFormat = false;
        if(isInThirdIndex && isNotLastIndex){
            matchesFormat = sectionList[callIndex - 2] == "Bot" && sectionList[callIndex - 1] == "." && sectionList[callIndex + 1] == "(";
        }

        if(!matchesFormat){
            return arrayToString(sections, 0);
        }

        int botIndex = callIndex - 2;
        int depth = 0;
        int endIndex = 0;
        //find the where the read function closes
        for(int i = botIndex; i < sections.Length; i++){
            if(sections[i] == "("){
                depth++;
            } else if(sections[i] == ")"){
                depth--;
                if(depth == 0){                    
                    endIndex = i;
                    break;
                }
            }
        }

        //get the function in its entirety
        //then hand it over to functionHandler
        List<string> funcString = new List<string>();
        for(int i = botIndex; i <= endIndex; i++){
            funcString.Add(sections[i]);
        }
        functionHandler.initializeHandler(arrayToString(funcString.ToArray(), 0), currentIndex + 1);
        if(functionHandler.hasError){
            hasError = true;
            sectionList.RemoveRange(botIndex, endIndex - botIndex + 1);
            sectionList.Insert(botIndex, "false");
            string failRes = arrayToString(sectionList.ToArray(), 0);
            return failRes;
        }

        //get arguments and fire function
        string argVal = new StringExpression(functionHandler.getPassedArgs[0]).removeQuotations();
        string boolResult = playerAct.CheckCube(argVal.ToLower(), isFirstPass).ToString().ToLower();

        sectionList.RemoveRange(botIndex, endIndex - botIndex + 1);
        sectionList.Insert(botIndex, boolResult);
        string result = arrayToString(sectionList.ToArray(), 0);
        
        if(result.Contains("CheckCube")){
            result = CheckForCube(result.Split(' '), isFirstPass);
        }
        return result;
    }

    private string CheckForArrLength(string[] sections){
        List<string> sectionList = sections.ToList();
        int lenIndex = sectionList.IndexOf("Length");

        //check if Length property
        bool isInThirdIndex = lenIndex >= 2;
        bool matchesFormat = false;
        if(isInThirdIndex){
            matchesFormat = sectionList[lenIndex - 1] == ".";
        }

        if(!matchesFormat){
            return arrayToString(sections, 0);
        }

        int varNameIndex = lenIndex - 2;
        string varName = sections[varNameIndex];
        
        if(!allVars.ContainsKey(varName)){
            return arrayToString(sections, 0);
        }
        var type = allVars[varName].type;
        
        if(!(type == VariableInfo.Type.boolVarArr || type == VariableInfo.Type.intVarArr || type == VariableInfo.Type.strVarArr)){
            return arrayToString(sections, 0);
        }

        sectionList.RemoveRange(lenIndex - 2, 3);
        if(type == VariableInfo.Type.intVarArr){
            sectionList.Insert(lenIndex - 2, intArrs[varName].Length.ToString());
        } else if(type == VariableInfo.Type.strVarArr){
            sectionList.Insert(lenIndex - 2, strArrs[varName].Length.ToString());
        } else if(type == VariableInfo.Type.boolVarArr){
            sectionList.Insert(lenIndex - 2, boolArrs[varName].Length.ToString());
        }
        return arrayToString(sectionList.ToArray(), 0);
    }

    private IEnumerator HandleCondition(bool condition, int lineIndex, int stopIndex, int endOfChain){
        if(condition){
            //run until it hits the end index
            //then skip to the end of the chain
            int conditionStopIndex = BlockEndLineIndex(lineIndex);
            yield return StartCoroutine(secondPass(lineIndex + 1, conditionStopIndex));
            yield return StartCoroutine(secondPass(endOfChain, stopIndex));
        } else {
            int conditionStopIndex = BlockEndLineIndex(lineIndex);
            yield return StartCoroutine(secondPass(conditionStopIndex + 1, stopIndex));
        }
    }

    public int BlockEndLineIndex(int blockStartIndex){
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
        //detect if lastindex is semicolon
        if(Array.LastIndexOf(array, ";") == array.Length - 1){
            //then move semicolon outside the parentheses
            List<string> tempList = new List<string>();
            for(int i = 0; i < array.Length - 1; i++){
                tempList.Add(array[i]);
            }
            array = tempList.ToArray();
            return string.Format("{0} = {0} {1} ( {2} ) ;", array[0], opChar, arrayToString(array, 2));
        }
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
        hasError = false;
        hasArgError = false;
        braceDepth = 0;
        clearDictionaries(true);
        EditorSaveLoad.Instance.SaveEditorState();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public static bool isResetSuccessful(){
        if(SceneManager.GetActiveScene().isDirty){
            return false;
        }
        return true;
    }

    public void clearDictionaries(bool clearConditions){
        allVars.Clear();
        intVars.Clear();
        strVars.Clear();
        boolVars.Clear();
        charVars.Clear();
        doubleVars.Clear();
        intArrs.Clear();
        strArrs.Clear();
        boolArrs.Clear();
        charArrs.Clear();
        doubleArrs.Clear();
        reserveDictionaries();
        if(clearConditions){
            conditionByIndex.Clear();
        }
    }

    public int reservedBools = 0;
    public int reservedInts = 0;
    public int reservedStrings = 0;
    private void reserveDictionaries(){
        allVars.Add("$readArr", new VariableInfo(VariableInfo.Type.strVarArr, true));
        strArrs.Add("$readArr", new string[0]);
        allVars.Add("$readIntArr", new VariableInfo(VariableInfo.Type.intVarArr, true));
        intArrs.Add("$readIntArr", new int[0]);
        allVars.Add("$readBoolArr", new VariableInfo(VariableInfo.Type.boolVarArr, true));
        boolArrs.Add("$readBoolArr", new bool[0]);
        
        reservedVars = allVars.Count;
        reservedBools = boolVars.Count;
        reservedInts = intVars.Count;
        reservedStrings = strVars.Count;
    }

    //called by anything that should stop execution
    //invalid direction errors, stepping on a trap etc
    public void terminateExecution(){
        clearDictionaries(true);
        Invoke("killTimer", Globals.Instance.timePerStep);
        Debug.Log("Execution terminated");
    }

    //delays the global coroutine stop by the global time to move
    public void killTimer(){
        /*foreach(var o in FindObjectsOfType<MonoBehaviour>()){
            o.StopAllCoroutines();
        }*/
        functionHandler.StopAllCoroutines();
        playerAct.StopAllCoroutines();
        playerMove.StopAllCoroutines();
        StopAllCoroutines();
    }

    public void KillAll(){
        foreach(var o in FindObjectsOfType<MonoBehaviour>()){
            o.StopAllCoroutines();
        }
    }

    public void addErr(string msg){
        string[] errorMsg = {msg};        
        errorChecker.errorConvo.dialogueBlocks.Add(new Dialogue(errorMsg, "ERROR", 'R', true, errorChecker.errorSprite));
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
    public enum Type {
        intVar,
        intVarArr,
        strVar,
        strVarArr,
        boolVar,
        boolVarArr,
        floatVar,
        floatVarArr,
        doubleVar,
        doubleVarArr,
        charVar,
        charVarArr
    }
    public VariableInfo(Type type, bool isSet){
        this.type = type;
        this.isSet = isSet;
    }
    public Type type;
    public bool isSet;
}