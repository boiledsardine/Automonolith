using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ValueString{
    public List<int> intValues;
    public List<ExpressionElement> elements;

    //int ValueString constructor
    public ValueString(string line, Dictionary<string, int> intVars){
        intValues = new List<int>();
        elements = new List<ExpressionElement>();
        string[] sectionArr = line.Split(' ');
        
        for(int i = 0; i < sectionArr.Length; i++){
            string currentSection = sectionArr[i];

            if(ReservedConstants.mathOperators.Contains(currentSection)){
                addOperatorElement(currentSection);
            } else if (intVars.ContainsKey(currentSection)){
                intValues.Add(intVars[currentSection]);
                elements.Add(ExpressionElement.Value);
            } else {
                int value;
                if(int.TryParse(currentSection, out value)){
                    intValues.Add(value);
                    elements.Add(ExpressionElement.Value);
                }
            }
        }
    }

    private void addOperatorElement(string currentSection){
        switch(currentSection){
            case "+":
                elements.Add(ExpressionElement.Add);
                break;
            case "-":
                elements.Add(ExpressionElement.Subtract);
                break;
            case "*":
                elements.Add(ExpressionElement.Multply);
                break;
            case "/":
                elements.Add(ExpressionElement.Divide);
                break;
            case "(":
                elements.Add(ExpressionElement.StartGroup);
                break;
            case ")":
                elements.Add(ExpressionElement.EndGroup);
                break;
        }
    }
}