using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
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

        List<string> sections = new List<string>();
        for(int i = argsStartIndex + 1; i < argsEndIndex; i++){
            sections.Add(lineArray[i]);
        }

        if(sections.Count > 0){
            argsCount = 1;
            foreach(string s in sections){
                if(s == ","){
                    argsCount++;
                }
            }
        }

        List<string[]> argsSections = new List<string[]>();
        if(argsCount > 0){
            string argsString = arrayToString(sections.ToArray(), 0);
            argsString = argsString.Replace(" , ", ",");
            string[] args = argsString.Split(',');
            foreach(string s in args){
                argsSections.Add(s.Split(' '));
            }
        }

        List<List<string>> typesInArgs = new List<List<string>>();
        for(int i = 0; i < argsSections.Count; i++){
            typesInArgs.Add(new List<string>());
            for(int j = 0; j < argsSections[i].Length; j++){
                if(allVars.ContainsKey(argsSections[i][j])){
                    typesInArgs[i].Add(allVars[argsSections[i][j]]);
                }
            }
        }

        passedArgs = new string[argsSections.Count];
        for(int i = 0; i < argsSections.Count; i++){
            argTypes.Add(new List<ArgTypes>());
            bool nonInt = false;
            if(argsSections[i].Contains("\"") || typesInArgs[i].Contains("string")){
                nonInt = true;
                argTypes[i].Add(ArgTypes.textstring);
            }
            if(argsSections[i].Contains("'") || typesInArgs[i].Contains("char")){
                nonInt = true;
                argTypes[i].Add(ArgTypes.character);
            }
            if(argsSections[i].Contains("true") || argsSections[i].Contains("false") || typesInArgs[i].Contains("bool")){
                nonInt = true;
                argTypes[i].Add(ArgTypes.truefalse);
            }
            if(arrayToString(argsSections[i], 0).Contains(".") || typesInArgs[i].Contains("float")){
                nonInt = true;
                argTypes[i].Add(ArgTypes.floatpoint);
            }
            if(!nonInt){
                argTypes[i].Add(ArgTypes.integer);
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
}