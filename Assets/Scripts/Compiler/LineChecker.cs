using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text.RegularExpressions;

public class LineChecker{
    public LineType lineType = LineType.none;
    public bool hasError = false;
    Dictionary<string, VariableInfo> allVars;
    int lineIndex;
    string line;
    ErrorChecker errorChecker;

    public LineChecker(string inputLine, int index, ErrorChecker err){
        allVars = Compiler.Instance.allVars;
        lineIndex = index;
        errorChecker = err;
        line = inputLine;
        
        readTokens(line);
    }
    
    private void readTokens(string inputLine){
        bool isCondLoop = false;
        //cant check for nonexistent types yet
        //non-existent var checking happens in runtime
        //place checkers in in the expression classes
        if(string.IsNullOrEmpty(inputLine)){
            return;
        }
        
        string[] sections = inputLine.Split(' ');

        //check a curly brace
        if(sections[0] == "{"){
            lineType = LineType.openBrace;
            //previous line should be a conditional, iterative, or array initializer
            if(!Flags.Instance.isExpectingBrace){
                addErr(string.Format("Line {0}: Open braces must come after a statement requiring them, like if-else statements or looping statements!", lineIndex));
                hasError = true;
                return;
            }
            if(Flags.Instance.isArray){
                Flags.Instance.isInArray = true;
            }
            Flags.Instance.isExpectingBrace = false;
        }
        else if(sections[0] == "}"){
            lineType = LineType.closeBrace;
            //triggers array checker if flags are set
            if(Flags.Instance.isArray && Flags.Instance.isInArray){
                Flags.Instance.isArray = false;
                Flags.Instance.isInArray = false;
                
                if(!ArrayProcessor(Flags.Instance.arrayElements)){
                    Flags.Instance.arrayElements.Clear();
                    hasError = true;
                    return;
                }
                Flags.Instance.arrayElements.Clear();

                if(!CheckSemicolon(sections)){
                    hasError = true;
                    return;
                }
            }
        }
        //brach is reached only if line is in array declaration
        //overrides everything else and has its own rules
        else if(Flags.Instance.isInArray){
            Flags.Instance.arrayElements.Add(inputLine);
        }
        //checks for an array declaration
        else if(ReservedConstants.arrVarTypes.Contains(sections[0])){
            if(!checkInitialize(sections)){
                hasError = true;
                return;
            }
            Flags.Instance.isArray = true;
            Flags.Instance.isInArray = true;
            Flags.Instance.isExpectingBrace = true;

            if(sections[0] == "int[]"){
                Flags.Instance.arrType = ValueType.intVal;
            } else if(sections[0] == "string[]"){
                Flags.Instance.arrType = ValueType.strVal;
            } else if(sections[0] == "bool[]"){
                Flags.Instance.arrType = ValueType.boolVal;
            }
        }
        //checks for variable initialization
        //to add a new data type, add it in ReservedConstants.varTypes
        else if(ReservedConstants.varTypes.Contains(sections[0])){
            if(!checkInitialize(sections)){
                hasError = true;
                return;
            }
        }
        //checks for a call using "Bot."
        else if(sections[0] == "Bot"){
            if(checkBotCall(sections)){
                lineType = LineType.functionCall;
            } else {
                hasError = true;
                return;
            }
        }
        //checks for variable assignment
        else if(isValidVarName(sections[0])){
            //first check if variable is an array index
            if(sections[0].Contains(ReservedConstants.arrayIndexSeparator)){
                if(!CheckArrayIndex(sections[0])){
                    hasError = true;
                    return;
                }
            }
            //check if the variable exists
            else if(!allVars.ContainsKey(sections[0])){
                addErr(string.Format("Line {0}: variable \"{1}\" does not exist!", lineIndex, sections[0]));
                hasError = true;
                return;
            }

            if(checkAssignment(sections, sections[0])){
                lineType = LineType.varAssign;
            } else {
                hasError = true;
                return;
            }
        }
        //checks for a conditional statement
        else if(sections[0] == "if" || sections[0] == "else"){
            isCondLoop = true;
            if(sections[0] == "if"){
                if(CheckLoopOrCondition(sections, lineIndex, "if")){
                    lineType = LineType.conditional;
                } else {
                    hasError = true;
                    return;
                }
            } else if(sections[0] == "else"){
                //check if multiple tokens
                if(sections.Length > 1){
                    //check if next token is if
                    if(sections[1] == "if"){
                        if(CheckLoopOrCondition(sections, lineIndex, "elseif")){
                            lineType = LineType.conditional;
                        } else {
                            addErr(string.Format("Line {0}: \"if\" should follow \"else\" if you're trying to make an else-if statement!", lineIndex));
                            hasError = true;
                            return;
                        }
                    } else {
                        addErr(string.Format("Line {0}: \"if\" should follow \"else\" if you're trying to make an else-if statement!", lineIndex));
                        hasError = true;
                        return;
                    }
                } else {
                    Flags.Instance.isExpectingBrace = true;
                }
            }
        }
        //checks for an iterative statement
        else if(sections[0] == "while"){
            isCondLoop = true;
            if(CheckLoopOrCondition(sections, lineIndex, "while")){
                lineType = LineType.loop;
            } else {
                hasError = true;
                return;
            }
        } else if(sections[0] == ";"){
            /*Debug.Log(Flags.Instance.isExpectingSemicolon);
            if(!Flags.Instance.isExpectingSemicolon){
                Debug.Log("oh no");
                hasError = true;
                return;
            }*/
        }
        //default for all
        else {
            addErr(string.Format("Line {0}: only assignment, call, increment, or decrement can be used as statements!", lineIndex));
            hasError = true;
            return;
        }

        if(!(Flags.Instance.isArray || Flags.Instance.isInArray || isCondLoop || sections[0] == "{" || sections[0] == "}")){
            if(!CheckSemicolon(sections)){
                hasError = true;
                return;
            }
        }
    }

