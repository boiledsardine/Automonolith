using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReservedConstants{
    public static readonly string[] keywords = {
        "int",
        "int[]",
        "string",
        "string[]",
        "bool",
        "bool[]",
        "char",
        "char[]",
        "float",
        "float[]",
        "double",
        "double[]",
        "while",
        "if",
        "else",
        "new",
        "abstract",
        "as",
        "base",
        "break",
        "byte",
        "case",
        "catch",
        "checked",
        "class",
        "const",
        "continue",
        "decimal",
        "default",
        "delegate",
        "do",
        "enum",
        "event",
        "explicit",
        "extern",
        "true",
        "false",
        "finally",
        "fixed",
        "for",
        "foreach",
        "goto",
        "implicit",
        "in",
        "int",
        "interface",
        "internal",
        "is",
        "lock",
        "long",
        "namespace",
        "new",
        "null",
        "object",
        "operator",
        "out",
        "override",
        "params",
        "private",
        "protected",
        "public",
        "readonly",
        "ref",
        "return",
        "sbyte",
        "sealed",
        "short",
        "sizeof",
        "stackalloc",
        "static",
        "struct",
        "switch",
        "this",
        "throw",
        "try",
        "typeof",
        "uint",
        "ulong",
        "unchecked",
        "unsafe",
        "ushort",
        "using",
        "virtual",
        "void",
        "volatile"
    };

    public static readonly string[] accessModifiers = {
        "public",
        "private",
        "protected",
        "internal",
        "virtual",
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
        "//",
        "&&",
        "||"
    };

    public static readonly string[] assignmentOperators = {
        "+=",
        "-=",
        "*=",
        "/=",
        "="
    };

    public static readonly string[] varTypes = {
        "int",
        "int[]",
        "string",
        "string[]",
        "bool",
        "bool[]",
        "char",
        "char[]",
        "float",
        "float[]",
        "double",
        "double[]"
    };

    public static readonly string[] arrVarTypes = {
        "int[]",
        "string[]",
        "bool[]"
    };

    public static readonly string[] outOfScopeTypes = {
        "float",
        "float[]",
        "double",
        "double[]"
    };

    public static readonly string[] comparisonOperators = {
        "<",
        "<=",
        "==",
        ">=",
        ">",
        "!="
    };

    public static readonly string[] booleanOperators ={
        "&&",
        "||"
    };

    public static readonly string[] allOperatorsArr = {
        "+", "-", "/", "*", "%",
        "<", "<=", "==", ">=", ">", "!=",
        "&&", "||",
        "+=", "-=", "*=", "/=", "="
    };

    public const string mathOperators = "+-*/%";

    public const string parentheses = "()";
    public const string allMathOperators = mathOperators + parentheses;
    const string comparisonString = "<>=";
    //const string textChars = "\"'";
    public const string specChars = "!.;,&|{}";
    public const string arrayIndexSeparator = "`";
    public const string allOperators = allMathOperators + comparisonString + specChars;
    public static readonly string[] reserved = FunctionHandler.builtInFunctions.Concat(varTypes).ToArray();

    public static readonly string[] intFields = {
        "Equals",
        "MaxValue",
        "MinValue",
        "Parse",
        "TryParse",
        "ReferenceEquals",
    };

    public static readonly string[] boolFields = {
        "Equals",
        "Parse",
        "TryParse",
        "ReferenceEquals",
        "FalseString",
        "TrueString",
    };

    public static readonly string[] stringFields = {
        "Compare",
        "CompareOrdinal",
        "Concat",
        "Copy",
        "Empty",
        "Equals",
        "Format",
        "Intern",
        "IsInterned",
        "IsNullOrEmpty",
        "IsNullOrWhiteSpace",
        "Join",
        "ReferenceEquals"
    };

    public static readonly string[] arrayFields = {
        "SyncRoot",
        "LongLength",
        "IsSynchronized",
        "IsReadOnly",
        "IsFixedSize",
        "Rank"
    };
}

public static class Extensions{
    public static void Shuffle<T>(this IList<T> list){  
        System.Random rnd = new System.Random();
        int n = list.Count;  
            while (n > 1) {  
                n--;  
                int k = rnd.Next(n + 1);  
                T value = list[k];  
                list[k] = list[n];  
                list[n] = value;  
            }  
    }

    public static List<GameObject> GetChildren(this GameObject obj){
        List<GameObject> children = new List<GameObject>();
        foreach(Transform t in obj.transform){
            children.Add(t.gameObject);
        }
        return children;
    }
}

public enum MathExpression{
    Add,
    Subtract,
    Multiply,
    Divide,
    Modulo,
    StartGroup,
    EndGroup,
    Value
}

public enum ArgTypes{
    integer,
    intArray,
    floatpoint,
    doubleArray,
    truefalse,
    boolArray,
    character,
    charArray,
    textstring,
    stringArray
}