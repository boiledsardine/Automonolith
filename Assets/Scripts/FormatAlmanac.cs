#if(UNITY_EDITOR)

using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

public class FormatAlmanac : EditorWindow{
    private ColorizerTheme theme;
    private AlmanacEntry entry;

    [MenuItem("Tools/Format Almanac Entry")]
    public static void ShowWindow() {
        EditorWindow window = GetWindow<FormatAlmanac>();
        window.minSize = new Vector2Int(250, 400);
    }

    private void OnGUI(){
        theme = EditorGUILayout.ObjectField("Colorizer Theme", theme, typeof(ColorizerTheme), false) as ColorizerTheme;
        entry = EditorGUILayout.ObjectField("Almanac Entry", entry, typeof(AlmanacEntry), false) as AlmanacEntry;

        if(GUILayout.Button("Colorize")){
            entry.articleText = Format(entry.articleText);
            entry.articleExample = Format(entry.articleExample);
        }
    }

    string Format(string input){
        var textLines = input.Split('\n');
        //find tagged groups
        for(int i = 0; i < textLines.Length; i++){
            string line = textLines[i];
            while(line.Contains("~") && line.Contains("`")){
                int start = line.IndexOf("~");
                int end = line.IndexOf("`");
                string text = GetSubstring(line, start + 1, end);
                text = "<font=\"Cascadia\">" + ColorizeCode(text) + "</font>";
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

    string GetSubstring(string input, int startIndex, int endIndex){
        string s = "";
        for(int i = startIndex; i < endIndex; i++){
            s += input[i];
        }
        return s;
    }
    
    string ArrayToString(string[] array){
        string text = "";
        foreach(string s in array){
            text += s + " ";
        }
        text = text.Trim();
        return text;
    }

    public string ColorizeCode(string line){
        string[] specialWords = {"dataType"};

        List<Color> colors = new List<Color>();
        List<int> stringStartIndices = new List<int>();
        List<int> stringEndIndices = new List<int>();

        string formattedLine = CodeFormatter.Format(line);
        string[] sections = formattedLine.Split(' ');
        int nonSpace = 0;
        bool isComment = false;

        for(int i = 0; i < sections.Length; i++){
            Color color = Color.clear;
            string section = sections[i];

            if(section == "//" || isComment){
                color = theme.commentColor;
                isComment = true;
            } else if(section == "if" || section == "else"){
                color = theme.conditionalColor;
            } else if(section == "while"){
                color = theme.loopColor;
            } else if(ReservedConstants.allOperatorsArr.Contains(section)){
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
            } else if(section == "Bot"){
                color = theme.botColor;
            } else if(FunctionHandler.builtInFunctions.Contains(section)){
                color = theme.funcColor;
            } else {
                color = theme.varColor;
            }

            if (color != Color.clear) {
                colors.Add (color);
                int endIndex = nonSpace + sections[i].Length;
                stringStartIndices.Add(nonSpace);
                stringEndIndices.Add(endIndex);
            }
            nonSpace += sections[i].Length;
        }

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

                    if(c == '\"' && !isLiteral){
                        isLiteral = true;
                    } else if(c == '\"' && isLiteral){
                        isLiteral = false;
                    }

                    //this is the condition causing the error
                    if(c != ' ' || (c == ' ' && isLiteral)){
                        nonSpace++;
                    }
                }
            }
        }

        for(int i = colors.Count - 1; i >= 0; i--){
            var col = colors[i];
            string colorString = ColorUtility.ToHtmlStringRGB(col);

            line = line.Insert(stringEndIndices[i], "</color>");
            line = line.Insert(stringStartIndices[i], "<color=#" + colorString + ">");
        }

        return line;
    }
}

#endif