    private bool CheckSemicolon(string[] sections){
        //checks for a semicolon at the end
        //exclusionary clause: arrays or curly braces
        //or the first lines of conditional and looping statements
        if(sections.Length > 1){
            if(sections[sections.Length - 1] != ";"){
                addErr(string.Format("Line {0}: don't forget to add semicolons at the end of your statements!", lineIndex));
                return false;
            }
        }

        //checks if there are elements after the first semicolon
        //if there are, throws an error
        if(sections.Contains(";") && Array.IndexOf(sections, ";") != sections.Length - 1){
            addErr(string.Format("Line {0}: semicolons are meant to terminate statements - put each command on a separate line!", lineIndex));
            return false;
        }

        return true;
    }

    //TODO: Expand to allow supporting multiple initialization
    private bool checkInitialize(string[] sections){
        Debug.Log("made it to checkInitialize");
        if(sections.Length == 1){
            Debug.Log("line 155");
            addErr(string.Format("Line{0}: identifier expected!", lineIndex));
            return false;
        }

        if(!isValidVarName(sections[1])){
            Debug.Log("line 161");
            return false;
        } else if(allVars.ContainsKey(sections[1])){
            Debug.Log("line 164");
            addErr(string.Format("Line {0}: {1} is already defined!", lineIndex, sections[1]));
            return false;
        }

        if(sections.Length == 2){
            Debug.Log("line 170");
            addErr(string.Format("Line {0}: ';' or '=' expected!", lineIndex));
            return false;
        }

        //initializes AND assigns
        if(sections[2] == "="){
            if(!checkExpression(sections, 3)){
                Debug.Log("line 178");
                return false;
            } else {
                Debug.Log("line 181");
                string expression = Compiler.arrayToString(sections, 3);
                switch(sections[0]){
                    case "int":
                        if(CheckTypes(expression) != ValueType.intVal){
                            addErr(string.Format("Line {0}: provided value is not an int!", lineIndex));
                            return false;
                        }
                        break;
                    case "string":
                        if(CheckTypes(expression) != ValueType.strVal){
                            addErr(string.Format("Line {0}: provided value is not a string!", lineIndex));
                            return false;
                        }
                    break;
                    case "bool":
                        if(CheckTypes(expression) != ValueType.boolVal){
                            addErr(string.Format("Line {0}: provided value is not a bool!", lineIndex));
                            return false;
                        }
                    break;
                    case "int[]":
                        //check the sequence later
                        //AddArray() in Compiler won't check it for you
                    break;
                }
                lineType = LineType.varAssign;
            }
        } else {
            //check next token - should be a semicolon
            if(sections[2] == ";" && sections.Length == 3){
                lineType = LineType.varInitialize;
            } else if(sections[2] == ";" && sections.Length > 3) {
                addErr(string.Format("Line {0}: unexpected token {1}!", lineIndex, sections[3]));
                
                //still allows initialization but is technically an error
                //just so everything else that comes after that calls a techincally correct variable doesn't throw an error
                lineType = LineType.varInitialize;
                return false;
            } else {
                addErr(string.Format("Line {0}: unexpected token {1}, expected \";\"!", lineIndex, sections[2]));
                return false;
            }
        }

        return true;
    }

