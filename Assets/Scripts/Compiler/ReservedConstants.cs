using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReservedConstants{
    public static readonly string[] keywords = {
        
    };

    public static readonly string[] compoundOperators = {
        "+=",
        "-=",
        "*=",
        "/=",
        "++",
        "--",
        "<=",
        ">=",
        "==",
        "//"
    };

    public static readonly string[] varTypes = {
        "int",
        "string"
    };

    public const string mathOperators = "+-*/";

    public const string parentheses = "()";
    public const string allMathOperators = mathOperators + parentheses;
    const string comparisonOperators = "<>=";
    //const string textChars = "\"'";
    const string specChars = ".;,";
    public const string allOperators = allMathOperators + comparisonOperators + specChars;
    public static readonly string[] reserved = FunctionHandler.builtInFunctions.Concat(varTypes).ToArray();
}

public enum MathExpression{
    Add,
    Subtract,
    Multiply,
    Divide,
    StartGroup,
    EndGroup,
    Value
}

public enum ArgTypes{
    integer,
    floatpoint,
    truefalse,
    character,
    textstring
}