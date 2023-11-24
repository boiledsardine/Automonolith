using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotKiller : MonoBehaviour, IActivate{
    GameObject playerCharacter;

    public void activate(){
        Compiler.Instance.terminateExecution();

        playerCharacter.GetComponent<Animator>().SetBool("ded", true);
        Invoke("DestroyBot", 1f);
    }

    public void deactivate(){
        //do nothing
    }

    void Start(){
        playerCharacter = GameObject.Find("PlayerCharacter");
    }

    void DestroyBot(){
        Destroy(playerCharacter);
    }
}