    private bool checkAssignment(string[] sections, string varName){
        if(allVars.ContainsKey(varName)){
            if(sections[1] == "="){
                if(!checkExpression(sections, 2)){
                    return false;
                }
                string expression = Compiler.arrayToString(sections, 2);
                if(Compiler.Instance.intVars.ContainsKey(sections[0])){
                    if(CheckTypes(expression) != ValueType.intVal){
                        addErr(string.Format("Line {0}: provided value is not an int!", lineIndex));
                        return false;
                    }
                } else if(Compiler.Instance.strVars.ContainsKey(sections[0])){
                    if(CheckTypes(expression) != ValueType.strVal){
                        addErr(string.Format("Line {0}: provided value is not a string!", lineIndex));
                        return false;
                    }
                } else if(Compiler.Instance.boolVars.ContainsKey(sections[0])){
                    if(CheckTypes(expression) != ValueType.boolVal){
                        addErr(string.Format("Line {0}: provided value is not a bool!", lineIndex));
                        return false;
                    }
                }
            } else if(sections[1] == ";"){
                addErr(string.Format("Line {0}: only assignment, call, increment, or decrement can be used as statements!", lineIndex));
                return false;
            } else {
                addErr(string.Format("Line {0}: unexpected token {1}!", lineIndex, sections[1]));
                return false;
            }
        } else {
            addErr(string.Format("Line {0}: variable {1} does not exist!", lineIndex, varName));
            return false;
        }
        return true;
    }

    //checks for botty call
    private bool checkBotCall(string[] sections){
        if(sections[1] == "."){
            //checks if function name exists in list of valid functions
            if(!FunctionHandler.builtInFunctions.Contains(sections[2])){
                addErr(string.Format("Line {0}: Bot has no definition for {1}!", lineIndex, sections[2]));
                return false;
            }
            if(sections[3] != "("){
                //name is valid but no parentheses
                addErr(string.Format("Line {0}: {1} is a method - add \"()\" after it!", lineIndex, sections[2]));
                return false;
            }

            //checks parenthesis depth
            if(!CheckDepth(sections)){
                return false;
            }
            
            if(!checkExpression(sections, 3)){
                return false;
            }
        } else {
            addErr(string.Format("Line {0}: unexpected token {1}, was expecting a \".\"!", lineIndex, sections[1]));
            return false;
        }
        return true;
    }

