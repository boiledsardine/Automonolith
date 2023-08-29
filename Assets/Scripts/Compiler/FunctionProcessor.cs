using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

//in retrospect i should've documented this spaghetti better
//idk what's going on here tbh
//some dark magic
//and yet it's one of the more important parts of this whole mess
public class FunctionProcessor {
    public List<List<ArgTypes>> argTypes;
    public int argsCount;
    public string[] passedArgs;
    private Dictionary<string, VariableInfo> allVars;

    public FunctionProcessor(string functionLine){
        this.allVars = Compiler.Instance.allVars;
        argTypes = new List<List<ArgTypes>>();
        functionProcessor(functionLine);
    }

    private void functionProcessor(string line){
        string[] lineArray = line.Split(' ');
        int argsStartIndex = Array.IndexOf(lineArray, "(");
        int argsEndIndex = Array.LastIndexOf(lineArray, ")");

        //adds each lineArray element to List sections
        //gets everything between the first and last parentheses
        List<string> sections = new List<string>();
        for(int i = argsStartIndex + 1; i < argsEndIndex; i++){
            sections.Add(lineArray[i]);
        }

        //checks if there are any passed arguments
        //counts passed arguments - increments on comma
        if(sections.Count > 0){
            argsCount = 1;
            foreach(string s in sections){
                if(s == ","){
                    argsCount++;
                }
            }
        }

        //makes new list of string arrays called argsSections
        //each array is one argument expression
        List<string[]> argsSections = new List<string[]>();
        if(argsCount > 0){
            string argsString = arrayToString(sections.ToArray(), 0);
            argsString = argsString.Replace(" , ", ",");
            //Debug.LogWarning(argsString);
            string[] args = argsString.Split(',');
            
            foreach(string s in args){
                if(s.Contains("\"")){
                    string _s = processLiteral(s);
                    argsSections.Add(_s.Split(' '));
                } else {
                    argsSections.Add(s.Split(' '));
                }
            }
        }

        //makes new list of variableinfo lists called typesInArgs
        //checks each element in argsSections
        //if current element is a variable, adds its type to typesInArgs
        List<List<VariableInfo.Type>> typesInArgs = new List<List<VariableInfo.Type>>();
        for(int i = 0; i < argsSections.Count; i++){
            typesInArgs.Add(new List<VariableInfo.Type>());
            for(int j = 0; j < argsSections[i].Length; j++){
                if(allVars.ContainsKey(argsSections[i][j])){
                    typesInArgs[i].Add(allVars[argsSections[i][j]].type);
                }
            }
        }

        //Emperor protects

        //assigns passedArgs array a new array with length = how many arguments were found
        //iterate through passedArgs array
        //for each argument, add new List<ArgTypes> to argTypes list
        passedArgs = new string[argsSections.Count];
        for(int i = 0; i < argsSections.Count; i++){
            argTypes.Add(new List<ArgTypes>());
            string argument = Compiler.arrayToString(argsSections[i], 0);

            //somehow narrow down the expression to a single type
            //maybe using the linechecker's enum system?
            LineChecker lc = new LineChecker();
            ValueType val = lc.CheckTypes(argument);
            
            if(val == ValueType.boolVal){
                argTypes[i].Add(ArgTypes.truefalse);
            } else if(val == ValueType.strVal){
                argTypes[i].Add(ArgTypes.textstring);
            } else if(val == ValueType.intVal){
                argTypes[i].Add(ArgTypes.integer);
            } else if(val == ValueType.intArr){
                argTypes[i].Add(ArgTypes.intArray);
            } else if(val == ValueType.strArr){
                argTypes[i].Add(ArgTypes.stringArray);
            } else if(val == ValueType.boolArr){
                argTypes[i].Add(ArgTypes.boolArray);
            } else if(val == ValueType.charVal){
                argTypes[i].Add(ArgTypes.character);
            } else if(val == ValueType.charArr){
                argTypes[i].Add(ArgTypes.charArray);
            } else if(val == ValueType.doubleVal){
                argTypes[i].Add(ArgTypes.floatpoint);
            } else if(val == ValueType.doubleArr){
                argTypes[i].Add(ArgTypes.doubleArray);
            } 
            passedArgs[i] = arrayToString(argsSections[i].ToArray(), 0);
        }
    }

    private string arrayToString(string[] array, int startIndex){
        string s = "";
        for(int i = startIndex; i < array.Length; i++){
            s += array[i];
            if(i != array.Length - 1){
                s += " ";
            }
        }
        return s;
    }

    //replaces whitespaces in literals with '^'
    //which is eventually replaced by StringExpression class
    //prevents shenanigans with anything that splits a string by whitespace
    //shenanigans being literals being split by space too and throwing errors everywhere
    private string processLiteral(string expression){
        char[] charArray = expression.ToCharArray();
        bool isLiteral = false;
        string substring = "";
        foreach(char c in charArray){
            if(c == '\"' && isLiteral == false){
                isLiteral = true;
                substring += c;
            } else if(c == '\"' && isLiteral == true){
                isLiteral = false;
                substring += c;
            } else if(c == ' '){
                substring += '^';
            } else {
                substring += c;
            }
        }
        return substring;
    }
}