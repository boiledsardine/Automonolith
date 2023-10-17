using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AlmanacManager : MonoBehaviour{
    public TMP_Text titleText, mainText, exampleText;
    public AlmanacEntry[] entries;
    public Animator helpPanelAnim;
    public Canvas helpCanvas;
    public GameObject buttonContainer;
    public GameObject almanacButton;
    public ColorizerTheme theme;

    void Start(){
        for(int i = 0; i < entries.Length; i++){
            var almButton = Instantiate(almanacButton);
            var textObj = almButton.transform.GetChild(0);
            textObj.gameObject.GetComponent<TMP_Text>().text = entries[i].articleName;
            var button = almButton.GetComponent<AlmanacButton>();
            button.managerName = gameObject.name;
            button.index = i;
            almButton.transform.SetParent(buttonContainer.transform);
            almButton.transform.localScale = new Vector3(1,1,1);
        }
        buttonContainer.GetComponent<ResizeScrollObject>().Resize();
        OpenHelp();
    }

    public void LoadEntry(int index){
        Debug.Log(index);
        titleText.text = entries[index].articleName;
        mainText.text = Format(entries[index].articleText);
        exampleText.text = Format(entries[index].articleExample);
    }

    public void OpenHelp(){
        helpCanvas.gameObject.SetActive(true);
        helpPanelAnim.SetBool("isOpen", true);
    }

    public void CloseHelp(){
        helpPanelAnim.SetBool("isOpen", false);
        Invoke("disableHelp", 0.25f);
    }

    void DisableHelp(){
        helpCanvas.gameObject.SetActive(false);
    }

    string Format(string input){
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

        string[] sections = CodeFormatter.Format(line).Split(' ');
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

                    if((c == '\"' || c == '\'') && !isLiteral){
                        isLiteral = true;
                    } else if((c == '\"' || c == '\'') && isLiteral){
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