    private bool CheckLoopOrCondition(string[] sections, int lineIndex, string type){
        //check the next line for a "{"
        string nextLine = Compiler.Instance.getCodeLines.Length > 1 ? Compiler.Instance.getCodeLines[lineIndex] : null;
        if(nextLine != "{"){
            addErr(string.Format("Line {0}: was expecting a \"{\"!", lineIndex));
            hasError = true;
            return false;
        }
        Flags.Instance.isExpectingBrace = true;

        int secLength = 0;
        int groupIndex = 0;
        int expIndex = 0;
        if(type == "if" || type == "while"){
            secLength = 2;
            groupIndex = 1;
            expIndex = 1;
        }
        if(type == "elseif"){
            secLength = 3;
            groupIndex = 2;
            expIndex = 2;
        }

        if(sections.Length < secLength){
            addErr(string.Format("Line {0}: was expecting a \"(\"!", lineIndex));
            return false;
        }
        if(sections[groupIndex] != "("){
            addErr(string.Format("Line {0}: was expecting a \"(\"!", lineIndex));
            return false;
        }

        //check if parentheses are closed
        if(!CheckDepth(sections)){
            return false;
        }
        //check if argument string is empty
        int startIndex = line.IndexOf("(") + 1;
        string argString = line.Substring(startIndex, line.LastIndexOf(")") - (startIndex)).Trim();
        if(string.IsNullOrEmpty(argString)){
            addErr(string.Format("Line {0}: conditional statements cannot be empty!", lineIndex));
            return false;
        }
        //check argument string validity
        if(!checkExpression(sections, expIndex)){
            //error handled by checkExpression
            return false;
        }

        string expression = Compiler.arrayToString(sections, groupIndex + 1);
        expression = expression.Substring(0, expression.LastIndexOf(")")).Trim();
        if(CheckTypes(expression) != ValueType.boolVal){
            addErr(string.Format("Line {0}: conditional statements must be a boolean!", lineIndex));
            return false;
        }
        
        return true;
    }

    //checks numerical expression validity
    private bool checkExpression(string[] sections, int index){
        bool isExpectingValue = true;
        //everything else should be part of the expression
        //TODO: detect comparison or boolean operators next to parentheses
        
        for(int i = index; i < sections.Length; i++){
            //error: math operator right after an "=" sign
            //might beome obsolete once improvement to checker is complete
            if(ReservedConstants.mathOperators.Contains(sections[i])){
                if(sections[i - 1] == "="){
                    //Debug.Log("unexpected token error");
                    addErr(string.Format("Line {0}: unexpected token {1}!", lineIndex, sections[i]));
                    return false;
                } else {
                    isExpectingValue = true;
                }
            }

            if(isExpectingValue && Regex.IsMatch(sections[i], @"^[0-9]+$")){
                isExpectingValue = false;
            }

            //dont add parentheses to the pattern
            //if you do this thing won't read expressions right
            //because then it'll flip isExpectingValue and you're screwed
            //Debug.Log(sections[i]);
            if(isExpectingValue && Regex.IsMatch(sections[i], @"^[a-zA-Z0-9_`]+$")){
                //Debug.Log("matches regex net");
                if(sections[i] == "true" || sections[i] == "false"){
                    //do nothing
                    //this is in here so the compiler can process boolean assignments
                    //removing this makes the interpreter treat true/false values as numerical expressions
                    //which subjects it to the same rules and errors as math expressions
                } else if(isValidVarName(sections[i])){
                    //check if it's an array index
                    if(sections[i].Contains('`')){
                        Debug.Log(sections[i]);
                        if(!CheckArrayIndex(sections[i])){
                            Debug.Log("got here");
                            return false;
                        }
                    }
                    
                    if(allVars.ContainsKey(sections[i])){
                        isExpectingValue = false;
                    } else {
                        addErr(string.Format("Line {0}: variable {1} does not exist!", lineIndex, sections[i]));
                        return false;
                    }
                } else {
                    addErr(string.Format("Line {0}: unexpected token {1}!", lineIndex, sections[i]));
                    return false;
                }
            }
        }

        //check sequence of tokens
        if(!CheckSequence(sections, index)){
            return false;
        }
        return true;
    }

