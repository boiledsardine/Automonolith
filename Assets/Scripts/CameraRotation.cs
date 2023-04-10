using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraRotation : MonoBehaviour{
    private Vector2 delta;
    private bool isMoving;
    private bool isRotating;
    private float xRotation;

    [SerializeField] private float moveSpeed = 10.0f;
    [SerializeField] private float rotateSpeed = 0.5f;

    private void Awake(){
        xRotation = transform.rotation.eulerAngles.x;
    }

    public void OnLook(InputAction.CallbackContext context){
        delta = context.ReadValue<Vector2>();
    }

    public void OnMove(InputAction.CallbackContext context){
        isMoving = context.started || context.performed;
    }

    public void OnRotate(InputAction.CallbackContext context){
        isRotating = context.started || context.performed;
    }

    private void LateUpdate(){        
        if(isMoving){
            var position = transform.right * (delta.x * -moveSpeed);
            position += transform.up * (delta.y * -moveSpeed);
            transform.position += position * Time.deltaTime;
        }

        if(isRotating){
            transform.Rotate(new Vector3(xRotation, -delta.x * rotateSpeed, 0));
            transform.rotation = Quaternion.Euler(xRotation, transform.rotation.eulerAngles.y, 0);
        }
    }
}