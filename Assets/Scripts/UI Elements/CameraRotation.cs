using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraRotation : MonoBehaviour{
    public Vector2 delta;
    public bool isMoving;
    public bool isRotating;
    private float xRotation;

    [SerializeField] private Vector3 defaultPosition;
    [SerializeField] private Quaternion defaultRotation;
    [SerializeField] private float moveSpeed = 10.0f;
    [SerializeField] private float rotateSpeed = 0.5f;
    [SerializeField] private CameraControlPanel panel;

    private void Awake(){
        xRotation = transform.rotation.eulerAngles.x;
    }

    private void Start(){
        ResetCameraDefaults();
    }

    public void ResetCameraDefaults(){
        gameObject.transform.position = defaultPosition;
        gameObject.transform.rotation = defaultRotation;
    }

    public void OnLook(InputAction.CallbackContext context){
        delta = context.ReadValue<Vector2>();
    }

    public void OnMove(InputAction.CallbackContext context){
        if(panel.pointerNotObstructed){
            isMoving = context.started || context.performed;   
        } else {
            isMoving = false;
        }
    }

    public void OnRotate(InputAction.CallbackContext context){
        if(panel.pointerNotObstructed){
            isRotating = context.started || context.performed;
        } else {
            isRotating = false;
        }
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