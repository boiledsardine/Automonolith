using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommandControl;

public class ExitPoint : ObjectBase, IActivate{
    private GameObject playerCharacter;

    private new void Start(){
        base.Start();
        playerCharacter = GameObject.Find("PlayerCharacter");
    }
    
    public void activate(){
        ObjectiveManager.Instance.exitTouched = true;
        ObjectiveManager.Instance.LevelComplete();
        Compiler.Instance.terminateExecution();
        Debug.Log("Compilation stopped!");
    }

    public void deactivate(){
        throw new System.NotImplementedException();
    }
}
