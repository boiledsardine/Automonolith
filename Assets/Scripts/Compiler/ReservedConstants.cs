using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReservedConstants{
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
    public const string mathOperators = "+-*/()";
    const string comparisonOperators = "<>=";
    const string textChars = "\"'";
    public const string allOperators = mathOperators + comparisonOperators + textChars;
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