using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.EventSystems;

public class GameUISound : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler{
    [SerializeField] UIType uiType;
    AudioSource source;
    public bool enable = true;
    
    void Awake(){
        source = GetComponent<AudioSource>();
    }

    public void OnPointerEnter(PointerEventData eventData){
        if(!enable){
            return;
        }

        source.clip = AudioPicker.Instance.uiMouseover;

        source.outputAudioMixerGroup = AudioPicker.Instance.buttonMixer;

        source.Play();
    }
    
    public void OnPointerClick(PointerEventData eventData){
        if(!enable){
            return;
        }

        switch(uiType){
            case UIType.button:
                source.clip = AudioPicker.Instance.uiClick;
            break;
            case UIType.play:
                source.clip = AudioPicker.Instance.playClick;
            break;
            case UIType.stop:
                source.clip = AudioPicker.Instance.stopClick;
            break;
            case UIType.toggle:
                source.clip = AudioPicker.Instance.toggleClick;
            break;
            case UIType.beep:  
                source.clip = AudioPicker.Instance.beep;
            break;
            case UIType.close:
                source.clip = AudioPicker.Instance.closeClick;
            break;
            default:
                source.clip = null;
            break;
        }

        if(uiType == UIType.play || uiType == UIType.stop){
            source.outputAudioMixerGroup = AudioPicker.Instance.sfxMaster;
        } else {
            source.outputAudioMixerGroup = AudioPicker.Instance.buttonMixer;
        }

        source.Play();
    }

    public enum UIType{
        button, play, stop, toggle, beep, close, none
    }
}
