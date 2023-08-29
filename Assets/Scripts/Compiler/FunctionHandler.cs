using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
        "moveTo",

        "hold",
        "drop",
        "interact",
        "say",
        "dig",

        "write",
        
        "read",
        "readInt",
        "readBool",
        "ReadStringArr",
        "ReadIntArr",
        "ReadBoolArr",
        "CheckCube",

        "readChar",
        "readCharArr",
        "readDouble",
        "readDoubleArr",
    };

    //add entry every time a new non-void function is added
    public static readonly string[] nonVoids = {
        "read",
        "readInt",
        "readBool",
        "look",
        "CheckCube",
        "ReadStringArr",
        "ReadIntArr",
        "ReadBoolArr",
    };

    //[SerializeField] private TMPro.TMP_Text stepCount;
    private int steps = 0;

    Dictionary<string, VariableInfo> allVars;
    Dictionary<string, int> intVars;
    Dictionary<string, string> strVars;
    int argsCount;
    string[] passedArgs;
    string[] sections;
    List<List<ArgTypes>> argTypes;

    Movement playerMove;
    Interaction playerAct;
    ErrorChecker errorChecker;

    public bool hasError = false;
    private int lineIndex;

    public string[] getPassedArgs{
        get { return passedArgs; }
    }

    public void initializeHandler(string functionLine, int lineIndex){
        steps = 0;

        this.allVars = Compiler.Instance.allVars;
        this.intVars = Compiler.Instance.intVars;
        this.strVars = Compiler.Instance.strVars;
        this.lineIndex = lineIndex;
        sections = functionLine.Split(' ');

        playerMove = GameObject.Find("PlayerCharacter").GetComponent<Movement>();
        playerAct = GameObject.Find("PlayerCharacter").GetComponent<Interaction>();

        FunctionProcessor proc = new FunctionProcessor(functionLine);
        argsCount = proc.argsCount;
        passedArgs = proc.passedArgs;
        argTypes = proc.argTypes;

        errorChecker = gameObject.GetComponent<ErrorChecker>();
        hasError = checkArgErrors();
    }

    //TODO: make function that checks for argument errors
    //checked at firstpass, no need to add writeError
    public bool checkArgErrors(){
        switch(sections[2]){
            case "moveUp": case "moveDown": case "moveLeft": case "moveRight":
                if(argsCount > 1){
                    addErr(string.Format("Line {0}: {1}() only takes either zero or one argument!", lineIndex, sections[2]));
                    return true;
                }
                if(argsCount == 1 && !(argTypes[0][0] == ArgTypes.integer)){
                    Compiler.Instance.addErr(string.Format("Line {0}: {1}() takes only integer arguments!", lineIndex, sections[2]));
                    return true;
                }
            return false;
            case "turnUp": case "turnDown": case "turnLeft": case "turnRight":
                if(argsCount > 0){
                    addErr(string.Format("Line {0}: {1}() doesn't take arguments!", lineIndex, sections[2]));
                    return true;
                }
            return false;
            case "hold": case "drop": case "interact": case "dig":
            case "read": case "readInt": case "readBool": case "readChar": case "readDouble":
            case "ReadIntArr": case "ReadStringArr": case "ReadBoolArr": case "ReadCharArr": case "ReadDoubleArr":
                if(argsCount > 0){
                    addErr(string.Format("Line {0}: {1}() doesn't take arguments!", lineIndex, sections[2]));
                    return true;
                }
            return false;
            case "say": case "moveTo": case "CheckCube":
                if(argsCount < 1){
                    Compiler.Instance.addErr(string.Format("Line {0}: {1}() needs one argument!", lineIndex, sections[2]));
                    return true;
                } else if(argsCount > 1){
                    Compiler.Instance.addErr(string.Format("Line {0}: {1}() takes only one argument!", lineIndex, sections[2]));
                    return true;
                }
                if(argsCount == 1 && !(argTypes[0][0] == ArgTypes.integer || argTypes[0][0] == ArgTypes.truefalse || argTypes[0][0] == ArgTypes.textstring
                    || argTypes[0][0] == ArgTypes.character || argTypes[0][0] == ArgTypes.floatpoint)){
                    Compiler.Instance.addErr(string.Format("Line {0}: {1}() takes only integer, boolean, or string arguments!", lineIndex, sections[2]));
                    return true;
                }
            return false;
            case "write":
                if(argsCount != 1){
                    addErr(string.Format("Line {0}: {1}() needs one argument!", lineIndex, sections[2]));
                    return true;
                }
                if(argsCount == 1  && !(checkArgTypes(0, ArgTypes.textstring) || checkArgTypes(0, ArgTypes.integer))){
                    addErr(string.Format("Line {0}: {1}() takes only string or int arguments!", lineIndex, sections[2]));
                    return true;
                }
            return false;
            default:
                Debug.LogAssertion("oopsie");
                addErr(string.Format("Line {0}: oopsie", lineIndex, sections[2]));
            return true;
        }
    }

    private bool checkArgTypes(int index, ArgTypes argType){
        foreach(ArgTypes arg in argTypes[index]){
            if(arg != argType){
                return false;
            }
        }
        return true;
    }

    private bool checkArgAssigned(){
        Debug.Log("Checking arg assignments");
        foreach(string s in passedArgs){
            if(allVars.ContainsKey(s) && !allVars[s].isSet){
                Compiler.Instance.addErr(string.Format("Line {0}: cannot use variable {1} - it is unassigned!",
                Compiler.Instance.currentIndex + 1, s));
                return true;
            }
        }
        return false;
    }

    private void addErr(string msg){
        string[] errorMsg = {msg};
        errorChecker.errorConvo.dialogueBlocks.Add(new Dialogue(errorMsg, "ERROR", 'R', true, errorChecker.errorSprite));
    }

    public IEnumerator runFunction(){
        if(hasError){
            Debug.Log("oopsie");
            StopCoroutine(runFunction());
        }

        switch(sections[2]){
            //movement functions
            case "moveUp":
                if(argsCount == 1){
                    int argVal = new IntExpression(passedArgs[0]).evaluate();
                    yield return StartCoroutine(playerMove.moveUp(argVal));
                } else {
                    yield return StartCoroutine(playerMove.moveUp());
                }
                break;
            case "moveDown":
                if(argsCount == 1){
                    int argVal = new IntExpression(passedArgs[0]).evaluate();
                    yield return StartCoroutine(playerMove.moveDown(argVal));
                } else {
                    yield return StartCoroutine(playerMove.moveDown());
                }
                break;
            case "moveLeft":
                if(argsCount == 1){
                    int argVal = new IntExpression(passedArgs[0]).evaluate();
                    yield return StartCoroutine(playerMove.moveLeft(argVal));
                } else {
                    yield return StartCoroutine(playerMove.moveLeft());
                }
                break;
            case "moveRight":
                if(argsCount == 1){
                    int argVal = new IntExpression(passedArgs[0]).evaluate();
                    yield return StartCoroutine(playerMove.moveRight(argVal));
                } else {
                    yield return StartCoroutine(playerMove.moveRight());
                }
                break;
            case "moveTo":
                if(argsCount == 1){
                    string argVal = new StringExpression(passedArgs[0]).removeQuotations();
                    yield return StartCoroutine(playerMove.moveTo(argVal));
                }
                break;
            
            //rotation functions
            case "turnUp":
                yield return StartCoroutine(playerMove.turnUp());
                break;
            case "turnDown":
                yield return StartCoroutine(playerMove.turnDown());
                break;
            case "turnLeft":
                yield return StartCoroutine(playerMove.turnLeft());
                break;
            case "turnRight":
                yield return StartCoroutine(playerMove.turnRight());
                break;
            
            //interaction functions
            case "hold":
                yield return StartCoroutine(playerAct.hold());
                break;
            case "drop":
                yield return StartCoroutine(playerAct.drop());
                break;
            case "interact":
                yield return StartCoroutine(playerAct.interact());
                break;
            case "say":
                Debug.Log(passedArgs[0]);
                if(argTypes[0][0] == ArgTypes.truefalse){
                    string exp = BoolExpression.ProcessBool(passedArgs[0]);
                    bool argVal = BoolExpression.GetConditionResult("( " + exp + " )");
                    yield return StartCoroutine(playerAct.say(argVal.ToString()));
                } else if(argTypes[0][0] == ArgTypes.textstring){
                    string argVal = new StringExpression(passedArgs[0]).removeQuotations();
                    yield return StartCoroutine(playerAct.say(argVal));
                } else if(argTypes[0][0] == ArgTypes.integer){
                    int argVal = new IntExpression(passedArgs[0]).evaluate();
                    yield return StartCoroutine(playerAct.say(argVal.ToString()));
                } else if(argTypes[0][0] == ArgTypes.character) {
                    char argVal;
                    if(Compiler.Instance.charVars.ContainsKey(passedArgs[0])){
                        argVal = Compiler.Instance.charVars[passedArgs[0]];
                    } else {
                        argVal = passedArgs[0].Trim()[1];
                    }
                    yield return StartCoroutine(playerAct.say(argVal.ToString()));
                } else if(argTypes[0][0] == ArgTypes.character) {
                    //yield return StartCoroutine(playerAct.say(argVal.ToString()));
                } else {
                    Debug.Log("what");
                }
                break;
            /*case "write":
                if(argTypes[0][0] == ArgTypes.textstring){
                    string writeArg = new StringExpression(passedArgs[0]).removeQuotations();
                    Debug.Log("Writing: " + writeArg);
                    yield return StartCoroutine(playerAct.write(writeArg));
                } else if(argTypes[0][0] == ArgTypes.integer){
                    int writeArg = new IntExpression(passedArgs[0]).evaluate();
                    Debug.Log("Writing: " + writeArg);
                    yield return StartCoroutine(playerAct.write(writeArg));
                } else {
                    Debug.Log("what");
                }
                break;*/
            case "dig":
                yield return StartCoroutine(playerAct.digTile());
            break;
            default:
                Debug.Log("oops");
                break;
        }
    }

    public void showError(){
        Debug.LogAssertion("Function args are invalid");
    }

    /*public void addStep(){
        steps++;
        stepCount.text = string.Format("Steps: {0}", steps.ToString());
    }*/
}