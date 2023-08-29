using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LevelBase : MonoBehaviour{
    public abstract bool PriObj1();
    public abstract bool SecObj1();
    public abstract bool SecObj2();
}

/*
example code for a Gem Checker:
    GemPickup[] activeGems = FindObjectsOfType<GemPickup>();
    return activeGems.Length == 0;

example code for an Exit Checker:
    exitTouched //bool toggles if exit is touched or button is reached
    return exitTouched;

example code for a Line Counter:
    int lineThreshold;
    return Compiler.Instance.lineCount <= lineThreshold;

example code for a Dictionary Checker:
    int expectedVarCount;
    return Compiler.Instance.allVars = expectedVarCount + Compiler.Instance.reservedVars;
*/