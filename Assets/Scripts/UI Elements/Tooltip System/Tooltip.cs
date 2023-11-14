using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

[ExecuteInEditMode()]
public class Tooltip : MonoBehaviour{
    public TMP_Text headerText, contentText;
    public LayoutElement layoutElement;
    public int charWrapLimit;
    public float offset = 5f;
    private RectTransform rectTransform;
    public bool isOverUI = false;
    private Vector2 ogPivot;

    private void Awake(){
        rectTransform = gameObject.GetComponent<RectTransform>();
        ogPivot = rectTransform.pivot;
    }

    public void SetText(string content, string header){
        if(string.IsNullOrEmpty(header)){
            headerText.transform.gameObject.SetActive(false);
        } else {
            headerText.transform.gameObject.SetActive(true);
            headerText.text = header;
        }

        contentText.text = content;

        int headerLength = headerText.text.Length;
        int contentLength = contentText.text.Length;

        layoutElement.enabled = headerLength > charWrapLimit || contentLength > charWrapLimit;
    }

    private void Update(){
        Vector2 position = Input.mousePosition;

        if(isOverUI){
            float pivotX = position.x / Screen.width;
            float pivotY = position.y / Screen.height;
            rectTransform.pivot = new Vector2(pivotX, pivotY);
        } else {
            rectTransform.pivot = ogPivot;
        }

        transform.position = new Vector2(position.x + offset, position.y + offset);
    }
}