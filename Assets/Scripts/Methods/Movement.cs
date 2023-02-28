using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommandControl {
    
public class Movement : MonoBehaviour {
    private Vector3 originPos, targetPos;
    private float timeToMove = 0.5f;
    private int recurCount = 0;

    Vector3 vectorUp = new Vector3(0f, 0f, 100f);
    Vector3 vectorDown = new Vector3(0f, 0f, -100f);
    Vector3 vectorLeft = new Vector3(-100f, 0f, 0f);
    Vector3 vectorRight = new Vector3(100f, 0f, 0f);

    public static Movement Instance { get; private set; }

    void Awake(){
        if(Instance == null){
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    public void moveUp(){
        transform.rotation = Quaternion.Euler(0, 180, 0);
        StartCoroutine(MovePlayer(vectorUp, "north", 1));
    }

    public void moveUp(int num){
        transform.rotation = Quaternion.Euler(0, 180, 0);
        StartCoroutine(MovePlayer(vectorUp, "north", num));
    }

    public void moveDown(){
        transform.rotation = Quaternion.Euler(0, 0, 0);
        StartCoroutine(MovePlayer(vectorDown, "south", 1));
    }

    public void moveDown(int num){
        transform.rotation = Quaternion.Euler(0, 0, 0);
        StartCoroutine(MovePlayer(vectorDown, "south", num));
    }

    public void moveLeft(){
        transform.rotation = Quaternion.Euler(0, 90, 0);
        StartCoroutine(MovePlayer(vectorLeft, "west", 1));
    }

    public void moveLeft(int num){
        transform.rotation = Quaternion.Euler(0, 90, 0);
        StartCoroutine(MovePlayer(vectorLeft, "west", num));
    }

    public void moveRight(){
        transform.rotation = Quaternion.Euler(0, -90, 0);
        StartCoroutine(MovePlayer(vectorRight, "east", 1));
    }

    public void moveRight(int num){
        transform.rotation = Quaternion.Euler(0, -90, 0);
        StartCoroutine(MovePlayer(vectorRight, "east", num));
    }

    public void stand(){
        transform.rotation = Quaternion.Euler(0, -90, 0);
        StartCoroutine(MovePlayer(vectorDown, "south", 0));
    }
        
    private IEnumerator MovePlayer(Vector3 moveDir, string cardDir, int num){
        recurCount = num;

        if(recurCount > 0){
            if(Environment.Instance.neighborIsValid(cardDir)){                
                float timeElapsed = 0;
                originPos = transform.position;
                targetPos = originPos + moveDir;

                while(timeElapsed < timeToMove){
                    transform.position = Vector3.Lerp(originPos, targetPos,
                    (timeElapsed / timeToMove));
                    timeElapsed += Time.deltaTime;
                    yield return null;
                }
                transform.position = targetPos;

                recurCount--;
                StartCoroutine(MovePlayer(moveDir, cardDir, recurCount));
            } else {
                Debug.LogAssertion("Invalid direction");
                Bot.Instance.terminateExecution();
                //Bot.Instance.commandExecution("posReset");
                //Bot.Instance.commandExecution("chkTile");
            }  
        } else{
            Bot.Instance.execute();
        }
    }
}

}