using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text.RegularExpressions;


public class LineChecker{
    public LineType lineType = LineType.none;
    public bool hasError = false;
    Dictionary<string, string> allVars;
    int lineIndex;
    ErrorChecker errorChecker;

    public LineChecker(string inputLine, int index, ErrorChecker err){
        allVars = Compiler.Instance.allVars;
        lineIndex = index;
        errorChecker = err;
        
        readTokens(inputLine);
    }
    
    private void readTokens(string inputLine){
        //cant check for nonexistent types yet
        if(string.IsNullOrEmpty(inputLine)){
            return;
        }
        
        string[] sections = inputLine.Split(' ');
        if(ReservedConstants.varTypes.Contains(sections[0])){
            if(!checkInitialize(sections)){
                hasError = true;
            }
        } else if(allVars.ContainsKey(sections[0])){
            if(checkAssignment(sections)){
                lineType = LineType.varAssign;
            } else {
                hasError = true;
            }
        } else if(sections[0] == "Bot"){
            if(checkBotCall(sections)){
                lineType = LineType.functionCall;
            } else {
                hasError = true;
            }
        } else {
            addErr(string.Format("Line {0}: only assignment, call, increment, or decrement can be used as statements!", lineIndex));
            hasError = true;
        }
    }

    private bool checkInitialize(string[] sections){
        if(!isValidVarName(sections[1])){
            return false;
        } else if(allVars.ContainsKey(sections[1])){
            addErr(string.Format("Line {0}: {1} is already defined!", lineIndex, sections[1]));
            return false;
        }

        if(sections[2] == "="){
            if(!checkExpression(sections, 3)){
                return false;
            } else {
                lineType = LineType.varAssign;
            }
        } else {
            //check next token - should be a semicolon
            if(sections[2] == ";" && sections.Length == 3){
                lineType = LineType.varInitialize;
            } else if(sections[2] == ";" && sections.Length > 3) {
                Debug.Log("found something after semicolon");
                addErr(string.Format("Line {0}: unexpected token {1}!", lineIndex, sections[3]));
                
                //still allows initialization but is technically an error
                //just so everything else that comes after that calls a techincally correct variable doesn't throw an error
                lineType = LineType.varInitialize;
                return false;
            } else {
                Debug.Log("expected semicolon");
                addErr(string.Format("Line {0}: unexpected token {1}, expected \";\"!", lineIndex, sections[2]));
                return false;
            }
        }

        return true;
    }

    private bool checkAssignment(string[] sections){
        if(allVars.ContainsKey(sections[0])){
            if(sections[1] == "="){
                if(!checkExpression(sections, 2)){
                    return false;
                }
            } else if(sections[1] == ";"){
                addErr(string.Format("Line {0}: only assignment, call, increment, or decrement can be used as statements!", lineIndex));
                return false;
            } else {
                addErr(string.Format("Line {0}: unexpected token {1}!", lineIndex, sections[1]));
                return false;
            }
        } else {
            addErr(string.Format("Line {0}: variable {1} does not exist!", lineIndex, sections[0]));
            return false;
        }
        return true;
    }

    private bool checkBotCall(string[] sections){
        if(sections[1] == "."){
            if(!FunctionHandler.builtInFunctions.Contains(sections[2])){
                addErr(string.Format("Line {0}: Bot has no definition for {1}!", lineIndex, sections[2]));
                return false;
            }
            if(!checkExpression(sections, 3)){
                return false;
            }
        } else {
            addErr(string.Format("Line {0}: unexpected token {1}!", lineIndex, sections[1]));
            return false;
        }
        return true;
    }

    private bool checkExpression(string[] sections, int index){
        bool isExpectingValue = true;
        //everything else should be part of the expression
        for(int i = index; i < sections.Length; i++){
            if(ReservedConstants.mathOperators.Contains(sections[i])){
                if(sections[i - 1] == "="){
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
            if(isExpectingValue && Regex.IsMatch(sections[i], @"^[a-zA-Z0-9_]+$")){
                if(isValidVarName(sections[i]) && allVars.ContainsKey(sections[i])){
                    isExpectingValue = false;
                } else if(isValidVarName(sections[i]) && !allVars.ContainsKey(sections[i])){
                    addErr(string.Format("Line {0}: variable {1} does not exist!", lineIndex, sections[i]));
                    isExpectingValue = false;
                    return false;
                } else {
                    addErr(string.Format("Line {0}: unexpected token {1}!", lineIndex, sections[i]));
                    return false;
                }
            }
        }
        return true;
    }

    private bool isValidVarName(string varName){
        
        if(Regex.IsMatch(varName.ToCharArray()[0].ToString(), @"^[0-9]$")){
            addErr(string.Format("Line {0}: {1} is an invalid name - variable names cannot start with digits!", lineIndex, varName));
            hasError = true;
            return false;
        }

        foreach(char c in varName.ToCharArray()){
            if(!Regex.IsMatch(c.ToString(), @"^[a-zA-Z0-9_]$")){
                addErr(string.Format("Line {0}: unexpected character {1} detected!", lineIndex, c));
                Debug.Log("Unexpected token: " + c);
                hasError = true;
                return false;
            }
        }
        
        return true;
    }
    
    private void addErr(string msg){
        string[] errorMsg = {msg};
        errorChecker.errorConvo.dialogueBlocks.Add(new Dialogue(errorMsg, "ERROR", 'R', true, errorChecker.errorSprite));
    }
}

public enum LineType{
    varInitialize,
    varAssign,
    functionCall,
    none
}