using System;
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

        //checks for variable initialization
        if(ReservedConstants.varTypes.Contains(sections[0])){
            if(!checkInitialize(sections)){
                hasError = true;
                return;
            }
        }
        //checks for variable assignment
        else if(allVars.ContainsKey(sections[0])){
            if(checkAssignment(sections)){
                lineType = LineType.varAssign;
            } else {
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
        //default for all
        else {
            addErr(string.Format("Line {0}: only assignment, call, increment, or decrement can be used as statements!", lineIndex));
            hasError = true;
            return;
        }

        //checks if there are elements after the first semicolon
        //if there are, throws an error
        if(Array.IndexOf(sections, ";") != sections.Length - 1){
            addErr(string.Format("Line {0}: semicolons are meant to terminate statements - put each command on a separate line!", lineIndex));
            hasError = true;
            return;
        }

        //checks for a semicolon at the end
        if(sections[sections.Length - 1] != ";"){
            addErr(string.Format("Line {0}: don't forget to add semicolons at the end of your statements!", lineIndex));
            hasError = true;
            return;
        }
    }

    //TODO: Expand to allow supporting multiple initialization
    private bool checkInitialize(string[] sections){
        if(sections.Length == 1){
            addErr(string.Format("Line{0}: identifier expected!", lineIndex));
            return false;
        }

        if(!isValidVarName(sections[1])){
            return false;
        } else if(allVars.ContainsKey(sections[1])){
            addErr(string.Format("Line {0}: {1} is already defined!", lineIndex, sections[1]));
            return false;
        }

        if(sections.Length == 2){
            addErr(string.Format("Line {0}: ';' or '=' expected!", lineIndex));
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
            
            if(!checkExpression(sections, 3)){
                return false;
            }
        } else {
            addErr(string.Format("Line {0}: unexpected token {1}, was expecting a \".\"!", lineIndex, sections[1]));
            return false;
        }
        return true;
    }

    //checks numerical expression validity
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

    //uses RegEx to check if variable name is valid
    //as always shouldn't start with a digit
    //or include anything other than alphanumeric and semicolon
    //not that the limited editor will allow special chars anyway
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