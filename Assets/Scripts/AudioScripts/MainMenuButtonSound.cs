using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenuButtonSound : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler{
    AudioSource source;    

    void Awake(){
        source = GetComponent<AudioSource>();
    }

    public void OnPointerEnter(PointerEventData eventData){
        source.clip = AudioPicker.Instance.levelRollover;

        source.outputAudioMixerGroup = AudioPicker.Instance.buttonMixer;
        
        source.Play();
    }

    public void OnPointerClick(PointerEventData eventData){
        source.outputAudioMixerGroup = AudioPicker.Instance.buttonMixer;
        
        source.clip = AudioPicker.Instance.levelSelect;
        source.Play();
    }
}
