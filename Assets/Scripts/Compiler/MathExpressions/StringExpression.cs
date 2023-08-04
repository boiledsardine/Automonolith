using System.Collections;
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

            if(ReservedConstants.mathOperators.Contains(currentSection)){
                addOperatorElement(currentSection);
            } else if (strVars.ContainsKey(currentSection)){
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
        char[] charArray = expression.ToCharArray();
        bool isLiteral = false;
        string substring = "";
        foreach(char c in charArray){
            if(c == '\"' && isLiteral == false){
                isLiteral = true;
                substring += c;
            } else if(c == '\"' && isLiteral == true){
                isLiteral = false;
                substring += c;
            } else if(c == ' '){
                substring += '^';
            } else {
                substring += c;
            }
        }
        return substring;
    }

    private void addOperatorElement(string currentSection){
        switch(currentSection){
            case "+":
                elements.Add(MathExpression.Add);
                break;
            case "-":
                elements.Add(MathExpression.Subtract);
                break;
            case "*":
                elements.Add(MathExpression.Multiply);
                break;
            case "/":
                elements.Add(MathExpression.Divide);
                break;
            case "(":
                elements.Add(MathExpression.StartGroup);
                break;
            case ")":
                elements.Add(MathExpression.EndGroup);
                break;
        }
    }
}