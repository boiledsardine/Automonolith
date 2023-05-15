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

    private void Awake(){
        rectTransform = gameObject.GetComponent<RectTransform>();
    }

    public void SetText(string content, string header){
        if(string.IsNullOrEmpty(header)){
            headerText.gameObject.SetActive(false);
        } else {
            headerText.gameObject.SetActive(true);
            headerText.text = header;
        }

        contentText.text = content;

        int headerLength = headerText.text.Length;
        int contentLength = contentText.text.Length;

        layoutElement.enabled = (headerLength > charWrapLimit || contentLength > charWrapLimit) ? true : false;
    }

    private void Update(){
        Vector2 position = Input.mousePosition;
        transform.position = new Vector2(position.x + offset, position.y + offset);
    }
}