using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechGate : GateBase, IActivate{
    [SerializeField] FloorButton boundButton;

    private Vector3 openPos, closedPos;
    [SerializeField] float targetPosY;
    private void Start(){
        closedPos = transform.position;
        openPos = closedPos + new Vector3(0f, targetPosY, 0f);
    }

    public void activate(){
        StartCoroutine(openGate());
    }

    public void deactivate(){
        StartCoroutine(closeGate());
    }

    public override void checkActivationFlag(){
        if(boundButton.isActivated){
            activate();
        } else {
            deactivate();
        }
    }

    private IEnumerator openGate(){
        float timeElapsed = 0;
        while(timeElapsed < Globals.Instance.timeToMove){
            transform.position = Vector3.Lerp(closedPos, openPos,
            (timeElapsed / Globals.Instance.timeToMove));
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = openPos;
    }

    private IEnumerator closeGate(){
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
