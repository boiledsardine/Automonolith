using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Compiler : MonoBehaviour{
    string code;
    string[] codeLines;
    string processedCode;    
    Dictionary<string, int> intVariables;
    List<string> functionNames;
    List<VirtualFunction> outputFunctions;


    public Compiler(string input){
        if(input == null){
            input = "";
        }
        intVariables = new Dictionary<string, int>();
        functionNames = new List<string>();
        outputFunctions = new List<VirtualFunction>();
    }

    public void addFunctions(string name){
        functionNames.Add(name);
    }

    public List<VirtualFunction> Run(){
        runLines(0, codeLines.Length);
        return outputFunctions;
    }
    
    private void preprocessCode(string rawCode){
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

    private void runLines(int lineIndex, int stopIndex){
        if(lineIndex >= codeLines.Length || lineIndex == stopIndex){
            return;
        }
        
        string currentLine = codeLines[lineIndex];
        string[] sections = currentLine.Split(' ');

        if(sections.Length > 1 && sections.Contains("=")){

        }

        runLines(lineIndex + 1, stopIndex);
    }

    private void assignVariable(string line){
        string[] sections = line.Split(' ');
        int operatorIndex = Array.IndexOf(sections, "=");
        string varName = sections[operatorIndex - 1];
        string expression = line.Split('=')[1].Trim();
        string[] delim = {varName};
        string varType = line.Split(delim, StringSplitOptions.None)[0].Trim();

        if(varType != null){
            switch(varType){
                case "int":
                break;
            }
        }
    }
    

    private string formatOperator(string[] array, string opChar){        
        return string.Format("{0} = {0} {1} ( {2} )", array[0], opChar,
        arrayToString(array, 2));
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

public class VirtualFunction{
    public string name = "";
    public List<int> intValues = new List<int>();   
}