    private bool CheckSequence(string[] sections, int index){
        string expressionString = Compiler.arrayToString(sections, index);
        string[] expSections = expressionString.Split(' ');
        List<ValueType> values = EnumAdder(expSections);
        bool expectValue = true;
        bool expectOperator = false;
        bool expectComma = false;
        bool expectStartGroup = true;
        bool expectEndGroup = false;
        
        int depth = 0;
        for(int i = 0; i < expSections.Length; i++){
            //int wah = 9 10;
            ValueType val = values[i];
            if(val == ValueType.intVal || val == ValueType.strVal || val == ValueType.boolVal){
                if(!expectValue){
                    addErr(string.Format("Line {0}: unexpected token {1}! Was expecting an operator, \")\", or \";\"!)", lineIndex, expSections[i]));
                    return false;
                }

                expectValue = false;
                expectOperator = true;
                expectComma = true;
                expectStartGroup = false;
                expectEndGroup = true;
            } else if(val == ValueType.mathOp || val == ValueType.cmprOp || val == ValueType.boolOp){
                if(!expectOperator){
                    Debug.LogWarning("ERROR!");
                    return false;
                }

                expectValue = true;
                expectOperator = false;
                expectComma = false;
                expectStartGroup = true;
                expectEndGroup = false;
            } else if(val == ValueType.startGroup){
                if(!expectStartGroup){
                    Debug.LogWarning("ERROR!");
                    return false;
                }

                expectValue = true;
                expectOperator = false;
                expectComma = false;
                expectStartGroup = true;
                expectEndGroup = false;

                depth++;
            } else if(val == ValueType.endGroup){
                if(!expectEndGroup){
                    Debug.LogWarning("ERROR!");
                    return false;
                }

                expectValue = false;
                expectOperator = true;
                expectComma = true;
                expectStartGroup = false;
                expectEndGroup = true;

                depth--;
            } else if(val == ValueType.comma){
                if(!expectComma){
                    Debug.LogWarning("ERROR!");
                    return false;
                }
                if(depth > 0){
                    Debug.LogWarning("Commas cannot be between () in an argument!");
                }

                expectValue = true;
                expectOperator = false;
                expectComma = false;
                expectStartGroup = true;
                expectEndGroup = false;
            }
        }
        return true;
    }
    
    private ValueType CheckTypes(string expression){
        List<ValueType> val = EvaluateGroups(EnumAdder(expression.Split(' ')));
        val = EvaluateList(val);
        return val[0];
    }

    private List<ValueType> EvaluateGroups(List<ValueType> values){
        //check if values has only one element or no groups
        if(values.Count == 1 || !values.Contains(ValueType.startGroup)){
            return values;
        }

        //recursively isolate/evaluate parentheses groups
        int startIndex = 0;
        int endIndex = 0;
        for(int i = 0; i < values.Count; i++){      
            if(values[i] == ValueType.startGroup){
                startIndex = i;
            } else if(values[i] == ValueType.endGroup){
                endIndex = i;
                break;
            }
        }
        
        //get sublist for parentheses
        List<ValueType> valueSubList = new List<ValueType>();
        for(int i = startIndex + 1; i < endIndex; i++){
            valueSubList.Add(values[i]);
        }
        
        //iterate through sublist to look for operators
        valueSubList = EvaluateList(valueSubList);

        //replace in value list
        values.RemoveRange(startIndex, endIndex - startIndex + 1);
        values.Insert(startIndex, valueSubList[0]);

        //recur
        if(values.Contains(ValueType.startGroup)){
            EvaluateGroups(values);
        }

        return values;
    }

