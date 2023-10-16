using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommandControl {
    
public class Movement : MonoBehaviour, IMovement {
    private Vector3 originPos, targetPos;
    private Quaternion originRot, targetRot;
    private int recurCount = 0;
    private char direction;
    
    [SerializeField] private char _facing;
    
    private Environment envirScript;
    private Interaction interScript;

    Animator anim;

    private void Awake(){
        envirScript = gameObject.GetComponent<Environment>();
        interScript = gameObject.GetComponent<Interaction>();
        setFacing();

        anim = GetComponent<Animator>();
    }

    //getters and setters
    public char facing{
        get { return _facing; }
        set { _facing = value; }
    }

    private void setFacing(){
        if(transform.localEulerAngles.y == 0){
            facing = 'S';
        }
        if(transform.localEulerAngles.y == 90){
            facing = 'W';
        }
        if(transform.localEulerAngles.y == 180){
            facing = 'N';
        }
        if(transform.localEulerAngles.y == 270){
            facing = 'E';
        }
    }

    //movement methods
    public IEnumerator moveUp(){
        if(facing != 'N' && !interScript.isHoldingBig){
            yield return StartCoroutine(Rotate(Quaternion.Euler(0, 180, 0)));
        }
        recurCount = 1;
        direction = 'N';
        yield return StartCoroutine(Move(new Vector3(0f, 0f, 100f)));
    }

    public IEnumerator moveDown(){
        if(facing != 'S' && !interScript.isHoldingBig){
            yield return StartCoroutine(Rotate(Quaternion.Euler(0, 0, 0)));
        }
        recurCount = 1;
        direction = 'S';
        yield return StartCoroutine(Move(new Vector3(0f, 0f, -100f)));
    }

    public IEnumerator moveLeft(){
        if(facing != 'W' && !interScript.isHoldingBig){
            yield return StartCoroutine(Rotate(Quaternion.Euler(0, 90, 0)));
        }
        recurCount = 1;
        direction = 'W';
        yield return StartCoroutine(Move(new Vector3(-100f, 0f, 0f)));
    }

    public IEnumerator moveRight(){
        if(facing != 'E' && !interScript.isHoldingBig){
            yield return StartCoroutine(Rotate(Quaternion.Euler(0, 270, 0)));
        }
        recurCount = 1;
        direction = 'E';
        yield return StartCoroutine(Move(new Vector3(100f, 0f, 0f)));
    }

    //parameterized movement methods
    public IEnumerator moveUp(int num){
        if(facing != 'N' && !interScript.isHoldingBig){
            yield return StartCoroutine(Rotate(Quaternion.Euler(0, 180, 0)));
        }
        recurCount = num;
        direction = 'N';
        yield return StartCoroutine(Move(new Vector3(0f, 0f, 100f)));
    }

    public IEnumerator moveDown(int num){
        if(facing != 'S' && !interScript.isHoldingBig){
            yield return StartCoroutine(Rotate(Quaternion.Euler(0, 0, 0)));
        }
        recurCount = num;
        direction = 'S';
        yield return StartCoroutine(Move(new Vector3(0f, 0f, -100f)));
    }

    public IEnumerator moveLeft(int num){
        if(facing != 'W' && !interScript.isHoldingBig){
            yield return StartCoroutine(Rotate(Quaternion.Euler(0, 90, 0)));
        }
        recurCount = num;
        direction = 'W';
        yield return StartCoroutine(Move(new Vector3(-100f, 0f, 0f)));
    }

    public IEnumerator moveRight(int num){
        if(facing != 'E' && !interScript.isHoldingBig){
            yield return StartCoroutine(Rotate(Quaternion.Euler(0, 270, 0)));
        }
        recurCount = num;
        direction = 'E';
        yield return StartCoroutine(Move(new Vector3(100f, 0f, 0f)));
    }

    //rotation methods
    public IEnumerator turnUp(){
        if(!interScript.isHoldingBig){
            yield return StartCoroutine(Rotate(Quaternion.Euler(0, 180, 0)));
        }
    }

    public IEnumerator turnDown(){
        if(!interScript.isHoldingBig){
            yield return StartCoroutine(Rotate(Quaternion.Euler(0, 0, 0)));
        }
    }

    public IEnumerator turnLeft(){
        if(!interScript.isHoldingBig){
            yield return StartCoroutine(Rotate(Quaternion.Euler(0, 90, 0)));
        }
    }
    
    public IEnumerator turnRight(){
        if(!interScript.isHoldingBig){
            yield return StartCoroutine(Rotate(Quaternion.Euler(0, 270, 0)));
        }
    }

    //waiting
    public IEnumerator wait(){
        yield return new WaitForSeconds(Globals.Instance.timePerStep);
    }

    public IEnumerator moveTo(string tileName){
        yield return new WaitForSeconds(Globals.Instance.timePerStep * 2);
        TileBase targetTile = null;

        //force global update of tile connections
        TileBase[] tiles = FindObjectsOfType<TileBase>();
        foreach(TileBase tile in tiles){
            tile.GetTileNeighbors();
        }

        try{
            targetTile = GameObject.Find(tileName).GetComponent<TileBase>();
        } catch {
            Compiler.Instance.addErr(string.Format("Cannot find any tile named {0}! Make sure it is in uppercase!", tileName));
            Compiler.Instance.errorChecker.writeError();
            Compiler.Instance.killTimer();
        }

        if(targetTile != null){
            List<TileBase> tileList = new List<TileBase>();
            try {
                tileList = Pathfinder.Instance.FindPath(envirScript.currentTile, targetTile);
            } catch {
                Compiler.Instance.addErr(string.Format("Pathfinding functionality currently not available!"));
                Compiler.Instance.errorChecker.writeError();
                Compiler.Instance.killTimer();
            }

            //go through the tileList figuring out the adjacency of each tile
            //then queue em up
            if(tileList.Count > 0){
                List<char> directList = Pathfinder.Instance.GetDirections(tileList);
                //Pathfinder.Instance.ColorTiles(tileList);
                yield return StartCoroutine(MoveAlongPath(directList, 0));
            } else {
                Compiler.Instance.addErr(string.Format("Cannot find a path to {0}!", tileName));
                Compiler.Instance.errorChecker.writeError();
                Compiler.Instance.killTimer();
            }
        }
    }

    private IEnumerator MoveAlongPath(List<char> directList, int index){
        switch(directList[index]){
            case 'n':
                yield return StartCoroutine(moveUp());
            break;
            case 's':
                yield return StartCoroutine(moveDown());
            break;
            case 'e':
                yield return StartCoroutine(moveRight());
            break;
            case 'w':
                yield return StartCoroutine(moveLeft());
            break;
        }

        if(index != directList.Count - 1){
            yield return MoveAlongPath(directList, index + 1);
        }
    }
    
    //Lerp'd coroutines        
    bool lastWalkLeft = false;
    public IEnumerator Move(Vector3 moveDir){
        yield return new WaitForSeconds(Globals.Instance.timePerStep);
        anim.SetBool("walk1", !lastWalkLeft);
        anim.SetBool("walk2", lastWalkLeft);
        if(recurCount > 0){
            if(envirScript.neighborIsValid(direction) && !envirScript.checkForWalls(direction)){                
                //moves any held object
                interScript.moveObject(direction);
                
                //moves the player
                float timeElapsed = 0;
                originPos = transform.position;
                targetPos = originPos + moveDir;

                while(timeElapsed < Globals.Instance.timeToMove){
                    transform.position = Vector3.Lerp(originPos, targetPos,
                    (timeElapsed / Globals.Instance.timeToMove));
                    timeElapsed += Time.deltaTime;
                    yield return null;
                }         
                transform.position = targetPos;
                setFacing();

                //tail recursion
                recurCount--;
                
                lastWalkLeft = !lastWalkLeft;
                yield return StartCoroutine(Move(moveDir));
            } else {
                Compiler.Instance.addErr(string.Format("G4wain cannot move to the specified direction!", Compiler.Instance.currentIndex + 1));
                Debug.LogAssertion("Invalid direction");
                Compiler.Instance.errorChecker.writeError();
                Compiler.Instance.killTimer();
            }  
        }
        anim.SetBool("walk1", false);
        anim.SetBool("walk2", false);
    }

    public IEnumerator Rotate(Quaternion rotDir){
        yield return new WaitForSeconds(Globals.Instance.timePerStep);
        
        float timeElapsed = 0;
        originRot = transform.rotation;
        targetRot = rotDir;

        while(timeElapsed < Globals.Instance.timeToRotate){
            transform.rotation = Quaternion.Lerp(originRot, targetRot,
            (timeElapsed / Globals.Instance.timeToRotate));
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        transform.rotation = targetRot;
        setFacing();
    }
}

}