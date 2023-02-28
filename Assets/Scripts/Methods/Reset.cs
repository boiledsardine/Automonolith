using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommandControl;
using Objects;

public class Reset : MonoBehaviour{
    private Vector3 origin;

    public void Exterminatussy(){
        Debug.LogAssertion("Declaring Exterminatus");
        resetCharacterPosition();
        resetQueues();
    }
    
    private void resetQueues(){
        Bot.Instance.getCommandQueue().Clear();
        Bot.Instance.getIntQueue().Clear();
    }

    private void resetCharacterPosition(){
        GameObject.Find("PlayerCharacter").transform.position = PlayerCharacter.Instance.getOriginPos();

        //PlayerCharacter.Instance.setCurrentPos(PlayerCharacter.Instance.getOriginPos());
        //PlayerCharacter.Instance.setCurrentRot(PlayerCharacter.Instance.getOriginRot());
    }

    public static bool isResetSuccessful(){
        if(!isQueueReset()){
            return false;
        }
        if(!isCharacterReset()){
            return false;
        }
        
        return true;
    }

    private static bool isQueueReset(){
        if(Bot.Instance.getCommandQueue().Count != 0){
            return false;
        }
        if(Bot.Instance.getIntQueue().Count != 0){
            return false;
        }

        return true;
    }

    private static bool isCharacterReset(){
        if(GameObject.Find("PlayerCharacter").transform.position
        != PlayerCharacter.Instance.getOriginPos()){
            return false;
        }

        return true;
    }
}
