using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommandControl{

public class Interaction : MonoBehaviour {
    private GameObject _heldObject;
    private ObjectMovement objMove;
    private bool _isHolding;
    private bool _isHoldingBig;
    private bool _isHoldingSmall;
    private Vector3 originPos, targetPos;

    private Environment envirScript;

    private void Awake(){
        envirScript = gameObject.GetComponent<Environment>();
    }

    public GameObject heldObject{
        get { return _heldObject; }
        set { _heldObject = value; }
    }

    public bool isHolding{
        get { return _isHolding; }
        set { _isHolding = value; }
    }

    public bool isHoldingBig{
        get { return _isHoldingBig; }
        set { _isHoldingBig = value; }
    }

    public bool isHoldingSmall{
        get { return _isHoldingSmall; }
        set { _isHoldingSmall = value; }
    }
    
    public IEnumerator hold(){
        if(envirScript.tileFront.isOccupied && !isHolding){
            heldObject = envirScript.tileFront.occupant;
            if(heldObject.GetComponent<ObjectBase>().isMovable){
                heldObject.GetComponent<ObjectBase>().isHeld = true;
                isHolding = true;
                if(heldObject.tag == "Big Object"){
                    objMove = heldObject.GetComponent<ObjectMovement>();
                    isHoldingBig = true;
                }
                if(heldObject.tag == "Small Object"){
                    isHoldingSmall = true;
                    Debug.Log("isHoldingSmall: " + isHoldingSmall);
                }
                Debug.Log("Now holding " + heldObject.name);
            } else {
                Debug.Log("Object is not movable!");
            }
        } else if(isHolding) {
            Debug.Log("Already holding something!");
        } else {
            Debug.Log("Nothing to hold!");
        }
        yield return new WaitForSeconds(Globals.Instance.timePerStep);
        Bot.Instance.execute();
    }

    public IEnumerator release(){
        if(isHoldingSmall){
            heldObject.GetComponent<Positioning>().release();
        }
        if(isHolding){
            heldObject.GetComponent<ObjectBase>().isHeld = false;
            heldObject = null;
            objMove = null;
            isHolding = false;
            isHoldingBig = false;
            isHoldingSmall = false;
        }
        yield return new WaitForSeconds(Globals.Instance.timePerStep);
        Bot.Instance.execute();
    }

    public void moveObject(char direction){
        if(objMove != null){
            objMove.dir = direction;
            switch(direction){
                case 'N':
                    StartCoroutine(objMove.moveUp());
                    break;
                case 'S':
                    StartCoroutine(objMove.moveDown());
                    break;
                case 'W':
                    StartCoroutine(objMove.moveLeft());
                    break;
                case 'E':
                    StartCoroutine(objMove.moveRight());
                    break;
                default:
                    break;
            }
        }
    }
}

}