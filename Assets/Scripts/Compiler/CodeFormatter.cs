using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CodeFormatter {
    //formats code line by line
    public static string Format(string line, bool colorizerFormatting){
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
                } else if(currentChar == '{' && !colorizerFormatting){
                    tempString += 'あ';
                } else if(currentChar == '}' && !colorizerFormatting){
                    tempString += 'え';
                } else if(currentChar == '[' && !colorizerFormatting){
                    tempString += 'い';
                } else if(currentChar == ']' && !colorizerFormatting){
                    tempString += 'お';
                } else if(currentChar == ',' && !colorizerFormatting){
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

        var formattedSections = formattedLine.Split(' ').ToList();
        for(int i = 0; i < formattedSections.Count; i++){
            if(formattedSections[i] == "." && i > 0 && i < formattedSections.Count - 1){
                string beforeDot = formattedSections[i - 1];
                string afterDot = formattedSections[i + 1];

                if(int.TryParse(beforeDot, out _) && int.TryParse(afterDot, out _)){
                    //probs a float val
                    int beforeDotIndex = i - 1;
                    string floatString = beforeDot + "." + afterDot;
                    formattedSections.RemoveRange(beforeDotIndex, 3);
                    formattedSections.Insert(beforeDotIndex, floatString);
                    i = beforeDotIndex;
                }
            }
        }

        formattedLine = "";
        for(int i = 0; i < formattedSections.Count; i++){
            formattedLine += formattedSections[i];

            if(i != formattedSections.Count - 1){
                formattedLine += ' ';
            }
        }
        formattedLine = formattedLine.Trim();

        return formattedLine;
    }
}