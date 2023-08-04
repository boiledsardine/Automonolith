using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CodeFormatter {
    public static string Format(string line){
        line = line.Trim();

        List<string> sections = new List<string>();
        string tempString = "";
        bool previousCharOperator = false;
        bool inArrayIndexer = false;

        //goes through the line char by char
        for(int i = 0; i < line.Length; i++){
            char currentChar = line[i];
            bool isOperator = ReservedConstants.allOperators.Contains(currentChar.ToString());
            
            if(currentChar == '['){
                inArrayIndexer = true;
            } else if(currentChar == ']'){
                inArrayIndexer = false;
            }

            if((currentChar == ' ' || isOperator || previousCharOperator) && !inArrayIndexer){
                if(!string.IsNullOrEmpty(tempString)){
                    sections.Add(tempString);
                }
                tempString = "";
            }
            if(isOperator || currentChar != ' '){
                tempString += currentChar;
            }

            if(i == line.Length - 1){
                if(!string.IsNullOrEmpty(tempString)){
                    sections.Add(tempString);
                }
            }
            
            previousCharOperator = isOperator;
        }

        //put something here that turns array var calls into the proper format
        //iterate through sections, looking for array var calls
        if(sections.Count > 0){
            for(int i = 0; i < sections.Count; i++){
                if((sections[i].Contains("[") && sections[i].Contains("]")) && !ReservedConstants.varTypes.Contains(sections[i])){
                    //get substring of everything between "[]"
                    string indexString = Compiler.GetSubstring(sections[i], sections[i].IndexOf("[") + 1, sections[i].IndexOf("]"));

                    //format index value string and evaluate to get the index number
                    int index = new IntExpression(indexString).evaluate();

                    //get the variable (array) name
                    sections[i] = sections[i].Substring(0, sections[i].IndexOf("[")) + ReservedConstants.arrayIndexSeparator + index;
                    Debug.Log(sections[i]);
                }
            }
        }
        
        string formattedLine = "";
        string lastAddedSection = "";
        for(int i = 0; i < sections.Count; i++){
            string compound = lastAddedSection + sections[i];
            if(ReservedConstants.compoundOperators.Contains(compound)){
                formattedLine = formattedLine.Substring(0, formattedLine.Length - 1);
            }

            formattedLine += sections[i];

            if(i != sections.Count - 1){
                formattedLine += ' ';
            }

            lastAddedSection = sections[i];
        }
        return formattedLine;
    }
}