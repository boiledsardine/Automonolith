using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharExpression{
    List<char> characters;
}

public class CharValue{
    Dictionary<string, VariableInfo> allVars;
    Dictionary<string, char> charVars;

    public List<char> charas;

    public CharValue(string line){
        allVars = Compiler.Instance.allVars;
        charVars = Compiler.Instance.charVars;
        
        charas = new List<char>();
        
        
    }

}