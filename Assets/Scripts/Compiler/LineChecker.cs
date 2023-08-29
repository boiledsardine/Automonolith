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
    int groupEndIndex = 0;
    ErrorChecker errorChecker;
    string[] globalSections;
    string strPattern = @"^[A-Za-z_]+$";
    string alphaNumPattern = @"^[A-Za-z_0-9]+$";
    bool isCondLoop = false;

    public LineChecker(string inputLine, int index, ErrorChecker err){
        allVars = Compiler.Instance.allVars;
        lineIndex = index;
        errorChecker = err;
        line = inputLine;
        globalSections = line.Split(' ');
    }

    public LineChecker(){}

    public void CheckLine(){
        readTokens(globalSections);
    }
    
    private void readTokens(string[] sections){
        isCondLoop = false;
        if(string.IsNullOrEmpty(line)){
            return;
        }

        //check a curly brace
        if(sections[0] == "{"){
            lineType = LineType.openBrace;
            //previous line should be a conditional, iterative, or array initializer
            if(!Flags.Instance.isExpectingBrace){
                addErr(string.Format("Line {0}: Open braces must come after a statement requiring them, like if-else statements, looping statements, or an array initialization!", lineIndex));
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
                Debug.LogAssertion(Flags.Instance.arrayElements.Count);
                if(Flags.Instance.isExpectingArrayCount && Flags.Instance.arrayElements.Count == Flags.Instance.expectedIndexCount){
                    addErr(string.Format("Line {0}: an array with {1} elements is expected!", Flags.Instance.indexOfArrayInitializer + 1, Flags.Instance.expectedIndexCount));
                    Flags.Instance.isExpectingArrayCount = false;
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
                            addErr(string.Format("Line {0}: 'if' should follow 'else' if you're trying to make an else-if statement!", lineIndex));
                            hasError = true;
                            return;
                        }
                    } else {
                        addErr(string.Format("Line {0}: 'if' should follow 'else' if you're trying to make an else-if statement!", lineIndex));
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
        }
        //brach is reached only if line is in array declaration
        //overrides everything else and has its own rules
        else if(Flags.Instance.isInArray){
            Flags.Instance.arrayElements.Add(line);
        }
        //checks for an array declaration
        else if(ReservedConstants.arrVarTypes.Contains(sections[0])){
            if(!checkInitialize(sections, true)){
                hasError = true;
                return;
            }

            //check if initialization is variable based
            string expression = Compiler.arrayToString(sections, 4);
            if(!string.IsNullOrEmpty(expression)){
                //check if initialization/assignment is based on 'new'
                //if it isn't, return
                if(!expression.Split(' ').Contains("new")){
                    return;
                }
            }

            //check if next token is a semicolon
            //if yes, immediately break out or else reach the next part
            if(sections[2] == ";"){
                return;
            }

            Flags.Instance.isArray = true;
            Flags.Instance.isExpectingBrace = true;

            if(sections[0] == "int[]"){
                Flags.Instance.arrType = ValueType.intVal;
            } else if(sections[0] == "string[]"){
                Flags.Instance.arrType = ValueType.strVal;
            } else if(sections[0] == "bool[]"){
                Flags.Instance.arrType = ValueType.boolVal;
            } else if(sections[0] == "char[]"){
                Flags.Instance.arrType = ValueType.charVal;
            } else if(sections[0] == "double[]"){
                Flags.Instance.arrType = ValueType.doubleVal;
            }
        }
        //checks for a break statement
        else if(sections[0] == "break"){
            lineType = LineType.loopBreak;
        }
        //checks for variable declaration
        //to add a new data type, add it in ReservedConstants.varTypes
        //follow a pattern: type varName = expression
        //or type varName ;
        else if(sections.Length > 1 && Regex.IsMatch(sections[0], alphaNumPattern) && Regex.IsMatch(sections[1], alphaNumPattern)){
            if(!checkInitialize(sections, false)){
                hasError = true;
                return;
            }
        }
        //checks for a call using "Bot."
        else if(sections[0] == "Bot"){
            //check if trying to use Bot as a type
            if(sections[1] != "." && isValidVarName(sections[1])){
                addErr(string.Format("Line {0}: cannot instantiate new object of class 'Bot', G4wain doesn't know object-oriented programming yet!", lineIndex));
                hasError = true;
                return;
            }

            lineType = LineType.functionCall;

            if(!checkBotCall(sections, 0)){
                hasError = true;
                return;
            }
        }
        //checks for variable assignment
        //!ReservedConstants.keywords.Contains(sections[0]) &&  isValidVarName(sections[0])
        else if(sections.Length > 1 && sections[1] == "="){
            //check if variable is an array index
            if(sections[0].Contains(ReservedConstants.arrayIndexSeparator)){
                if(!CheckArrayIndex(sections[0])){
                    hasError = true;
                    return;
                }
            }
            //check if the variable exists
            else if(!allVars.ContainsKey(sections[0])){
                Debug.LogAssertion("non extant var");
                addErr(string.Format("Line {0}: the name '{1}' does not exist or was improperly declared!", lineIndex, sections[0]));
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
        else if(sections[0] == ";"){
            /*if(!Flags.Instance.isExpectingSemicolon){
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

    private bool CheckDotSyntax(string[] sections, int dotIndex){
        //checks both sides of a dot
        string dotLeft = dotIndex > 0 ? sections[dotIndex - 1] : null;
        string dotRight = dotIndex < sections.Length - 1 ? sections[dotIndex + 1] : null;

        //check if dotLeft is a Bot type
        if(dotLeft == "Bot"){
            //check if dotRight exists
            if(dotRight == null || dotRight == ")" || dotRight == "(" || dotRight == ";" || ReservedConstants.allOperators.Contains(dotRight)){
                addErr(string.Format("Line {0}: identifier expected after the '.'!", lineIndex));
                return false;
            }
            //pass it to the function checker
            return true;
        }
        //check if dotLeft is a type
        else if(ReservedConstants.varTypes.Contains(dotLeft)){
            if(dotRight == null || dotRight == ")" || dotRight == "(" || dotRight == ";" || ReservedConstants.allOperators.Contains(dotRight)){
                addErr(string.Format("Line {0}: identifier expected after the '.'!", lineIndex));
                return false;
            }
            if((dotLeft == "int" || dotLeft == "int[]") && ReservedConstants.intFields.Contains(dotRight)){
                if(ReservedConstants.intFields.Contains(dotRight)){
                    addErr(string.Format("Line {0}: {1} has a definition for {2}, but for now, G4wain cannot use it.", lineIndex, "int", dotRight));
                } else {
                    addErr(string.Format("Line {0}: {1} does not have a definition for {2}!", lineIndex, "int", dotRight));
                }
            }

            if((dotLeft == "string" || dotLeft == "string[]") && ReservedConstants.stringFields.Contains(dotRight)){
                if(ReservedConstants.stringFields.Contains(dotRight)){
                    addErr(string.Format("Line {0}: {1} has a definition for {2}, but for now, G4wain cannot use it.", lineIndex, "string", dotRight));
                } else {
                    addErr(string.Format("Line {0}: {1} does not have a definition for {2}!", lineIndex, "string", dotRight));
                }
            }
            
            if((dotLeft == "bool" || dotLeft == "bool[]") && ReservedConstants.boolFields.Contains(dotRight)){
                if(ReservedConstants.boolFields.Contains(dotRight)){
                    addErr(string.Format("Line {0}: {1} has a definition for {2}, but for now, G4wain cannot use it.", lineIndex, "bool", dotRight));
                } else {
                    addErr(string.Format("Line {0}: {1} does not have a definition for {2}!", lineIndex, "bool", dotRight));
                }
            }
            //then throw an error anyway
            return false;
        }
        //check if dotLeft is an extant variable
        else if(isValidVarName(dotLeft)){
            if(dotRight == null || dotRight == ")" || dotRight == "(" || dotRight == ";" || ReservedConstants.allOperators.Contains(dotRight)){
                addErr(string.Format("Line {0}: identifier expected after the '.'!", lineIndex));
                return false;
            }
            if(Compiler.Instance.allVars.ContainsKey(dotLeft)){
                if(dotRight == null){
                    addErr(string.Format("Line {0}: identifier expected after the '.'!", lineIndex));
                    return false;
                }

                //check variable type
                var type = Compiler.Instance.allVars[dotLeft].type;
                if(type == VariableInfo.Type.intVarArr || type == VariableInfo.Type.strVarArr || type == VariableInfo.Type.boolVarArr){
                    //check for array properties
                    if(dotRight == "Length"){
                        //replace with stand-in
                        List<string> secList = sections.ToList();
                        secList.RemoveRange(dotIndex - 1, 3);
                        secList.Insert(dotIndex - 1, "0");
                        sections = secList.ToArray();
                        globalSections = sections;
                        return true;
                    } else if(ReservedConstants.arrayFields.Contains(dotRight)){
                        addErr(string.Format("Line {0}: {1} has a definition for {2}, but for now, G4wain cannot use it.", lineIndex, VarTypeToString(type), dotRight));
                        return false;
                    } else {
                        addErr(string.Format("Line {0}: {1} does not have a definition for '{2}'!", lineIndex, VarTypeToString(type), dotRight));
                        return false;
                    }
                } else {
                    //no properties for other vars yet
                    if(type == VariableInfo.Type.intVar){
                        if(ReservedConstants.intFields.Contains(dotRight)){
                            addErr(string.Format("Line {0}: {1} has a definition for {2}, but for now, G4wain cannot use it.", lineIndex, VarTypeToString(type), dotRight));
                        } else {
                            addErr(string.Format("Line {0}: {1} does not have a definition for {2}!", lineIndex, VarTypeToString(type), dotRight));
                        }
                    }

                    if(type == VariableInfo.Type.strVar && ReservedConstants.stringFields.Contains(dotRight)){
                       if(ReservedConstants.stringFields.Contains(dotRight)){
                            addErr(string.Format("Line {0}: {1} has a definition for {2}, but for now, G4wain cannot use it.", lineIndex, VarTypeToString(type), dotRight));
                        } else {
                            addErr(string.Format("Line {0}: {1} does not have a definition for {2}!", lineIndex, VarTypeToString(type), dotRight));
                        }
                    }
                    
                    if(type == VariableInfo.Type.boolVar && ReservedConstants.boolFields.Contains(dotRight)){
                        if(ReservedConstants.boolFields.Contains(dotRight)){
                            addErr(string.Format("Line {0}: {1} has a definition for {2}, but for now, G4wain cannot use it.", lineIndex, VarTypeToString(type), dotRight));
                        } else {
                            addErr(string.Format("Line {0}: {1} does not have a definition for {2}!", lineIndex, VarTypeToString(type), dotRight));
                        }
                    }
                    return false;
                }
            } else {
                addErr(string.Format("Line {0}: variable \'{1}\' does not exist or was improperly declared!", lineIndex, dotLeft));
            }
        }
        //go here if dotLeft isn't a type, Bot, or a variable
        else {
            addErr(string.Format("Line {0}: '.' is invalid in this location!", lineIndex));
            return false;
        }
        return true;

    }

    private string VarTypeToString(VariableInfo.Type type){
        return type switch{
            VariableInfo.Type.intVar => "int",
            VariableInfo.Type.strVar => "string",
            VariableInfo.Type.boolVar => "bool",
            VariableInfo.Type.intVarArr => "int[]",
            VariableInfo.Type.strVarArr => "string[]",
            VariableInfo.Type.boolVarArr => "bool[]",
            _ => null,
        };
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
    private bool checkInitialize(string[] sections, bool isArrayDeclaration){
        bool isCorrect = true;
        bool typeError = false;
        //check if it ain't a type in the scope of the game
        if(ReservedConstants.outOfScopeTypes.Contains(sections[0])){
            addErr(string.Format("Line {0}: {1} is a valid data type, but for now, G4wain cannot use them.", lineIndex, sections[0]));
            return false;
        }

        if(!ReservedConstants.varTypes.Contains(sections[0])){
            addErr(string.Format("Line {0}: {1} is not a valid data type!", lineIndex, sections[0]));
            isCorrect = false;
            typeError = true;
        }

        if(sections.Length == 1){
            addErr(string.Format("Line {0}: identifier expected!", lineIndex));
            isCorrect = false;
        }

        if(!isValidVarName(sections[1])){
            isCorrect = false;
        } else if(allVars.ContainsKey(sections[1])){
            addErr(string.Format("Line {0}: {1} is already defined!", lineIndex, sections[1]));
            isCorrect = false;
        }

        if(sections.Length == 2){
            addErr(string.Format("Line {0}: ';' or '=' expected!", lineIndex));
            isCorrect = false;
        }

        //initializes AND assigns
        if(!typeError && sections[2] == "="){
            //check if new
            if(sections.Length >= 4 && sections[3] == "new"){
                if(!CheckNew(sections, 5)){
                    return false;
                }
                sections = globalSections;
            }
            if(!checkExpression(sections, 3, !isArrayDeclaration)){
                lineType = LineType.varInitialize;
                return false;
            } else {
                sections = globalSections;
                string expression = Compiler.arrayToString(sections, 3);
                switch(sections[0]){
                    case "int":
                        if(CheckTypes(expression) != ValueType.intVal){
                            addErr(string.Format("Line {0}: provided value is not an int!", lineIndex));
                            //force creation of empty variable
                            Compiler.Instance.allVars.Add(sections[1], new VariableInfo(VariableInfo.Type.intVar, true));
                            Compiler.Instance.intVars.Add(sections[1], 0);
                            return false;
                        }
                        break;
                    case "string":
                        if(CheckTypes(expression) != ValueType.strVal){
                            addErr(string.Format("Line {0}: provided value is not a string!", lineIndex));
                            Compiler.Instance.allVars.Add(sections[1], new VariableInfo(VariableInfo.Type.strVar, true));
                            Compiler.Instance.strVars.Add(sections[1], "");
                            return false;
                        }
                    break;
                    case "bool":
                        if(CheckTypes(expression) != ValueType.boolVal){
                            addErr(string.Format("Line {0}: provided value is not a bool!", lineIndex));
                            Compiler.Instance.allVars.Add(sections[1], new VariableInfo(VariableInfo.Type.boolVar, true));
                            Compiler.Instance.boolVars.Add(sections[1], false);
                            return false;
                        }
                    break;
                    case "char":
                        if(CheckTypes(expression) != ValueType.charVal){
                            addErr(string.Format("Line {0}: provided value is not a char!", lineIndex));
                            Compiler.Instance.allVars.Add(sections[1], new VariableInfo(VariableInfo.Type.boolVar, true));
                            Compiler.Instance.charVars.Add(sections[1], 'a');
                            return false;
                        }
                    break;
                    case "double":
                        if(CheckTypes(expression) != ValueType.doubleVal){
                            addErr(string.Format("Line {0}: provided value is not a double!", lineIndex));
                            Compiler.Instance.allVars.Add(sections[1], new VariableInfo(VariableInfo.Type.boolVar, true));
                            Compiler.Instance.doubleVars.Add(sections[1], 0.0);
                            return false;
                        }
                    break;
                    case "int[]":
                        if(!string.IsNullOrEmpty(expression) && CheckTypes(expression) != ValueType.intArr){
                            addErr(string.Format("Line {0}: provided value is not an int array!", lineIndex));
                            Compiler.Instance.allVars.Add(sections[1], new VariableInfo(VariableInfo.Type.intVarArr, true));
                            Compiler.Instance.intArrs.Add(sections[1], new int[0]);
                            return false;
                        }
                    break;
                    case "string[]":
                        if(!string.IsNullOrEmpty(expression) && CheckTypes(expression) != ValueType.strArr){
                            addErr(string.Format("Line {0}: provided value is not a string array!", lineIndex));
                            Compiler.Instance.allVars.Add(sections[1], new VariableInfo(VariableInfo.Type.strVarArr, true));
                            Compiler.Instance.strArrs.Add(sections[1], new string[0]);
                            return false;
                        }
                    break;
                    case "bool[]":
                        if(!string.IsNullOrEmpty(expression) && CheckTypes(expression) != ValueType.boolArr){
                            addErr(string.Format("Line {0}: provided value is not a bool array!", lineIndex));
                            Compiler.Instance.allVars.Add(sections[1], new VariableInfo(VariableInfo.Type.boolVarArr, true));
                            Compiler.Instance.boolArrs.Add(sections[1], new bool[0]);
                            return false;
                        }
                    break;
                    case "char[]":
                        if(!string.IsNullOrEmpty(expression) && CheckTypes(expression) != ValueType.charArr){
                            addErr(string.Format("Line {0}: provided value is not a char array!", lineIndex));
                            Compiler.Instance.allVars.Add(sections[1], new VariableInfo(VariableInfo.Type.charVarArr, true));
                            Compiler.Instance.charArrs.Add(sections[1], new char[0]);
                            return false;
                        }
                    break;
                    case "double[]":
                        if(!string.IsNullOrEmpty(expression) && CheckTypes(expression) != ValueType.doubleArr){
                            addErr(string.Format("Line {0}: provided value is not a double array!", lineIndex));
                            Compiler.Instance.allVars.Add(sections[1], new VariableInfo(VariableInfo.Type.doubleVarArr, true));
                            Compiler.Instance.doubleArrs.Add(sections[1], new double[0]);
                            return false;
                        }
                    break;
                }
                lineType = LineType.varAssign;
            }
        } else if(!typeError) {
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
        return isCorrect;
    }

    private bool checkAssignment(string[] sections, string varName){
        if(allVars.ContainsKey(varName)){
            if(sections[1] == "="){
                if(sections.Length >= 3 && sections[2] == "new"){
                    if(!CheckNew(sections, 4)){
                        return false;
                    }
                    sections = globalSections;
                }

                var type = allVars[varName].type;
                bool isArrayVar = type == VariableInfo.Type.intVarArr || type == VariableInfo.Type.strVarArr || type == VariableInfo.Type.boolVarArr;
                if(!checkExpression(sections, 2, !isArrayVar)){
                    return false;
                }
                sections = globalSections;
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
                } else if(Compiler.Instance.intArrs.ContainsKey(sections[0])){
                    if(!string.IsNullOrEmpty(expression) && CheckTypes(expression) != ValueType.intArr){
                        addErr(string.Format("Line {0}: provided value is not an int array!", lineIndex));
                        return false;
                    } else if(string.IsNullOrEmpty(expression)){
                        //check next line if array initializer
                        if(Compiler.Instance.getCodeLines[lineIndex] == "{"){
                            addErr(string.Format("Line {0}: in assigning previously-declared int arrays, make sure to add \"new int[]\" before the initializer!", lineIndex));
                            return false;
                        }
                    } else if(expression == ";"){
                        addErr(string.Format("Line {0}: expression cannot be empty!", lineIndex));
                        return false;
                    }
                } else if(Compiler.Instance.strArrs.ContainsKey(sections[0])){
                    if(!string.IsNullOrEmpty(expression) && CheckTypes(expression) != ValueType.strArr){
                        addErr(string.Format("Line {0}: provided value is not a string array!", lineIndex));
                        return false;
                    } else if(string.IsNullOrEmpty(expression)){
                        //check next line if array initializer
                        if(Compiler.Instance.getCodeLines[lineIndex] == "{"){
                            addErr(string.Format("Line {0}: in assigning previously-declared string arrays, make sure to add \"new string[]\" before the initializer!", lineIndex));
                            return false;
                        }
                    } else if(expression == ";"){
                        addErr(string.Format("Line {0}: expression cannot be empty!", lineIndex));
                        return false;
                    }
                } else if(Compiler.Instance.boolArrs.ContainsKey(sections[0])){
                    if(!string.IsNullOrEmpty(expression) && CheckTypes(expression) != ValueType.boolArr){
                        addErr(string.Format("Line {0}: provided value is not a bool array!", lineIndex));
                        return false;
                    } else if(string.IsNullOrEmpty(expression)){
                        //check next line if array initializer
                        if(Compiler.Instance.getCodeLines[lineIndex] == "{"){
                            addErr(string.Format("Line {0}: in assigning previously-declared bool arrays, make sure to add \"new bool[]\" before the initializer!", lineIndex));
                            return false;
                        }
                    } else if(expression == ";"){
                        addErr(string.Format("Line {0}: expression cannot be empty!", lineIndex));
                        return false;
                    }
                }
                
            } else if(sections[1] == ";"){
                addErr(string.Format("Line {0}: only assignment, call, increment, or decrement can be used as statements!", lineIndex));
                return false;
            } else {
                Debug.LogAssertion("unexpected token");
                addErr(string.Format("Line {0}: unexpected token {1}!", lineIndex, sections[1]));
                return false;
            }
        } else {
            Debug.LogAssertion("non extant error");
            addErr(string.Format("Line {0}: the name {1} does not exist or was improperly declared!", lineIndex, varName));
            return false;
        }
        return true;
    }

    //checks for botty call
    private bool checkBotCall(string[] sections, int botIndex){
        bool isCorrect = true;
        if(sections[botIndex + 1] == "."){
            //checks if function name exists in list of valid functions
            if(!FunctionHandler.builtInFunctions.Contains(sections[botIndex + 2])){
                addErr(string.Format("Line {0}: Bot has no definition for {1}!", lineIndex, sections[botIndex + 2]));
                isCorrect = false;
            }
            if(sections[botIndex + 3] != "("){
                //name is valid but no parentheses
                addErr(string.Format("Line {0}: {1} is a method - add \"()\" after it!", lineIndex, sections[botIndex + 2]));
                isCorrect = false;
            }

            //checks parenthesis depth
            if(!CheckDepth(sections)){
                isCorrect = false;
            }
            
            //check expression validity
            if(!checkExpression(sections, botIndex + 3, false)){
                return false;
            }
            sections = globalSections;

            //find index of last parenthesis
            groupEndIndex = FindGroupEndIndex(sections, botIndex);
            List<string> funcString = new List<string>();
            for(int i = botIndex; i <= groupEndIndex; i++){
                funcString.Add(sections[i]);
            }

            //check function arguments
            if(CheckFunctionArgs(Compiler.arrayToString(funcString.ToArray(), 0))){
                isCorrect = false;
            }

        } else {
            addErr(string.Format("Line {0}: unexpected token {1}, was expecting a \".\"!", lineIndex, sections[botIndex + 1]));
            return false;
        }
        return isCorrect;
    }

    private bool CheckNew(string[] sections, int statementMinLength){
        string afterNew = "";
        if(sections.Length < statementMinLength){
            addErr(string.Format("Line {0}: enter a data type after new and add () or [] after it!", lineIndex));
            return false;
        }
        if(sections.Length >= statementMinLength){
            afterNew = sections[statementMinLength - 1];
            if(afterNew.Contains("[") && afterNew.Contains("]")){
                afterNew = Compiler.GetSubstring(afterNew, 0, afterNew.IndexOf("[")) + "[]";
            }
            if(!ReservedConstants.varTypes.Contains(afterNew)){
                addErr(string.Format("Line {0}: enter a data type after new and add () or [] after it!", lineIndex));
                return false;
            }
        }
        if(sections.Length >= statementMinLength + 1 && sections[statementMinLength] == "("){
            addErr(string.Format("Line {0}: constructors are used in Object-Oriented Programming (OOP), something G4wain cannot fully do!", lineIndex));
            return false;
        }
        int x = statementMinLength - 1;
        if(sections.Length >= statementMinLength && ReservedConstants.arrVarTypes.Contains(afterNew)){
            string indexString = Compiler.GetSubstring(sections[x], sections[x].IndexOf("[") + 1, sections[x].IndexOf("]"));
            if(string.IsNullOrEmpty(indexString)){
                //check for an initializer
                if(Compiler.Instance.getCodeLines[lineIndex] != "{"){
                    //no initializer, error
                    addErr(string.Format("Line {0}: array creation must have an array size between the [] or an initializer with values between {{}}!", lineIndex));
                    return false;
                }

                //set mode to array initialization
                Flags.Instance.isArray = true;
                Flags.Instance.isExpectingBrace = true;

                string s = "";
                if(sections[x] == "int[]"){
                    Flags.Instance.arrType = ValueType.intVal;
                    s = "intArr";
                } else if(sections[x] == "string[]"){
                    Flags.Instance.arrType = ValueType.strVal;
                    s = "stringArr";
                } else if(sections[x] == "bool[]"){
                    Flags.Instance.arrType = ValueType.boolVal;
                    s = "boolArr";
                } else if(sections[x] == "char[]"){
                    Flags.Instance.arrType = ValueType.charVal;
                    s = "charArr";
                } else if(sections[x] == "double[]"){
                    Flags.Instance.arrType = ValueType.doubleVal;
                    s = "doubleArr";
                }

                //replace
                List<string> secList = sections.ToList();
                secList.RemoveRange(x - 1, 2);
                secList.Insert(x - 1, s);
                globalSections = secList.ToArray();
                sections = globalSections;
            } else {
                string formattedLine = CodeFormatter.Format(indexString);
                int index = new IntExpression(formattedLine).evaluate();
                //check for an initializer
                if(Compiler.Instance.getCodeLines[lineIndex] == "{"){
                    //set mode to array initialization and count the elements in the initalizer
                    //set mode to array initialization
                    Flags.Instance.isArray = true;
                    Flags.Instance.isExpectingBrace = true;

                    string s = "";
                    if(sections[x] == "int[]"){
                        Flags.Instance.arrType = ValueType.intVal;
                        s = "intArr";
                    } else if(sections[x] == "string[]"){
                        Flags.Instance.arrType = ValueType.strVal;
                        s = "stringArr";
                    } else if(sections[x] == "bool[]"){
                        Flags.Instance.arrType = ValueType.boolVal;
                        s = "boolArr";
                    } else if(sections[x] == "char[]"){
                        Flags.Instance.arrType = ValueType.charVal;
                        s = "charArr";
                    } else if(sections[x] == "double[]"){
                        Flags.Instance.arrType = ValueType.doubleVal;
                        s = "doubleArr";
                    }

                    List<string> secList = sections.ToList();
                    secList.RemoveRange(x - 1, 2);
                    secList.Insert(x - 1, s);
                    globalSections = secList.ToArray();
                    sections = globalSections;
                    
                    //count the array elements later and compare with indexCount
                    Flags.Instance.isExpectingArrayCount = true;
                    Flags.Instance.indexOfArrayInitializer = lineIndex;
                    Flags.Instance.expectedIndexCount = index;
                } else {
                    string s = "";
                    if(sections[x].Contains("int")){
                        s = "intArr";
                    } else if(sections[x].Contains("string")){
                        s = "stringArr";
                    } else if(sections[x].Contains("bool")){
                        s = "boolArr";
                    }

                    List<string> secList = sections.ToList();
                    secList.RemoveRange(x - 1, 2);
                    secList.Insert(x - 1, s);
                    globalSections = secList.ToArray();
                }
                
            }
        }
        return true;
    }

    private bool CheckFunctionArgs(string line){
        Compiler.Instance.functionHandler.initializeHandler(line, lineIndex);
        return Compiler.Instance.functionHandler.hasError;
    }

    private int FindGroupEndIndex(string[] sections, int botIndex){
        int groupEndIndex = 0;
        int depth = 0;
        for(int i = botIndex; i < sections.Length; i++){
            if(sections[i] == "("){
                depth++;
            } else if(sections[i] == ")" && depth > 1){
                depth--;
            } else if(sections[i] == ")" && depth == 1){
                depth--;
                groupEndIndex = i;
                break;
            } else if(sections[i] == ")" && depth < 1){
                //error
                break;
            }
        }
        return groupEndIndex;
    }

    private bool CheckLoopOrCondition(string[] sections, int lineIndex, string type){
        //check the next line for a "{"
        string nextLine = Compiler.Instance.getCodeLines.Length > 1 ? Compiler.Instance.getCodeLines[lineIndex] : null;
        if(nextLine != "{"){
            addErr(string.Format("Line {0}: was expecting a '{{' after the condition!", lineIndex));
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
            addErr(string.Format("Line {0}: was expecting a '('!", lineIndex));
            return false;
        }
        if(sections[groupIndex] != "("){
            addErr(string.Format("Line {0}: was expecting a '('", lineIndex));
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
        if(!checkExpression(sections, expIndex, true)){
            //error handled by checkExpression
            return false;
        }

        string expression = Compiler.arrayToString(globalSections, groupIndex + 1);
        expression = expression.Substring(0, expression.LastIndexOf(")")).Trim();
        if(CheckTypes(expression) != ValueType.boolVal){
            addErr(string.Format("Line {0}: conditional statements must be a boolean!", lineIndex));
            return false;
        }
        return true;
    }

    //checks numerical expression validity
    private bool checkExpression(string[] sections, int index, bool notEmpty){
        bool isCorrect = true;
        bool isExpectingValue = true;
        //everything else should be part of the expression
        string expr = Compiler.arrayToString(sections, index).Trim();
        if(notEmpty && string.IsNullOrEmpty(expr) || expr == ";"){
            addErr(string.Format("Line {0}: the expression here cannot be empty!", lineIndex));
            return false;
        }
        
        for(int i = index; i < sections.Length; i++){
            //error: math operator right after an "=" sign
            
            if(ReservedConstants.mathOperators.Contains(sections[i])){
                if(sections[i - 1] == "=" && !(sections[i] == "+" || sections[i] == "-")){
                    addErr(string.Format("Line {0}: unexpected token {1}!", lineIndex, sections[i]));
                    isCorrect = false;
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
            if(isExpectingValue && Regex.IsMatch(sections[i], @"^[a-zA-Z0-9_`.\']+$")){
                string[] words = {"true", "false", "intArr", "stringArr", "boolArr"};
                if(words.Contains(sections[i])){
                    continue;
                }

                //check for an internal function call
                else if(sections[i] == "Bot"){
                    if(!checkBotCall(sections, i)){
                        return false;
                    }
                    string insert = "";
                    //check the function's return type
                    if(FunctionHandler.nonVoids.Contains(sections[i + 2])){
                        if(sections[i + 2] == "read"){
                            insert = "\"string\"";
                        } else if(sections[i + 2] == "readInt"){
                            insert = "0";
                        } else if(sections[i + 2] == "readBool" || sections[i + 2] == "look" || sections[i + 2] == "CheckCube"){
                            insert = "false";
                        } else if(sections[i + 2] == "ReadIntArr"){
                            insert = "intArr";
                        } else if(sections[i + 2] == "ReadStringArr"){
                            insert = "stringArr";
                        } else if(sections[i + 2] == "ReadBoolArr"){
                            insert = "boolArr";
                        }
                    } else {
                        insert = "void";
                    }
                    //replace the whole function call with a stand-in
                    List<string> secList = sections.ToList();
                    secList.RemoveRange(i, groupEndIndex - i + 1);
                    secList.Insert(i, insert);
                    sections = secList.ToArray();
                    globalSections = sections;
                    Debug.LogWarning(Compiler.arrayToString(secList.ToArray(), 0));
                }

                //check for dot syntax
                else if(sections[i] == "."){
                    if(!CheckDotSyntax(sections, i)){
                        return false;
                    }
                    sections = globalSections;
                    Debug.LogWarning(Compiler.arrayToString(sections.ToArray(), 0));
                    continue;
                }
                else if(ReservedConstants.varTypes.Contains(sections[i])){
                    if(i < sections.Length - 1 && sections[i + 1] == "."){
                        continue;
                    }
                }

                //check char format
                else if(Regex.IsMatch(sections[i], @"^\'.*\'$")){
                    if(sections[i].Length == 2){
                        addErr(string.Format("Line {0}: char literals cannot be empty!", lineIndex));
                        isCorrect = false;
                    }
                    if(sections[i].Length > 3){
                        addErr(string.Format("Line {0}: char literals can only have one character inside them!", lineIndex));
                        isCorrect = false;
                    }
                }

                //check variable name validity
                else if(isValidVarName(sections[i])){
                    //check if it's an array index
                    if(sections[i].Contains('`')){
                        if(!CheckArrayIndex(sections[i])){
                            isCorrect = false;
                        }
                    }
                    if(allVars.ContainsKey(sections[i])){
                        //check if variable is assigned
                        if(!allVars[sections[i]].isSet){
                            addErr(string.Format("Line {0}: cannot use variable {1} - it is unassigned!", lineIndex, sections[i]));
                            isCorrect = false;
                        }
                        //isExpectingValue = false;
                        //check if it's followed by a dot
                        if(i < sections.Length - 1 && sections[i + 1] == "."){
                            continue;
                        }
                    } else {
                        addErr(string.Format("Line {0}: the name {1} does not exist or was improperly declared!", lineIndex, sections[i]));
                        return false;
                    }
                } else {
                    Debug.LogAssertion("unexpected token");
                    addErr(string.Format("Line {0}: unexpected token {1}!", lineIndex, sections[i]));
                    return false;
                }
            }
        }
        //check sequence of tokens
        if(!CheckSequence(sections, index, true)){
            return false;
        }

        return isCorrect;
    }

    private bool CheckSequence(string[] sections, int index, bool zeroArgs){
        string expressionString = Compiler.arrayToString(sections, index);
        string[] expSections = expressionString.Split(' ');
        List<ValueType> values = EnumAdder(expSections);
        bool expectValue = true;
        bool expectOperator = false;
        bool expectComma = false;
        bool expectStartGroup = true;
        bool expectEndGroup = false;
        bool expectNegate = true;
        
        int depth = 0;
        for(int i = 0; i < expSections.Length; i++){
            //int wah = 9 10;
            ValueType val = values[i];
            if(val == ValueType.intVal || val == ValueType.strVal || val == ValueType.boolVal || val == ValueType.voidVal
            || val == ValueType.intArr || val == ValueType.strArr || val == ValueType.boolArr || val == ValueType.charVal
            || val == ValueType.charArr || val == ValueType.doubleVal || val == ValueType.doubleArr || val == ValueType.nonExistVar){
                if(!expectValue){
                    Debug.LogAssertion("unexpected value - sequence");
                    addErr(string.Format("Line {0}: unexpected token {1}!", lineIndex, expSections[i]));
                    return false;
                }

                expectValue = false;
                expectOperator = true;
                expectComma = true;
                expectStartGroup = false;
                expectEndGroup = true;
                expectNegate = false;
            } else if(val == ValueType.negateOp){
                if(!expectNegate){
                    Debug.LogAssertion("unexpected value - negation");
                    addErr(string.Format("Line {0}: unexpected token {1}!", lineIndex, expSections[i]));
                }

                expectValue = true;
                expectOperator = false;
                expectComma = false;
                expectStartGroup = true;
                expectEndGroup = false;
                expectNegate = false;
            } else if(val == ValueType.mathOp || val == ValueType.cmprOp || val == ValueType.boolOp){
                if(!expectOperator){
                    //check if operator in question is a sign
                    if(expSections[i] == "+" || expSections[i] == "-"){
                        //do nothing
                    } else {
                        Debug.LogAssertion("unexpected operator");
                        addErr(string.Format("Line {0}: unexpected token {1}!", lineIndex, expSections[i]));
                        return false;
                    }
                }

                expectValue = true;
                expectOperator = false;
                expectComma = false;
                expectStartGroup = true;
                expectEndGroup = false;
                expectNegate = true;
            } else if(val == ValueType.startGroup){
                if(!expectStartGroup){
                    Debug.LogAssertion("unexpected startgroup");
                    addErr(string.Format("Line {0}: unexpected token {1}!", lineIndex, expSections[i]));
                    return false;
                }

                expectValue = true;
                expectOperator = false;
                expectComma = false;
                expectStartGroup = true;
                expectEndGroup = zeroArgs;
                expectNegate = true;

                zeroArgs = !zeroArgs;

                depth++;
            } else if(val == ValueType.endGroup){
                if(!expectEndGroup){
                    Debug.LogAssertion("unexpected endgroup");
                    addErr(string.Format("Line {0}: unexpected ')'!", lineIndex));
                    return false;
                }

                expectValue = false;
                expectOperator = true;
                expectComma = true;
                expectStartGroup = false;
                expectEndGroup = true;
                expectNegate = false;

                depth--;
            } else if(val == ValueType.comma){
                if(!expectComma){
                    Debug.LogAssertion("unexpected comma!");
                    addErr(string.Format("Line {0}: unexpected comma; was expecting a value, '!', or '('!", lineIndex));
                    return false;
                }
                if(depth > 0 && isCondLoop){
                    //needs nuance - function args or conditional?
                    Debug.LogAssertion("Commas cannot be between () in an argument!");
                    addErr(string.Format("Line {0}: commas cannot be in an expression!", lineIndex));
                }

                expectValue = true;
                expectOperator = false;
                expectComma = false;
                expectStartGroup = true;
                expectEndGroup = false;
                expectNegate = true;
            } else if(val != ValueType.semicolon) {
                //unexpected
                Debug.LogAssertion("semicolon error!");
                addErr(string.Format("Line {0}: unexpected token {1}!", lineIndex, expSections[i]));
                return false;
            }
        }
        return true;
    }

    string[] typeCheckTokens;
    public ValueType CheckTypes(string expression){
        string[] expTokens = expression.Split(' ');
        List<ValueType> val = EvaluateGroups(EnumAdder(expTokens), expTokens);
        val = EvaluateList(val, typeCheckTokens.ToList());
        return val[0];
    }

    private List<ValueType> EvaluateGroups(List<ValueType> values, string[] expTokens){
        //check if values has only one element or no groups
        if(values.Count == 1 || !values.Contains(ValueType.startGroup)){
            typeCheckTokens = expTokens;
            return values;
        }

        //recursively isolate/evaluate parentheses groups
        //find start and end of group
        //gets the group at the leftmost side
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
        List<string> tokenSubList = new List<string>();
        for(int i = startIndex + 1; i < endIndex; i++){
            valueSubList.Add(values[i]);
            tokenSubList.Add(expTokens[i]);
        }
        
        //iterate through sublist to look for operators
        valueSubList = EvaluateList(valueSubList, tokenSubList);

        //replace in value list
        List<string> tokens = expTokens.ToList();
        values.RemoveRange(startIndex, endIndex - startIndex + 1);
        tokens.RemoveRange(startIndex, endIndex - startIndex + 1);
        values.Insert(startIndex, valueSubList[0]);
        tokens.Insert(startIndex, "tkn");

        //recur
        if(values.Contains(ValueType.startGroup)){
            EvaluateGroups(values, tokens.ToArray());
        }

        typeCheckTokens = tokens.ToArray();

        return values;
    }

    //TODO: errors for this one
    private List<ValueType> EvaluateList(List<ValueType> values, List<string> expTokens){
        Debug.Log(Compiler.arrayToString(expTokens.ToArray(), 0));
        while(values.Contains(ValueType.mathOp)){
            for(int i = values.Count - 1; i >= 0; i--){
                if(values[i] == ValueType.mathOp){
                    ValueType a = i > 0 ? values[i - 1] : ValueType.blank;
                    ValueType b = i < values.Count ? values[i + 1] : ValueType.blank;
                    //check for signs first
                    if(expTokens[i] == "+" || expTokens[i] == "-"){
                        bool isOperator = a == ValueType.mathOp || a == ValueType.boolOp || a == ValueType.cmprOp;
                        if((a == ValueType.blank || isOperator) && b == ValueType.intVal){
                            //correct
                            values.RemoveAt(i);
                            break;
                        }
                    }

                    if(a == ValueType.intVal && b == ValueType.intVal){
                        //correct
                        values.RemoveRange(i - 1, 3);
                        values.Insert(i - 1, ValueType.intVal);
                        expTokens.RemoveRange(i - 1, 3);
                        expTokens.Insert(i - 1, "0");
                        break;
                    } else if(a == ValueType.strVal && b == ValueType.strVal){
                        //depends, is it a "+"?
                        if(expTokens[i] != "+"){
                            Debug.LogAssertion("type mismatch for math operator");
                            addErr(string.Format("Line {0}: can't use operator {1} for types {2} and {3}! {1} operates on numerical types only!",
                                lineIndex, expTokens[i], ValueToString(a), ValueToString(b)));
                            values.RemoveRange(i - 1, 3);
                            values.Insert(i - 1, ValueType.mismatch);
                            expTokens.RemoveRange(i - 1, 3);
                            expTokens.Insert(i - 1, "???");
                            hasError = true;
                            return values;
                        }
                        values.RemoveRange(i - 1, 3);
                        values.Insert(i - 1, ValueType.strVal);
                        expTokens.RemoveRange(i - 1, 3);
                        expTokens.Insert(i - 1, "string");
                        break;
                    } else {
                        //type mismatch error
                        addErr(string.Format("Line {0}: can't use operator {1} for types {2} and {3}! {1} operates on numerical types only!",
                                lineIndex, expTokens[i], ValueToString(a), ValueToString(b)));
                        hasError = true;
                        values.RemoveRange(i - 1, 3);
                        values.Insert(i - 1, ValueType.mismatch);
                        expTokens.RemoveRange(i - 1, 3);
                        expTokens.Insert(i - 1, "???");
                        return values;
                    }
                }
            }
        }
        while(values.Contains(ValueType.negateOp)){
            for(int i = 0; i < values.Count; i++){
                if(values[i] == ValueType.negateOp){
                    ValueType a = values[i + 1];
                    if(a == ValueType.boolVal){
                        //correct
                        values.RemoveAt(i);
                        break;
                    } else {
                        addErr(string.Format("Line {0}: can't use operator {1} for type {2}! {1} only works on bools!",
                                lineIndex, expTokens[i], ValueToString(a)));
                        Debug.LogAssertion("type mismatch for negation operator");
                        values.RemoveRange(i, 2);
                        values.Insert(i, ValueType.mismatch);
                        expTokens.RemoveRange(i, 2);
                        expTokens.Insert(i, "???");
                        //type mismatch error
                        hasError = true;
                        return values;
                    }
                }
            }
        }
        while(values.Contains(ValueType.cmprOp)){
            for(int i = 0; i < values.Count; i++){
                if(values[i] == ValueType.cmprOp){
                    ValueType a = values[i - 1];
                    ValueType b = values[i + 1];
                    if(a == ValueType.intVal && b == ValueType.intVal){
                        //correct
                        values.RemoveRange(i - 1, 3);
                        values.Insert(i - 1, ValueType.boolVal);
                        expTokens.RemoveRange(i - 1, 3);
                        expTokens.Insert(i - 1, "false");
                        break;
                    } else {
                        if(a == ValueType.strVal && b == ValueType.strVal){
                            //correct
                            values.RemoveRange(i - 1, 3);
                            values.Insert(i - 1, ValueType.boolVal);
                            expTokens.RemoveRange(i - 1, 3);
                            expTokens.Insert(i - 1, "false");
                        }
                        addErr(string.Format("Line {0}: can't use operator {1} for types {2} and {3}! {1} compares the same data types!",
                                lineIndex, expTokens[i], ValueToString(a), ValueToString(b)));
                        values.RemoveRange(i - 1, 3);
                        values.Insert(i - 1, ValueType.mismatch);
                        expTokens.RemoveRange(i - 1, 3);
                        expTokens.Insert(i - 1, "???");
                        //type mismatch error
                        Debug.LogAssertion("type mismatch for comparison operator");
                        hasError = true;
                        return values;
                    }
                }
            }
        }
        while(values.Contains(ValueType.boolOp)){
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
                        addErr(string.Format("Line {0}: can't use operator {1} for types {2} and {3}! {1} is for booleans only!",
                                lineIndex, expTokens[i], ValueToString(a), ValueToString(b)));
                        values.RemoveRange(i - 1, 3);
                        values.Insert(i - 1, ValueType.mismatch);
                        expTokens.RemoveRange(i - 1, 3);
                        expTokens.Insert(i - 1, "???");
                        //type mismatch error
                        Debug.LogAssertion("type mismatch for boolean operator");
                        hasError = true;
                        return values;
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
            } else if(Regex.IsMatch(s, "^\"[^\"]*\"$") || Compiler.Instance.strVars.ContainsKey(s)){
                valTypes.Add(ValueType.strVal);
            } else if(Regex.IsMatch(s, "^\'.+\'$") || Compiler.Instance.charVars.ContainsKey(s)){
                valTypes.Add(ValueType.charVal);
            } else if(Regex.IsMatch(s, "^[0-9]+.?[0-9]*$") || Compiler.Instance.doubleVars.ContainsKey(s)){
                valTypes.Add(ValueType.doubleVal);
            } else if(s == "intArr" || Compiler.Instance.intArrs.ContainsKey(s)){
                valTypes.Add(ValueType.intArr);
            } else if(s == "stringArr" || Compiler.Instance.strArrs.ContainsKey(s)){
                valTypes.Add(ValueType.strArr);
            } else if(s == "boolArr" || Compiler.Instance.boolArrs.ContainsKey(s)){
                valTypes.Add(ValueType.boolArr);
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
            } else if(s == "void"){
                valTypes.Add(ValueType.voidVal);
            } else if(s == "!"){
                valTypes.Add(ValueType.negateOp);
            } else if(isValidVarName(s) && !Compiler.Instance.allVars.ContainsKey(s)){
                valTypes.Add(ValueType.nonExistVar);
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
            addErr(string.Format("Line {0}: Semicolons cannot be inside arrays!", lineIndex));
            return false;
        }

        if(!checkExpression(arrElems, 0, true)){
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
        bool hasValError = false;
        for(int i = 0; i < arrVals.Count; i++){
            if(Flags.Instance.arrType != arrVals[i]){
                addErr(string.Format("Line {0}: {1} is a(n) {2} - it cannot go in an array of {3}s!",
                    lineIndex, arrElems[i], ValueToString(arrVals[i]), ValueToString(Flags.Instance.arrType)));
                hasValError = true;
            }
        }

        if(hasValError){
            return false;
        }

        return true;
    }

    private bool CheckArrayIndex(string variable){
        //now check if its varName is in allVars
        //if it doesn't, array wasn't declared
        string varName = variable.Substring(0, variable.IndexOf(ReservedConstants.arrayIndexSeparator));
        if(!allVars.ContainsKey(varName)){
            addErr(string.Format("Line {0}: array \"{1}\" does not exist or was improperly declared!", lineIndex, varName));
            hasError = true;
            return false;
        }

        //now check if array is assigned
        if(!allVars[varName].isSet){
            addErr(string.Format("Line {0}: array \"{1}\" hasn't been assigned yet!", lineIndex, varName));
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
            Debug.LogAssertion("invalid name error - starts with a digit");
            addErr(string.Format("Line {0}: {1} is an invalid name - variable names cannot start with digits!", lineIndex, varName));
            hasError = true;
            return false;
        }

        foreach(char c in varName.ToCharArray()){
            if(!Regex.IsMatch(c.ToString(), @"^[a-zA-Z0-9_`]$")){
                Debug.LogAssertion("unexpected character error");
                addErr(string.Format("Line {0}: unexpected character {1} detected!", lineIndex, c));
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
            ValueType.boolVal => "bool",
            ValueType.charVal => "char",
            ValueType.doubleVal => "double",
            ValueType.intArr => "int[]",
            ValueType.strArr => "string[]",
            ValueType.boolArr => "bool[]",
            ValueType.charArr => "char[]",
            ValueType.doubleArr => "double[]",
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
    loopBreak,
    openBrace,
    closeBrace,
    none
}

public enum ValueType{
    intVal,
    intArr,
    strVal,
    strArr,
    boolVal,
    boolArr,
    charVal,
    charArr,
    doubleVal,
    doubleArr,
    mathOp,
    cmprOp,
    boolOp,
    startGroup,
    endGroup,
    comma,
    semicolon,
    mismatch,
    voidVal,
    negateOp,
    nonExistVar,
    blank,
    none
}