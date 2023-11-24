using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class CodeColorizer{
    static string[] MinigameClasses = {"Student", "Console"};
    static string[] MinigameFunctions = {
        "BoilWater",
        "Write",
        "WriteLine",
        "GiveGrade",
        "Alarm"
    };

    public static string Colorize(string input, bool changeFont, ColorizerTheme theme){
        input = input.Replace('“', '\"');
        input = input.Replace('”', '\"');
        input = input.Replace('‘', '\'');
        input = input.Replace('’', '\'');
        var textLines = input.Split('\n');
        //find tagged groups
        for(int i = 0; i < textLines.Length; i++){
            string line = textLines[i];
            while(line.Contains("~") && line.Contains("`")){
                int start = line.IndexOf("~");
                int end = line.IndexOf("`");
                string text = GetSubstring(line, start + 1, end);
                if(changeFont){
                    text = "<font=\"Perfect DOS VGA 437 Win SDF\">" + ColorizeCode(text, theme) + "</font>";
                } else {
                    text = ColorizeCode(text, theme);
                }
                line = line.Replace(GetSubstring(line, start, end + 1), text);
            }
            textLines[i] = line;
        }

        string formattedEntry = "";
        foreach(string s in textLines){
            formattedEntry += s + "\n";
        }
        formattedEntry = formattedEntry.Trim();
        return formattedEntry;
    }

    static string GetSubstring(string input, int startIndex, int endIndex){
        string s = "";
        for(int i = startIndex; i < endIndex; i++){
            s += input[i];
        }
        return s;
    }

    public static string ColorizeCode(string line, ColorizerTheme theme){
        string[] specialWords = {"dataType"};

        List<Color> colors = new List<Color>();
        List<int> stringStartIndices = new List<int>();
        List<int> stringEndIndices = new List<int>();

        string[] sections = CodeFormatter.Format(line, true).Split(' ');
        int nonSpace = 0;
        bool isComment = false;

        for(int i = 0; i < sections.Length; i++){
            Color color = Color.clear;
            string section = sections[i];

            //identify sections and assign colors
            if(section == "//" || isComment){
                color = theme.commentColor;
                isComment = true;
            } else if(section == "if" || section == "else"){
                color = theme.conditionalColor;
            } else if(section == "while"){
                color = theme.loopColor;
            } else if(ReservedConstants.allOperatorsArr.Contains(section) || ReservedConstants.specChars.Contains(section)){
                color = theme.operatorColor;
            } else if(section == "(" || section == ")"){
                color = theme.braceColor1;
            } else if(section == "{" || section == "}"){
                color = theme.braceColor2;
            } else if(section == "[" || section == "]"){
                color = theme.braceColor3;
            } else if(section.Contains("\"") || section.Contains("\'")){
                color = theme.stringColor;
            } else if(int.TryParse(section, out _) || float.TryParse(section.Replace('.', ','), out _)){
                color = theme.numColor;
            } else if(ReservedConstants.keywords.Contains(section) || specialWords.Contains(section)){
                color = theme.keywordColor;  
            } else if(section == "Bot" || MinigameClasses.Contains(section)){
                color = theme.botColor;
            } else if(FunctionHandler.builtInFunctions.Contains(section) || MinigameFunctions.Contains(section)){
                color = theme.funcColor;
            } else {
                color = theme.varColor;
            }

            //tracks start and end indices of words checked
            if (color != Color.clear) {
                colors.Add (color);
                int endIndex = nonSpace + sections[i].Length;
                stringStartIndices.Add(nonSpace);
                stringEndIndices.Add(endIndex);
            }
            nonSpace += sections[i].Length;
        }

        //find spaces between tokens in code and start/end indices for words
        //spaces demarcate tokens and separate color tags
        //could've split the tokens into an array instead
        //but merging them will not always result in the same thing the user inputted
        if(colors.Count > 0){
            nonSpace = 0;
            int colorIndex = 0;
            int startIndex = -1;
            bool isLiteral = false;

            for(int i = 0; i <= line.Length; i++){
                if(nonSpace == stringStartIndices[colorIndex]){
                    startIndex = i;
                } else if(nonSpace == stringEndIndices[colorIndex]){
                    stringStartIndices[colorIndex] = startIndex;
                    stringEndIndices[colorIndex] = i;

                    colorIndex++;
                    if(colorIndex >= colors.Count){
                        break;
                    }
                    i--;
                    continue;
                }

                if(i < line.Length){
                    char c = line[i];

                    //count spaces in literals as non-spaces
                    //not doing so leads to funky shenanigans in the editor
                    //such as in-literal spaces displacing the color tags
                    if((c == '\"' || c == '\'') && !isLiteral){
                        isLiteral = true;
                    } else if((c == '\"' || c == '\'') && isLiteral){
                        isLiteral = false;
                    }

                    if(c != ' ' || (c == ' ' && isLiteral)){
                        nonSpace++;
                    }
                }
            }
        }

        //actually apply colors
        for(int i = colors.Count - 1; i >= 0; i--){
            var col = colors[i];
            string colorString = ColorUtility.ToHtmlStringRGB(col);

            line = line.Insert(stringEndIndices[i], "</color>");
            line = line.Insert(stringStartIndices[i], "<color=#" + colorString + ">");
        }

        return line;
    }
}
