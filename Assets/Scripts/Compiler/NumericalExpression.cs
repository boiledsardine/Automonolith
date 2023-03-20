using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumericalExpression {
    List<int> intValues;
    List<ExpressionElement> elements;

    //int NumericalExpression constructor
    public NumericalExpression(string expression, Dictionary<string, int> intVars){
        ValueString intValueString = new ValueString(expression, intVars);
        intValues = intValueString.intValues;
        elements = intValueString.elements;
    }

    public int evaluateInt(){
        return evaluateGroup(elements, intValues);
    }

    int evaluateGroup(List<ExpressionElement> groupElements, List<int> groupValues){
        int valueIndex = 0;

        for(int i = 0; i < groupElements.Count; i++){
            ExpressionElement element = groupElements[i];
            if(element == ExpressionElement.Value){
                valueIndex++;
            } else if(element == ExpressionElement.Multply || element == ExpressionElement.Divide){
                int valueA = groupValues[valueIndex - 1];
                int valueB = groupValues[valueIndex];
                int resultA = (element == ExpressionElement.Multply) ? valueA * valueB : valueA / valueB;
                groupValues[valueIndex - 1] = resultA;
                groupValues.RemoveAt(valueIndex);
                groupElements.RemoveRange(i - 1, 2);
                i--;
            }
        }

        valueIndex = 0;
        int result = 0;
        for(int i = 0; i < groupElements.Count; i++){
            ExpressionElement element = groupElements[i];
            if(element == ExpressionElement.Value){
                int sign = 1;

                for(int j = i - 1; j >= 0; j--){
                    if(groupElements[j] == ExpressionElement.Value){
                        break;
                    }
                    if (groupElements[j] == ExpressionElement.Subtract){
                        sign *= -1;
                    }
                }

                result += groupValues[valueIndex] * sign;
                valueIndex++;
            }
        }
        return result;
    }

    private void intResolveParentheses(){
        if(elements.Contains(ExpressionElement.StartGroup)){
            int groupStartIndex = -1;
            int groupEndIndex = -1;
            int deepestGroupIndex = -1;

            int currentDepth = 0;

            //find start/end index of deepest nested parenthesis pair
            for(int i = 0; i < elements.Count; i++){
                if(elements[i] == ExpressionElement.StartGroup){
                    currentDepth++;
                    if(currentDepth >= deepestGroupIndex){
                        deepestGroupIndex = currentDepth;
                        groupStartIndex = i;
                    }
                } else if(elements[i] == ExpressionElement.EndGroup){
                    if(currentDepth == deepestGroupIndex){
                        groupEndIndex = i;
                    }
                    currentDepth--;
                }
            }

            if(currentDepth != 0){
                Debug.Log("Unmatched bracket");
                return;
            }

            int valueStartIndex = 0;
            for(int i = 0; i < groupStartIndex; i++){
                if(elements[i] == ExpressionElement.Value){
                    valueStartIndex++;
                }
            }

            List<ExpressionElement> groupElements = new List<ExpressionElement>();
            List<int> intGroupValues = new List<int>();

            int valueIndex = valueStartIndex;
            for(int i = groupStartIndex + 1; i < groupEndIndex; i++){
                groupElements.Add(elements[i]);
                if(elements[i] == ExpressionElement.Value){
                    intGroupValues.Add(intValues[valueIndex]);
                    valueIndex++;
                }
            }

            int value = evaluateGroup(groupElements, intGroupValues);

            elements.RemoveRange(groupStartIndex, groupEndIndex - groupStartIndex + 1);
            elements.Insert(groupStartIndex, ExpressionElement.Value);
            intValues.RemoveRange(valueStartIndex, (valueIndex - 1) - valueStartIndex);

            intValues[valueStartIndex] = value;
            intResolveParentheses();
        }
    }
}