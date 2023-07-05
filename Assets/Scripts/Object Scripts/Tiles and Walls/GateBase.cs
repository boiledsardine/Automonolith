using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateBase : MonoBehaviour{
    private Vector3 openPos, closedPos;
    [SerializeField] float targetPosY;

    protected void Start(){
        closedPos = transform.position;
        openPos = closedPos + new Vector3(0f, targetPosY, 0f);
    }

    protected IEnumerator openGate(){
        gameObject.layer = 0;
        float timeElapsed = 0;
        while(timeElapsed < Globals.Instance.timeToMove){
            transform.position = Vector3.Lerp(closedPos, openPos,
            (timeElapsed / Globals.Instance.timeToMove));
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = openPos;
    }

    protected IEnumerator closeGate(){
        float timeElapsed = 0;
        while(timeElapsed < Globals.Instance.timeToMove){
            transform.position = Vector3.Lerp(openPos, closedPos,
            (timeElapsed / Globals.Instance.timeToMove));
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = closedPos;
    }
}
