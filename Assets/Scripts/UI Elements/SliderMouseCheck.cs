using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;

public class SliderMouseCheck : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler{
    AudioSource source;
    public SliderType sliderType;
    public bool isDragging = false;

    void Awake(){
        source = GetComponent<AudioSource>();
    }

    public void OnBeginDrag(PointerEventData eventData){
        isDragging = true;
    }

    public void OnDrag(PointerEventData eventData){
        if(sliderType == SliderType.sfx){
            PlayTestSound(AudioPicker.Instance.sfxTestSound, AudioPicker.Instance.sfxMaster);
        } else {
            PlayTestSound(AudioPicker.Instance.bgmTestSound, AudioPicker.Instance.bgmMaster);
        }
    }

    public void OnEndDrag(PointerEventData eventData){
        isDragging = false;
    }

    void PlayTestSound(AudioClip clip, AudioMixerGroup mixerGroup){
        source.outputAudioMixerGroup = mixerGroup;
        source.clip = clip;
        
        if(!source.isPlaying){
            source.Play();
        }
    }

    public enum SliderType{
        sfx, bgm
    }
}
