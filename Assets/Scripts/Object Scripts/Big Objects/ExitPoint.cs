using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommandControl;

public class ExitPoint : MonoBehaviour, IActivate{
    private GameObject playerCharacter;

    private void Start(){
        playerCharacter = GameObject.Find("PlayerCharacter");
    }
    
    public void activate(){
        Debug.LogWarning("LEVEL FINISHED!");
        Destroy(playerCharacter);
        Bot.Instance.terminateExecution();
    }

    public void deactivate(){
        throw new System.NotImplementedException();
    }
}
