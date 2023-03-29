using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommandControl;

public class FunctionHandler : MonoBehaviour{
    public static readonly string[] builtInFunctions = {
        "moveUp",
        "moveDown",
        "moveLeft",
        "moveRight",
        "turnUp",
        "turnDown",
        "turnLeft",
        "turnRight",
        "hold",
        "drop",
        "interact",
        "say",
        "write"
    };
    
    Dictionary<string, string> allVars;
    Dictionary<string, int> intVars;
    Dictionary<string, string> strVars;
    int argsCount;
    string[] passedArgs;
    string[] sections;
    List<List<ArgTypes>> argTypes;

    Movement playerMove;
    Interaction playerAct;

    public void initializeHandler(string functionLine){
        
        this.allVars = Compiler.Instance.allVars;
        this.intVars = Compiler.Instance.intVars;
        this.strVars = Compiler.Instance.strVars;
        sections = functionLine.Split(' ');

        playerMove = GameObject.Find("PlayerCharacter").GetComponent<Movement>();
        playerAct = GameObject.Find("PlayerCharacter").GetComponent<Interaction>();

        FunctionProcessor proc = new FunctionProcessor(functionLine);
        argsCount = proc.argsCount;
        passedArgs = proc.passedArgs;
        argTypes = proc.argTypes;
    }

    public IEnumerator runFunction(){
        switch(sections[0]){
            //movement functions
            case "moveUp":
                if(argsCount == 1 && argTypes[0][0] == ArgTypes.integer){
                    int argVal = new IntExpression(passedArgs[0]).evaluate();
                    yield return StartCoroutine(playerMove.moveUp(argVal));
                } else if(argsCount == 0){
                    yield return StartCoroutine(playerMove.moveUp());
                } else {
                    Debug.LogAssertion("Function args are invalid");
                }
                break;
            case "moveDown":
                if(argsCount == 1 && argTypes[0][0] == ArgTypes.integer){
                    int argVal = new IntExpression(passedArgs[0]).evaluate();
                    yield return StartCoroutine(playerMove.moveDown(argVal));
                } else if(argsCount == 0){
                    yield return StartCoroutine(playerMove.moveDown());
                } else {
                    Debug.LogAssertion("Function args are invalid");
                }
                break;
            case "moveLeft":
                if(argsCount == 1 && argTypes[0][0] == ArgTypes.integer){
                    int argVal = new IntExpression(passedArgs[0]).evaluate();
                    yield return StartCoroutine(playerMove.moveLeft(argVal));
                } else if(argsCount == 0){
                    yield return StartCoroutine(playerMove.moveLeft());
                } else {
                    Debug.LogAssertion("Function args are invalid");
                }
                break;
            case "moveRight":
                if(argsCount == 1 && argTypes[0][0] == ArgTypes.integer){
                    int argVal = new IntExpression(passedArgs[0]).evaluate();
                    yield return StartCoroutine(playerMove.moveRight(argVal));
                } else if(argsCount == 0){
                    yield return StartCoroutine(playerMove.moveRight());
                } else {
                    Debug.LogAssertion("Function args are invalid");
                }
                break;
            
            //rotation functions
            case "turnUp":
                if(argsCount == 0){
                    yield return StartCoroutine(playerMove.turnUp());
                } else {
                    Debug.LogAssertion("Function args are invalid");
                }
                break;
            case "turnDown":
                if(argsCount == 0){
                    yield return StartCoroutine(playerMove.turnDown());
                } else {
                    Debug.LogAssertion("Function args are invalid");
                }
                break;
            case "turnLeft":
                if(argsCount == 0){
                    yield return StartCoroutine(playerMove.turnLeft());
                } else {
                    Debug.LogAssertion("Function args are invalid");
                }
                break;
            case "turnRight":
                if(argsCount == 0){
                    yield return StartCoroutine(playerMove.turnRight());
                } else {
                    Debug.LogAssertion("Function args are invalid");
                }
                break;
            
            //interaction functions
            case "hold":
                if(argsCount == 0){
                    yield return StartCoroutine(playerAct.hold());
                } else {
                    Debug.LogAssertion("Function args are invalid");
                }
                break;
            case "drop":
                if(argsCount == 0){
                    yield return StartCoroutine(playerAct.drop());
                } else {
                    Debug.LogAssertion("Function args are invalid");
                }
                break;
            case "interact":
                if(argsCount == 0){
                    yield return StartCoroutine(playerAct.interact());
                } else {
                    Debug.LogAssertion("Function args are invalid");
                }
                break;
            case "say":
                if(argsCount == 1 && argTypes[0][0] == ArgTypes.textstring){
                    StringExpression textString = new StringExpression(passedArgs[0]);
                    string argVal = textString.removeQuotations();
                    StartCoroutine(playerAct.say(argVal));
                } else if(argsCount == 1 && argTypes[0][0] == ArgTypes.integer){
                    int argVal = new IntExpression(passedArgs[0]).evaluate();
                    StartCoroutine(playerAct.say(argVal.ToString()));
                }
                break;
            case "write":
                if(argsCount == 1 && argTypes[0][0] == ArgTypes.textstring){
                    StringExpression textString = new StringExpression(passedArgs[0]);
                    string argVal = textString.removeQuotations();
                    Debug.Log("Writing: " + argVal);
                    StartCoroutine(playerAct.write(argVal));
                } else {
                    Debug.LogAssertion("Function args are invalid");
                }
                break;
        }
    }
}