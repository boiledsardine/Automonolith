using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BoolExpression {
    public enum Element{ And, Or, StartGroup, EndGroup, Value }

    public BoolExpression(){}

    public bool Evaluate(List<bool> groupValues, List<Element> groupElements){
        int valueIndex = 0;
        //calculate AND operations first
        List<Element> elements = groupElements;
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

    public static string ProcessBool(string line){
        line = ReplaceTrueFalse(line);
        line = SimplifyBoolExpression(line);
        return line;
    }

    public static string SimplifyBoolExpression(string line){
        List<string> sections = line.Split(' ').ToList();
        
        //check if line is already as simple as it gets
        if(sections.Count == 1 || !sections.Contains("(")){
            return line;
        }

        //find deepest group
        int startIndex = 0;
        int endIndex = 0;
        for(int i = 0; i < sections.Count; i++){
            if(sections[i] == "("){
                startIndex = i;
            } else if(sections[i] == ")"){
                endIndex = i;
                break;
            }
        }
        
        //get deepest group
        List<string> subList = new List<string>();
        for(int i = startIndex + 1; i < endIndex; i++){
            subList.Add(sections[i]);
        }
        
        //find operators
        bool foundMath = false;
        bool foundCompare = false;
        bool foundBoolean = false;
        foreach(string s in subList){
            if(ReservedConstants.booleanOperators.Contains(s)){
                foundBoolean = true;
            }
            if(ReservedConstants.comparisonOperators.Contains(s)){
                foundCompare = true;
            }
            if(ReservedConstants.mathOperators.Contains(s)){
                foundMath = true;
            }
        }
        
        //process
        string e = Compiler.arrayToString(subList.ToArray(), 0);
        string evaled;
        bool yieldedBool = false;
        if(foundCompare || foundBoolean){
            evaled = GetConditionResult("( " + e + " )").ToString();
            yieldedBool = true;
        } else if(foundMath){
            var expression = new IntExpression(e);
            evaled = expression.evaluate().ToString();
        } else {
            evaled = e;
        }
        
        //replace
        sections.RemoveRange(startIndex, endIndex - startIndex + 1);
        sections.Insert(startIndex, evaled);

        //check negate
        if(yieldedBool){
            if(startIndex > 0 && sections[startIndex - 1] == "!"){
                Console.WriteLine("negate!");
                if(sections[startIndex].ToLower() == "true"){
                    sections[startIndex] = "false";
                    sections.RemoveAt(startIndex - 1);
                } else if(sections[startIndex].ToLower() == "false"){
                    sections[startIndex] = "true";
                    sections.RemoveAt(startIndex - 1);
                }
            }
        }
        
        string result;
        if(sections.Contains("(")){
            result = SimplifyBoolExpression(Compiler.arrayToString(sections.ToArray(), 0));
        } else {
            result = Compiler.arrayToString(sections.ToArray(), 0);
        }
        
        return result;
    }

    public static string ReplaceTrueFalse(string line){
        List<string> sections = line.Split(' ').ToList();
        for(int i = 0; i < sections.Count; i++){
            string s = sections[i];
            //check if boolean value
            if(s.ToLower() == "true"){
                string[] arr = {"(", "0", "==", "0", ")"};
                sections.InsertRange(i, arr);
                sections.RemoveAt(i + 5);
                i += 5;
            }
            if(s.ToLower() == "false"){
                string[] arr = {"(", "0", "!=", "0", ")"};
                sections.InsertRange(i, arr);
                sections.RemoveAt(i + 5);
                i += 5; 
            }

            //check if boolean variable
            if(Compiler.Instance.allVars.ContainsKey(s) && Compiler.Instance.allVars[s].type == VariableInfo.Type.boolVar){
                if(Compiler.Instance.boolVars[s] == true){
                    string[] arr = {"(", "0", "==", "0", ")"};
                    sections.InsertRange(i, arr);
                    sections.RemoveAt(i + 5);
                    i += 5;
                } else {
                    string[] arr = {"(", "0", "!=", "0", ")"};
                    sections.InsertRange(i, arr);
                    sections.RemoveAt(i + 5);
                    i += 5;
                }
            }
        }
        
        return Compiler.arrayToString(sections.ToArray(), 0);
    }

    //TODO: string comparison support
    public static bool GetConditionResult(string line){
        int startIndex = line.IndexOf('(') + 1;
        int endIndex = line.LastIndexOf(')');
        string conditionString = Compiler.GetSubstring(line, startIndex, endIndex);

        //check if currentString has a boolean variable or value
        //preprocess code to replace stuff
        //way this works is it can't take booleans as-is
        //so convert booleans to numerical comparisons that evaluate as true or false
        
        conditionString = ReplaceTrueFalse(conditionString);
        List<string> sections = conditionString.Split(' ').ToList();
        
        var numValues = new List<int>();
        var operators = new List<string>();
        string currentString = "";

        var boolValues = new List<bool>();
        var boolOperators = new List<Element>();

        //simplify condition by finding/evaluating numerical expressions within
        //store as list of values and operators
        for(int i = 0; i < sections.Count; i++){
            string section = sections[i];
            bool isConditionOperator = ReservedConstants.comparisonOperators.Contains(section) ||
                ReservedConstants.booleanOperators.Contains(section);

            if(isConditionOperator || i == sections.Count - 1){
                //checks if condition operator
                if(isConditionOperator){
                    operators.Add(section);
                }

                //currentString is a group that isn't an operator
                //so like 10 + 5 > 20
                //currentString 1 is 10 + 5, then 20
                //currentString is purged when iterator hits an operator

                if(!string.IsNullOrEmpty(currentString)){
                    var expression = new IntExpression(currentString);
                    numValues.Add(expression.evaluate());
                    currentString = "";
                }
            } else {                
                currentString += section + " ";
            }
        }

        //evaluate comparisons to bools
        for(int i = 0; i < operators.Count; i++){
            int a = numValues[i];
            int b = numValues[i + 1];
            string op = operators[i];

            switch(op){
                case "<":
                    boolValues.Add(a < b);
                    boolOperators.Add(Element.Value);
                break;
                case "<=":
                    boolValues.Add(a <= b);
                    boolOperators.Add(Element.Value);
                break;
                case ">":
                    boolValues.Add(a > b);
                    boolOperators.Add(Element.Value);
                break;
                case ">=":
                    boolValues.Add(a >= b);
                    boolOperators.Add(Element.Value);
                break;
                case "==":
                    boolValues.Add(a == b);
                    boolOperators.Add(Element.Value);
                break;
                case "!=":
                    boolValues.Add(a != b);
                    boolOperators.Add(Element.Value);
                break;
                case "&&":
                    boolOperators.Add(Element.And);
                break;
                case "||":
                    boolOperators.Add(Element.Or);
                break;
            }
        }

        BoolExpression boolEx = new BoolExpression();
        bool result = boolEx.Evaluate(boolValues, boolOperators);
        return result;
    }
}
