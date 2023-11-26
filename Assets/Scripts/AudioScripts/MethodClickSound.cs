using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class MethodClickSound : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler{
    AudioSource source;
    
    void Awake(){
        source = gameObject.transform.parent.GetComponent<AudioSource>();
    }

    public void OnPointerEnter(PointerEventData eventData){
        source.outputAudioMixerGroup = AudioPicker.Instance.buttonMixer;

        source.clip = AudioPicker.Instance.methodRollover;
        source.Play();
    }

    public void OnPointerClick(PointerEventData eventData){
        source.outputAudioMixerGroup = AudioPicker.Instance.buttonMixer;
        
        source.clip = AudioPicker.Instance.methodClick;
        source.Play();
    }    
}
