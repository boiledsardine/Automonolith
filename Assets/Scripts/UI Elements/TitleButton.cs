using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TitleButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler{
    AudioSource source;
    TMPro.TMP_Text arrow;
    public bool enableSound = true;
    void Awake(){
        source = GetComponent<AudioSource>();
        arrow = transform.GetChild(0).GetComponent<TMPro.TMP_Text>();
    }

    public void OnPointerClick(PointerEventData eventData){
        if(!GetComponent<Button>().interactable){
            return;
        }
        
        //PlayClickSound();
    }

    public void OnPointerEnter(PointerEventData eventData){
        if(!GetComponent<Button>().interactable){
            return;
        }

        arrow.gameObject.SetActive(true);
        PlayRolloverSound();
    }

    public void OnPointerExit(PointerEventData eventData){
        arrow.gameObject.SetActive(false);
    }

    void PlayRolloverSound(){
        if(!enableSound){
            return;
        }

        source.outputAudioMixerGroup = AudioPicker.Instance.buttonMixer;

        source.clip = AudioPicker.Instance.titleRollover;
        source.Play();
    }

    void PlayClickSound(){
        if(!enableSound){
            return;
        }

        source.outputAudioMixerGroup = AudioPicker.Instance.buttonMixer;

        source.clip = AudioPicker.Instance.titleSelect;
        source.Play();
    }
}
