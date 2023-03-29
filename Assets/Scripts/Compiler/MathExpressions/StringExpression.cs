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
            if(s != "\""){
                sentence += s + " ";
            }
        }
        
        return sentence.Trim();
    }
}

public class StringValue{
    Dictionary<string, string> allVars;
    Dictionary<string, int> intVars;
    Dictionary<string, string> strVars;
    public List<string> strTexts;
    public List<MathExpression> elements;
    
    public StringValue(string line){
        this.allVars = Compiler.Instance.allVars;
        this.intVars = Compiler.Instance.intVars;
        this.strVars = Compiler.Instance.strVars;

        strTexts = new List<string>();
        elements = new List<MathExpression>();
        string[] sectionArr = line.Split(' ');
        
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