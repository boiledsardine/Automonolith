using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class StringExpression{
    List<string> words;
    List<MathExpression> elements;
    
    public StringExpression(string expression){
        StringValue valString = new StringValue(expression);
        words = valString.strTexts;
        elements = valString.elements;
    }

    public string removeQuotations(){
        string sentence = "";

        foreach(string s in words){
            if(s.Contains("\"")){
                char[] literal = s.ToCharArray();
                int stringStartIndex = s.IndexOf("\"");
                int stringEndIndex = s.LastIndexOf("\"");
                for(int i = stringStartIndex + 1; i < stringEndIndex; i++){
                    if(literal[i] == '^'){
                        sentence += ' ';
                    } else if(literal[i] == 'あ'){
                        sentence += '{';
                    } else if(literal[i] == 'え'){
                        sentence += '}';
                    } else if(literal[i] == 'い'){
                        sentence += '[';
                    } else if(literal[i] == 'お'){
                        sentence += ']';
                    } else if(literal[i] == 'け'){
                        sentence += ',';
                    } else {
                        sentence += literal[i];
                    }
                }
            } else {
                sentence += s;
            }
        }

        return sentence;
    }
}

public class StringValue{
    Dictionary<string, VariableInfo> allVars;
    Dictionary<string, int> intVars;
    Dictionary<string, string> strVars;
    public List<string> strTexts;
    public List<MathExpression> elements;
    
    public StringValue(string expression){
        this.allVars = Compiler.Instance.allVars;
        this.intVars = Compiler.Instance.intVars;
        this.strVars = Compiler.Instance.strVars;

        strTexts = new List<string>();
        elements = new List<MathExpression>();
        expression = processLiteral(expression);

        string[] sectionArr = expression.Split(' ');
        
        for(int i = 0; i < sectionArr.Length; i++){
            string currentSection = sectionArr[i];

            if (strVars.ContainsKey(currentSection)){
                strTexts.Add(strVars[currentSection]);
            } else {
                strTexts.Add(currentSection);
            }
        }
    }
    
    //replaces whitespaces in literals with '^'
    //which is eventually replaced by StringExpression class
    //prevents shenanigans with anything that splits a string by whitespace
    //shenanigans being literals being split by space too and throwing errors everywhere
    //of course this means that you can't put '^' in a string literal anymore
    //gotta fix that somehow
    //or not
    private string processLiteral(string expression){
        List<string> expressionArr = expression.Split(' ').ToList();

        //iterate through tokens, looking for variables
        //if found variable, replace its position with a literal of its value
        for(int i = 0; i < expressionArr.Count; i++){
            if(Compiler.Instance.strVars.ContainsKey(expressionArr[i])){
                expressionArr.Insert(i, '\"' + Compiler.Instance.strVars[expressionArr[i]] + '\"');
                expressionArr.RemoveAt(i + 1);
            }
        }

        expression = Compiler.arrayToString(expressionArr.ToArray(), 0);

        char[] charArray = expression.ToCharArray();
        bool isLiteral = false;
        string substring = "";
        foreach(char c in charArray){
            if(c == '\"' && !isLiteral){
                isLiteral = true;
                substring += c;
            } else if(c == '\"' && isLiteral){
                isLiteral = false;
                substring += c;
            } else if(c == ' '){
                if(isLiteral){
                    substring += '^';
                } else {
                    substring += c;
                }
            } else {
                substring += c;
            }
        }
        return substring;
    }
}