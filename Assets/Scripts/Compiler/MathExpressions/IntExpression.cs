using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntExpression{
    List<int> values;
    List<MathExpression> elements;
    public bool isInt = true;

    public IntExpression(string expression){
        IntValue valString = new IntValue(expression);
        values = valString.intValues;
        elements = valString.elements;
        isInt = valString.isInt;
    }

    public int evaluate(){
        resolveGroups();
        return evaluateGroup(elements, values);
    }

    private int evaluateGroup(List<MathExpression> groupElements, List<int> groupValues){
        int valueIndex = 0;

        //resolve multiplication and division
        for(int i = 0; i < groupElements.Count; i++){
            MathExpression elem = groupElements[i];
            if(elem == MathExpression.Value){
                valueIndex++;
            } else if(elem == MathExpression.Multiply || elem == MathExpression.Divide){
                int valueA = groupValues[valueIndex - 1];
                int valueB = groupValues[valueIndex];
                
                if(elem == MathExpression.Multiply){
                    groupValues[valueIndex - 1] = valueA * valueB;
                } else {
                    groupValues[valueIndex - 1] = valueA / valueB;
                }

                groupValues.RemoveAt(valueIndex);
                groupElements.RemoveRange(i - 1, 2);
                i--;
            }
        }

        //sum up remaining terms
        valueIndex = 0;
        int result = 0;
        for(int i = 0; i < groupElements.Count; i++){
            MathExpression element = groupElements[i];
            if(element == MathExpression.Value){
                int sign = 1;

                //Backtrack through operators up to last value
                //to figure out if it should be addition or subtraction
                //This deals with multiple operators like x-(-y) = x+y
                //could turn this into an if statement instead
                //if it hits a value, the loop terminates
                for(int j = i - 1; j >= 0; j--){
                    if(groupElements[j] == MathExpression.Value){
                        break;
                    }
                    if(groupElements[j] == MathExpression.Subtract){
                        sign *= -1;
                    }
                }
                result += groupValues[valueIndex] * sign;
                valueIndex++;
            }
        }
        return result;
    }

    //Recursively replaces expressions inside parentheses with their actual value
    private void resolveGroups(){
        if(elements.Contains(MathExpression.StartGroup)){
            int groupStartIndex = -1;
            int groupEndIndex = -1;
            int deepestGroup = -1;
            int currentDepth = 0;

            //find start/end index of deepest parenthesis group
            for(int i = 0; i < elements.Count; i++){
                switch (elements[i]){
                    case MathExpression.StartGroup:
                        currentDepth++; 
                        if (currentDepth >= deepestGroup){
                            deepestGroup = currentDepth;
                            groupStartIndex = i;
                        }
                        break;
                    case MathExpression.EndGroup:
                        if (currentDepth == deepestGroup){
                            groupEndIndex = i;
                        }
                        currentDepth--;
                        break;
                    default:
                        break;
                }
            }

            if(currentDepth != 0){
                Debug.Log("Unmatched bracket");
                return;
            }
            
            //Find index of first value in current group
            int valueStartIndex = 0;
            for(int i = 0; i < groupStartIndex; i++){
                if(elements[i] == MathExpression.Value){
                    valueStartIndex++;
                }
            }

            //Get list of elements and values inside current group
            List<MathExpression> groupElements = new List<MathExpression>();
            List<int> groupValues = new List<int>();

            int valueIndex = valueStartIndex;
            for(int i = groupStartIndex + 1; i < groupEndIndex; i++){
                groupElements.Add(elements[i]);
                if(elements[i] == MathExpression.Value){
                    groupValues.Add(values[valueIndex]);
                    valueIndex++;
                }
            }

            //Get group value
            int value = evaluateGroup(groupElements, groupValues);

            //Replace group with value
            elements.RemoveRange(groupStartIndex, groupEndIndex - groupStartIndex + 1);
            elements.Insert(groupStartIndex, MathExpression.Value);
            values.RemoveRange(valueStartIndex, (valueIndex - 1) - valueStartIndex);

            values[valueStartIndex] = value;

            resolveGroups();
        }
    }
}

public class IntValue{
    Dictionary<string, string> allVars;
    Dictionary<string, int> intVars;
    Dictionary<string, string> strVars;
    public List<int> intValues;
    public List<string> strTexts;
    public List<MathExpression> elements;
    public bool isInt = true;

    //int ValueString constructor
    public IntValue(string line){
        this.allVars = Compiler.Instance.allVars;
        this.intVars = Compiler.Instance.intVars;
        this.strVars = Compiler.Instance.strVars;

        intValues = new List<int>();
        elements = new List<MathExpression>();
        string[] sectionArr = line.Split(' ');
        
        for(int i = 0; i < sectionArr.Length; i++){
            string currentSection = sectionArr[i];

            if(ReservedConstants.mathOperators.Contains(currentSection)){
                addOperatorElement(currentSection);
            } else if(allVars.ContainsKey(currentSection)){
                if(intVars.ContainsKey(currentSection)){
                    intValues.Add(intVars[currentSection]);
                    elements.Add(MathExpression.Value);
                } else if(strVars.ContainsKey(currentSection)){
                    int value;
                    if(int.TryParse(strVars[currentSection], out value)){
                        intValues.Add(value);
                        elements.Add(MathExpression.Value);
                    }
                } else {
                    Debug.Log("Cannot be cast into int!");
                }
            } else {
                int value;
                if(int.TryParse(currentSection, out value)){
                    intValues.Add(value);
                    elements.Add(MathExpression.Value);
                } else {
                    isInt = false;
                }
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