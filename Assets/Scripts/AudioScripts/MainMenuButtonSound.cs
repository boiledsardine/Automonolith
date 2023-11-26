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

        float globalVolume = GlobalSettings.Instance.sfxVolume;
        float multiplier = AudioPicker.Instance.buttonVolume;
        source.volume = globalVolume * multiplier;
        
        source.Play();
    }

    public void OnPointerClick(PointerEventData eventData){
        float globalVolume = GlobalSettings.Instance.sfxVolume;
        float multiplier = AudioPicker.Instance.buttonVolume;
        source.volume = globalVolume * multiplier;
        
        source.clip = AudioPicker.Instance.levelSelect;
        source.Play();
    }
}
