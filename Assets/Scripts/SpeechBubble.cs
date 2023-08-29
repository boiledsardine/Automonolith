using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeechBubble : MonoBehaviour{
    private GameObject playerCharacter;
    [SerializeField] public float yPos = 80;
    public float destroyTime = 5f;
    private Vector3 followPos;
    public FollowMode followMode;
    public GameObject boundObject;
    
    private void Start(){
        playerCharacter = GameObject.Find("PlayerCharacter");

        if(followMode == FollowMode.FollowPlayer){
            followPos = playerCharacter.transform.position;
        } else if(followMode == FollowMode.FollowBoundObject){
            followPos = boundObject.transform.position;
        }

        transform.position = new Vector3(followPos.x, followPos.y + yPos, followPos.z);
        Invoke("selfDestruct", destroyTime);
    }
    
    private void Update(){
        if(followMode == FollowMode.FollowPlayer){
            followPos = playerCharacter.transform.position;
            transform.position = new Vector3(followPos.x, followPos.y + yPos, followPos.z);
        } else if(followMode == FollowMode.FollowBoundObject){
            followPos = boundObject.transform.position;
            transform.position = new Vector3(followPos.x, followPos.y + yPos, followPos.z);
        }
    }

    //billboarding
    private void LateUpdate(){
        transform.forward = Camera.main.transform.forward;
    }

    private void selfDestruct(){
        Destroy(gameObject);
    }

    public enum FollowMode{
        FollowPlayer,
        FollowBoundObject
    }
}