    private List<ValueType> EvaluateList(List<ValueType> values){
        int[] wah = {};
        while(values.Contains(ValueType.mathOp)){
            for(int i = 0; i < values.Count; i++){
                if(values[i] == ValueType.mathOp){
                    ValueType a = values[i - 1];
                    ValueType b = values[i + 1];
                    if(a == ValueType.intVal && b == ValueType.intVal){
                        //correct
                        values.RemoveRange(i - 1, 3);
                        values.Insert(i - 1, ValueType.intVal);
                        break;
                    } else if(a == ValueType.strVal && b == ValueType.strVal){
                        //also correct
                        values.RemoveRange(i - 1, 3);
                        values.Insert(i - 1, ValueType.strVal);
                        break;
                    } else {
                        values.RemoveRange(i - 1, 3);
                        values.Insert(i - 1, ValueType.mismatch);
                        Debug.LogWarning("ERROR!");
                        break;
                    }
                }
            }
        }
        while(values.Contains(ValueType.cmprOp)){
            Console.WriteLine("checking for cmprOps");
            for(int i = 0; i < values.Count; i++){
                if(values[i] == ValueType.cmprOp){
                    ValueType a = values[i - 1];
                    ValueType b = values[i + 1];
                    if(a == ValueType.intVal && b == ValueType.intVal){
                        //correct
                        values.RemoveRange(i - 1, 3);
                        values.Insert(i - 1, ValueType.boolVal);
                        break;
                    } else {
                        values.RemoveRange(i - 1, 3);
                        values.Insert(i - 1, ValueType.mismatch);
                        Debug.LogWarning("ERROR!");
                        break;
                    }
                }
            }
        }
        while(values.Contains(ValueType.boolOp)){
            Console.WriteLine("checking for boolOps");
            for(int i = 0; i < values.Count; i++){
                if(values[i] == ValueType.boolOp){
                    ValueType a = values[i - 1];
                    ValueType b = values[i + 1];
                    if(a == ValueType.boolVal && b == ValueType.boolVal){
                        //correct
                        values.RemoveRange(i - 1, 3);
                        values.Insert(i - 1, ValueType.boolVal);
                        break;
                    } else {
                        values.RemoveRange(i - 1, 3);
                        values.Insert(i - 1, ValueType.mismatch);
                        Debug.LogWarning("ERROR!");
                        break;
                    }
                }
            }
        }
        return values;
    }

    private List<ValueType> EnumAdder(string[] expression){
        List<ValueType> valTypes = new List<ValueType>();
        foreach(string s in expression){
            if(Regex.IsMatch(s, "^[0-9]+$") || Compiler.Instance.intVars.ContainsKey(s)){
                valTypes.Add(ValueType.intVal);
            } else if(Regex.IsMatch(s, "^(true||false)$") || Compiler.Instance.boolVars.ContainsKey(s)){
                valTypes.Add(ValueType.boolVal);
            } else if(Regex.IsMatch(s, "^\"[^\"]+\"$") || Compiler.Instance.strVars.ContainsKey(s)){
                valTypes.Add(ValueType.strVal);
            } else if(s == "("){
                valTypes.Add(ValueType.startGroup);
            } else if(s == ")"){
                valTypes.Add(ValueType.endGroup);
            } else if(ReservedConstants.mathOperators.Contains(s)){
                valTypes.Add(ValueType.mathOp);
            } else if(ReservedConstants.comparisonOperators.Contains(s)){
                valTypes.Add(ValueType.cmprOp);
            } else if(ReservedConstants.booleanOperators.Contains(s)){
                valTypes.Add(ValueType.boolOp);
            } else if(s == ";"){
                valTypes.Add(ValueType.semicolon);
            } else if(s == ","){
                valTypes.Add(ValueType.comma);
            } else {
                valTypes.Add(ValueType.none);
            }
        }
        return valTypes;
    }

