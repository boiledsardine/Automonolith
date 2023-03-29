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
                }
                //Debug.Log("Now holding " + heldObject.name);
            } else {
                Debug.Log("Object is not movable!");
            }
        } else if(isHolding) {
            Debug.Log("Already holding something!");
        } else {
            Debug.Log("Nothing to hold!");
        }
        yield return new WaitForSeconds(Globals.Instance.timePerStep);
    }

    public IEnumerator drop(){
        drop_NoExecute();
        yield return new WaitForSeconds(Globals.Instance.timePerStep);
    }

    public void drop_NoExecute(){
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
    }

    public IEnumerator interact(){
        yield return new WaitForSeconds(Globals.Instance.timePerStep / 2);
        Vector3 fwd = transform.TransformDirection(Vector3.back);
        float distance = Globals.Instance.distancePerTile;
        RaycastHit hit;

        if(Physics.Raycast(transform.position, fwd, out hit, distance)){
            GameObject hitObject = hit.transform.gameObject;
            if(hitObject.tag == "Interactable"){
                IActivate activation = hitObject.GetComponent<IActivate>();
                activation.activate();
                //Debug.Log("Interacted with " + hitObject.name);
            } else {
                Debug.Log(hitObject.name + " is not interactable");
            }
        }
        yield return new WaitForSeconds(Globals.Instance.timePerStep / 2);
    }

    public string read(){
        //yield return new WaitForSeconds(Globals.Instance.timePerStep);
        Vector3 fwd = transform.TransformDirection(Vector3.back);
        float distance = Globals.Instance.distancePerObject;
        RaycastHit hit;
        string readString = null;
        if(Physics.Raycast(transform.position, fwd, out hit, distance)){
            if(hit.transform.gameObject.tag == "WallPanel"){
                WallPanel panel = hit.transform.gameObject.GetComponent<WallPanel>();
                readString = panel.storedText;
                //Debug.Log("Read: " + panel.storedLine);
                //Bot.readLine = panel.storedLine;
            } else {
                Debug.Log("Object is not a readable panel!");
            }
        }
        return readString;
    }

    public IEnumerator write(string input){
        yield return new WaitForSeconds(Globals.Instance.timePerStep);
        Vector3 fwd = transform.TransformDirection(Vector3.back);
        float distance = Globals.Instance.distancePerObject;
        RaycastHit hit;
        if(Physics.Raycast(transform.position, fwd, out hit, distance)){
            if(hit.transform.gameObject.tag == "WallPanel"){
                WallPanel panel = hit.transform.gameObject.GetComponent<WallPanel>();
                panel.storedText = input;
                Debug.Log("Successfully wrote \"" + input + "\"");
                panel.checkPassword();
            } else {
                Debug.Log("Object is not a writable panel!");
            }
        }
    }

    public IEnumerator say(string input){
        yield return new WaitForSeconds(Globals.Instance.timePerStep);
        Debug.LogWarning("Gawain says: " + input);
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