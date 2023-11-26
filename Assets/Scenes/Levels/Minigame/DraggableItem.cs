using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DraggableItem : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler{
    public Image image;
    
    //original parent object
    public Transform ogParent;
    //original location
    public Vector3 ogPosition;    

    void Start(){
        image = GetComponent<Image>();
        transform.position = transform.parent.transform.position;
    }

    public void OnPointerDown(PointerEventData eventData){
        PlayClickSound();
    }

    public void OnBeginDrag(PointerEventData eventData){
        ogPosition = transform.position;

        ogParent = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();

        image.raycastTarget = false;

        SendMessage("ClearSlot");
    }

    public void OnDrag(PointerEventData eventData){
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData){
        transform.SetParent(ogParent);
        transform.position = ogPosition;

        image.raycastTarget = true;

        SendMessage("CheckSlot");
    }

    void PlayClickSound(){
        AudioSource source = MinigameManager.Instance.GetComponent<AudioSource>();
        System.Random rnd = new System.Random();
        int maxIndex = AudioPicker.Instance.boxClick.Length;

        source.outputAudioMixerGroup = AudioPicker.Instance.minigameMixer;

        source.clip = AudioPicker.Instance.boxClick[rnd.Next(maxIndex)];
        source.Play();
    }
}