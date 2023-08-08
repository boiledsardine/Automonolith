using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BoolExpression {
    public enum Element{ And, Or, StartGroup, EndGroup, Value }

    List<bool> values;
    List<Element> elements;

    public BoolExpression(List<bool> values, List<Element> elements){
        this.values = values;
        this.elements = elements;
    }

    public bool Evaluate(){
        return EvaluateGroup(values, elements);
    }

    public bool EvaluateGroup(List<bool> groupValues, List<Element> groupElements){
        /*foreach(Element e in groupElements){
            Debug.Log(e);
        }*/

        int valueIndex = 0;
        //calculate AND operations first
        for(int i = 0; i < elements.Count; i++){
            Element element = elements[i];
            if(element == Element.Value){
                valueIndex++;
            } else if(element == Element.And){
                bool result = groupValues[valueIndex - 1] && groupValues[valueIndex];
                groupValues[valueIndex - 1] = result;
                groupValues.RemoveAt(valueIndex);
                groupElements.RemoveRange(i - 1, 2);
                i--;
            }
        }

        //calculate remaining OR operations
        for(int i = 0; i < groupValues.Count; i++){
            if(groupValues[i]){
                return true;
            }
        }
        return false;
    }
}