    private bool ArrayProcessor(List<string> array){
        string tempString = "";
        foreach(string s in array){
            tempString += " " + s;
        }
        string[] arrElems = tempString.Trim().Split(' ');

        //check for semicolons
        if(arrElems.Contains(";")){
            Debug.LogWarning("Semicolons cannot be inside arrays - replace it with a '}'!");
            return false;
        }

        if(!checkExpression(arrElems, 0)){
            return false;
        }

        //separate by comma
        arrElems = tempString.Trim().Split(',');
        for(int i = 0; i < arrElems.Length; i++){
            arrElems[i] = arrElems[i].Trim();
        }

        //check each value type
        //then check if the list contains anything that isn't the array's type
        List<ValueType> arrVals = new List<ValueType>();
        foreach(string s in arrElems){
            arrVals.Add(CheckTypes(s));
        }
        for(int i = 0; i < arrVals.Count; i++){
            if(Flags.Instance.arrType != arrVals[i]){
                addErr(string.Format("Line {0}: {1} is a(n) {2} - it cannot go in an array of {2}s!", lineIndex, ValueToString(arrVals[i]), ValueToString(Flags.Instance.arrType)));
            }
        }

        return true;
    }

    private bool CheckArrayIndex(string variable){
        //now check if its varName is in allVars
        //if it doesn't, array doesn't initialize
        string varName = variable.Substring(0, variable.IndexOf('`'));
        if(!allVars.ContainsKey(varName)){
            addErr(string.Format("Line {0}: array \"{1}\" does not exist!", lineIndex, varName));
            hasError = true;
            return false;
        }
        //now check if it exists
        //if it doesn't, index is out of bounds
        if(!allVars.ContainsKey(variable)){
            addErr(string.Format("Line {0}: array index is out of the array's bounds!", lineIndex));
            hasError = true;
            return false;
        }
        return true;
    }

    private void TestFunc(int wah, int nah){

    }

    //uses RegEx to check if variable name is valid
    //as always shouldn't start with a digit
    //or include anything other than alphanumeric and semicolon
    //not that the limited editor will allow special chars anyway
    private bool isValidVarName(string varName){
        if(ReservedConstants.keywords.Contains(varName)){
            addErr(string.Format("Line {0}: {1} is an invalid name - variable names cannot be reserved words!", lineIndex, varName));
            hasError = true;
            return false;
        }

        if(Regex.IsMatch(varName.ToCharArray()[0].ToString(), @"^[0-9]$")){
            addErr(string.Format("Line {0}: {1} is an invalid name - variable names cannot start with digits!", lineIndex, varName));
            hasError = true;
            return false;
        }

        foreach(char c in varName.ToCharArray()){
            if(!Regex.IsMatch(c.ToString(), @"^[a-zA-Z0-9_`]$")){
                addErr(string.Format("Line {0}: unexpected character {1} detected!", lineIndex, c));
                Debug.Log("Unexpected token: " + c);
                hasError = true;
                return false;
            }
        }
        
        return true;
    }

    private bool CheckDepth(string[] sections){
        int parenthesisDepth = 0;
        foreach(string s in sections){
            if(s == "("){
                parenthesisDepth++;
            }
            if(s == ")"){
                parenthesisDepth--;
            }
        }

        if(parenthesisDepth > 0){
            addErr(string.Format("Line {0}: line has an unclosed parenthesis pair!", lineIndex));
            return false;
        }
        if(parenthesisDepth < 0){
            addErr(string.Format("Line {0}: line has an unopened parenthesis pair!", lineIndex));
            return false;
        }
        return true;
    }
    
    private void addErr(string msg){
        string[] errorMsg = {msg};
        errorChecker.errorConvo.dialogueBlocks.Add(new Dialogue(errorMsg, "ERROR", 'R', true, errorChecker.errorSprite));
    }

    private string ValueToString(ValueType val){
        return val switch{
            ValueType.intVal => "int",
            ValueType.strVal => "string",
            ValueType.boolOp => "bool",
            _ => null,
        };
    }
}

public enum LineType{
    varInitialize,
    varAssign,
    functionCall,
    conditional,
    loop,
    openBrace,
    closeBrace,
    none
}

public enum ValueType{
    intVal,
    strVal,
    boolVal,
    mathOp,
    cmprOp,
    boolOp,
    startGroup,
    endGroup,
    comma,
    semicolon,
    mismatch,
    none
}