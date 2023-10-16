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
        bool inLiteral = false;
        bool inCharLiteral = false;

        int squareDepth = 0;
        //goes through the line char by char
        for(int i = 0; i < line.Length; i++){
            char currentChar = line[i];
            
            bool isOperator = ReservedConstants.allOperators.Contains(currentChar.ToString());
            
            //if inArrayIndexer, spaces are ignored
            if(currentChar == '['){
                inArrayIndexer = true;
                squareDepth++;
            } else if(currentChar == ']'){
                squareDepth--;
                if(squareDepth <= 0){
                    inArrayIndexer = false;
                }
            }

            //if inLiteral, spaces are replaced with ^
            if(inLiteral || inCharLiteral){
                if(currentChar == ' '){
                    tempString += '^';
                } else if(currentChar == '{'){
                    tempString += 'あ';
                } else if(currentChar == '}'){
                    tempString += 'え';
                } else if(currentChar == '['){
                    tempString += 'い';
                } else if(currentChar == ']'){
                    tempString += 'お';
                } else if(currentChar == ','){
                    tempString += 'け';
                }
            }

            //adds a full token
            if((currentChar == ' ' || isOperator || previousCharOperator) && !inArrayIndexer && !inLiteral && !inCharLiteral){
                if(!string.IsNullOrEmpty(tempString)){
                    sections.Add(tempString);
                }
                tempString = "";
            }

            if(isOperator || currentChar != ' '){
                bool addChar = true;
                if(currentChar == ',' && (inLiteral || inCharLiteral)){
                    addChar = false;
                }

                if(addChar){
                    tempString += currentChar;
                }
            }

            if(currentChar == '\"' && !inLiteral){
                inLiteral = true;
            } else if(currentChar == '\"' && inLiteral){
                inLiteral = false;
                sections.Add(tempString);
                tempString = "";
            }

            if(currentChar == '\'' && !inCharLiteral){
                inCharLiteral = true;
            } else if(currentChar == '\'' && inCharLiteral){
                inCharLiteral = false;
                sections.Add(tempString);
                tempString = "";
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