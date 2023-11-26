using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

public class OptionsMenu : MonoBehaviour{
    public TMP_Dropdown dropdown;
    public Toggle fullscreenToggle;
    public Slider bgmSlider, sfxSlider;
    public Animator panelAnim;
    AudioSource source;
    public AudioMixer mixer;

    // Start is called before the first frame update
    void Awake(){
        source = GetComponent<AudioSource>();
    }

    public void SetValues(){
        switch(GlobalSettings.Instance.resolution){
            case ScreenResMode._1280x720:
                dropdown.value = 0;
                SetScreenRes(1280, 720);
            break;
            case ScreenResMode._1366x768:
                dropdown.value = 1;
                SetScreenRes(1366, 768);
            break;
            case ScreenResMode._1600x900:
                dropdown.value = 2;
                SetScreenRes(1600, 900);
            break;
            case ScreenResMode._1920x1080:
                dropdown.value = 3;
                SetScreenRes(1920, 1080);
            break;
        }

        fullscreenToggle.isOn = GlobalSettings.Instance.isFullscreen;
        
        Debug.Log("bgm: " + GlobalSettings.Instance.bgmVolume);
        mixer.SetFloat("bgmMasterVolume", Mathf.Log10(GlobalSettings.Instance.bgmVolume) * 20);
        bgmSlider.value = GlobalSettings.Instance.bgmVolume;
        
        Debug.Log("sfx: " + GlobalSettings.Instance.bgmVolume);
        mixer.SetFloat("sfxMasterVolume", Mathf.Log10(GlobalSettings.Instance.sfxVolume) * 20);
        sfxSlider.value = GlobalSettings.Instance.sfxVolume;
    }

    void SetScreenRes(int width, int height){
        Screen.SetResolution(width, height, GlobalSettings.Instance.isFullscreen);
    }

    public void DropdownChanged(){
        Debug.Log("Changing res");
        switch(dropdown.value){
            case 0:
                GlobalSettings.Instance.resolution = ScreenResMode._1280x720;
                SetScreenRes(1280, 720);
            break;
            case 1:
                GlobalSettings.Instance.resolution = ScreenResMode._1366x768;
                SetScreenRes(1366, 768);
            break;
            case 2:
                GlobalSettings.Instance.resolution = ScreenResMode._1600x900;
                SetScreenRes(1600, 900);
            break;
            case 3:
                GlobalSettings.Instance.resolution = ScreenResMode._1920x1080;
                SetScreenRes(1920, 1080);
            break;
        }
    }

    public void SetFullScreen(){
        Debug.Log("Changing fullscreen");
        GlobalSettings.Instance.isFullscreen = fullscreenToggle.isOn;
        Screen.fullScreen = fullscreenToggle.isOn;
    }

    public void BGM_Changed(){
        if(!sfxSlider.GetComponent<SliderMouseCheck>().isDragging){
            return;
        }
        
        GlobalSettings.Instance.bgmVolume = bgmSlider.value;
        mixer.SetFloat("bgmMasterVolume", Mathf.Log10(bgmSlider.value) * 20);
    }

    public void SFX_Changed(){
        if(!sfxSlider.GetComponent<SliderMouseCheck>().isDragging){
            return;
        }

        GlobalSettings.Instance.sfxVolume = sfxSlider.value;
        mixer.SetFloat("sfxMasterVolume", Mathf.Log10(sfxSlider.value) * 20);
    }

    public void CloseOptions(bool confirm){
        if(confirm){
            GlobalSettings.Instance.SaveSettings();
        } else {
            GlobalSettings.Instance.LoadOptions();
            SetValues();
        }

        PlayCloseSound();
        panelAnim.SetBool("isOpen", false);
    }

    public void PlayOpenSound(){
        source.outputAudioMixerGroup = AudioPicker.Instance.swooshMixer;
        
        source.clip = AudioPicker.Instance.menuOpen;
        source.Play();
    }

    void PlayCloseSound(){
        source.outputAudioMixerGroup = AudioPicker.Instance.swooshMixer;
        
        source.clip = AudioPicker.Instance.menuClose;
        source.Play();
    }
}
