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
        Debug.LogWarning("LEVEL FINISHED!");
        Destroy(playerCharacter);
        Compiler.Instance.terminateExecution();
    }

    public void deactivate(){
        throw new System.NotImplementedException();
    }
}
