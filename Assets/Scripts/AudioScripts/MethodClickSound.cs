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
        float globalVolume = GlobalSettings.Instance.sfxVolume;
        float multiplier = AudioPicker.Instance.buttonVolume;
        source.volume = globalVolume * multiplier;

        source.clip = AudioPicker.Instance.methodRollover;
        source.Play();
    }

    public void OnPointerClick(PointerEventData eventData){
        float globalVolume = GlobalSettings.Instance.sfxVolume;
        float multiplier = AudioPicker.Instance.buttonVolume;
        source.volume = globalVolume * multiplier;
        
        source.clip = AudioPicker.Instance.methodClick;
        source.Play();
    }    
}
