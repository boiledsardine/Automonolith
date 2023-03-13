using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMovement {
    //interface for movement functions
    
    IEnumerator moveUp();
    IEnumerator moveDown();
    IEnumerator moveLeft();
    IEnumerator moveRight();
    IEnumerator Move(Vector3 moveDir);
}