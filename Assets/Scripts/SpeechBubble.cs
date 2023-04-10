using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeechBubble : MonoBehaviour{
    private GameObject playerCharacter;
    [SerializeField] private float yPos = 80;
    [SerializeField] private float destroyTime = 5f;
    private Vector3 playerPos;
    
    private void Awake(){
        playerCharacter = GameObject.Find("PlayerCharacter");
        playerPos = playerCharacter.transform.position;
        transform.position = new Vector3(playerPos.x, playerPos.y + yPos, playerPos.z);
        Invoke("selfDestruct", destroyTime);
    }
    
    private void Update(){
        playerPos = playerCharacter.transform.position;
        transform.position = new Vector3(playerPos.x, playerPos.y + yPos, playerPos.z);
    }

    private void LateUpdate(){
        transform.forward = Camera.main.transform.forward;
    }

    private void selfDestruct(){
        Destroy(gameObject);
    }
}
