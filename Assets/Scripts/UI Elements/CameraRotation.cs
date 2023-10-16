using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraRotation : MonoBehaviour{
    public Vector2 delta;
    public bool isMoving;
    public bool isRotating;
    public bool isScrolling;
    private float xRotation;

    float scrollDelta;

    [SerializeField] public Vector3 defaultPosition;
    [SerializeField] private Quaternion defaultRotation;
    [SerializeField] private float moveSpeed = 10.0f;
    [SerializeField] private float rotateSpeed = 0.5f;
    [SerializeField] private CameraControlPanel panel;
    public float zoomAmount = 25;
    public float zoomUpperBound = 400;
    public float zoomLowerBound = 200;
    public float defaultZoom = 300;

    Camera mainCam;

    private void Awake(){
        xRotation = transform.rotation.eulerAngles.x;
        mainCam = transform.GetChild(0).GetComponent<Camera>();

        defaultPosition = transform.position;
    }

    private void Start(){
        ResetCameraDefaults();
    }

    public void ResetCameraDefaults(){
        ResetCameraPosition();
        ResetCameraRotation();
        ResetCameraZoom();
    }

    public void ResetCameraPosition(){
        gameObject.transform.position = defaultPosition;
    }

    public void ResetCameraRotation(){
        gameObject.transform.rotation = defaultRotation;
    }

    public void ResetCameraZoom(){
        mainCam.orthographicSize = defaultZoom;
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

    public void OnScroll(InputAction.CallbackContext context){
        scrollDelta = context.ReadValue<float>();
        if(panel.pointerNotObstructed){
            isScrolling = context.started || context.performed;   
        } else {
            isScrolling = false;
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

        if(isScrolling){
            if (scrollDelta < 0 && mainCam.orthographicSize < zoomUpperBound){
                mainCam.orthographicSize += zoomAmount;
            }
            else if (scrollDelta > 0 && mainCam.orthographicSize > zoomLowerBound){
                mainCam.orthographicSize -= zoomAmount;
            }
        }
    }
}