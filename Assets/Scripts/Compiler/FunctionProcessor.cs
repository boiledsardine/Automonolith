using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class FunctionProcessor {
    public List<List<ArgTypes>> argTypes;
    public int argsCount;
    public string[] passedArgs;
    private Dictionary<string, string> allVars;

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

        //makes new list of string lists called typesInArgs
        //checks each element in argsSections
        //if current element is a variable, adds its type to typesInArgs
        List<List<string>> typesInArgs = new List<List<string>>();
        for(int i = 0; i < argsSections.Count; i++){
            typesInArgs.Add(new List<string>());
            for(int j = 0; j < argsSections[i].Length; j++){
                if(allVars.ContainsKey(argsSections[i][j])){
                    typesInArgs[i].Add(allVars[argsSections[i][j]]);
                }
            }
        }

        //Regex matching patterns
        //Emperor protects
        string intPattern = @"^[0-9]+$";
        string stringPattern = "^\".*\"$";

        //assigns passedArgs array a new array with length = how many arguments were found
        //iterate through passedArgs array
        //for each argument, add new List<ArgTypes> to argTypes list
        passedArgs = new string[argsSections.Count];
        for(int i = 0; i < argsSections.Count; i++){
            argTypes.Add(new List<ArgTypes>());

            for(int j = 0; j < argsSections[i].Length; j++){
                string s = argsSections[i][j];
                s = s.Trim();

                if(Regex.IsMatch(s, intPattern) || typesInArgs[i].Contains("int")){
                    argTypes[i].Add(ArgTypes.integer);
                }
                if(Regex.IsMatch(s, stringPattern) || typesInArgs[i].Contains("string")){
                    argTypes[i].Add(ArgTypes.textstring);
                }
                passedArgs[i] = arrayToString(argsSections[i].ToArray(), 0);
            }
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

    //replaces whitespaces in literals with '|'
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
                substring += '|';
            } else {
                substring += c;
            }
        }
        return substring;
    }
}