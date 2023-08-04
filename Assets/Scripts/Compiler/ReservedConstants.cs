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
        "!=",
        "//"
    };

    public static readonly string[] varTypes = {
        "int",
        "int[]",
        "string",
        "bool"
    };

    public static readonly string[] comparisonOperators = {
        "&&",
        "||",
        "<",
        "<=",
        "==",
        ">=",
        ">",
        "!="
    };

    public const string mathOperators = "+-*/";

    public const string parentheses = "()";
    public const string allMathOperators = mathOperators + parentheses;
    const string comparisonString = "<>=";
    //const string textChars = "\"'";
    const string specChars = ".;,";
    public const string arrayIndexSeparator = "`";
    public const string allOperators = allMathOperators + comparisonString + specChars;
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