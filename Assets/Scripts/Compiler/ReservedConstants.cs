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
    public const string allOperators = mathOperators + comparisonOperators;
}

public enum ExpressionElement{
    Add,
    Subtract,
    Multply,
    Divide,
    StartGroup,
    EndGroup,
    Value
}