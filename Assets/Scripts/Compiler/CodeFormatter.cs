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

        for(int i = 0; i < line.Length; i++){
            char currentChar = line[i];
            bool isOperator = ReservedConstants.allOperators.Contains(currentChar.ToString());
            
            if(currentChar == ' ' || isOperator || previousCharOperator){
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