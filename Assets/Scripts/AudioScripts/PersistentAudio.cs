using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PersistentAudio : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler{
    AudioSource source;
    public ClickType clickType;
    public MouseoverType mouseoverType;

    public void OnPointerClick(PointerEventData eventData){
        source = AudioPicker.Instance.gameObject.GetComponent<AudioSource>();

        float globalVolume = GlobalSettings.Instance.sfxVolume;
        float multiplier = AudioPicker.Instance.buttonVolume;
        source.volume = globalVolume * multiplier;

        if(gameObject.tag == "Play Button"){
            bool coreLevelInitialized = LevelSelectManager.Instance.sceneToLoad > 1;
            bool minigameInitialized = MinigameLevelSelector.Instance.sceneToLoad == "mg-level";
            
            if(coreLevelInitialized || minigameInitialized){
                PlayClick();
            } else {
                source.clip = AudioPicker.Instance.wrongAnswer;
                source.Play();
            }
        } else {
            PlayClick();
        }
    }

    public void OnPointerEnter(PointerEventData eventData){
        source = GetComponent<AudioSource>();

        float globalVolume = GlobalSettings.Instance.sfxVolume;
        float multiplier = AudioPicker.Instance.buttonVolume;
        source.volume = globalVolume * multiplier;

        PlayMouseOver();
    }

    void PlayClick(){
        switch(clickType){
            case ClickType.beep:
                source.pitch = 1;
                source.clip = AudioPicker.Instance.persistBeep;
            break;
            case ClickType.bell:
                source.pitch = 1;
                source.clip = AudioPicker.Instance.persistBell;
            break;
            case ClickType.confirm:
                source.pitch = 0.75f;
                source.clip = AudioPicker.Instance.persistConf;
            break;
            case ClickType.click:
                source.pitch = 1;
                source.clip = AudioPicker.Instance.uiClick;
            break;
            case ClickType.close:
                source.pitch = 1;
                source.clip = AudioPicker.Instance.closeClick;
            break;
            default:
            break;
        }
        source.Play();
    }

    void PlayMouseOver(){
        switch(mouseoverType){
            case MouseoverType.beep:
                source.clip = AudioPicker.Instance.beep;
            break;
            case MouseoverType.rollover:
                source.clip = AudioPicker.Instance.uiMouseover;
            break;
        }
        source.Play();
    }

    public enum ClickType{
        beep, bell, confirm, click, close
    }

    public enum MouseoverType{
        beep, rollover
    }
}
