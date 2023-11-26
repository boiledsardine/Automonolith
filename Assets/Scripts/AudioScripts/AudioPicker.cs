using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;
using UnityEngine.Audio;

public class AudioPicker : MonoBehaviour{
    public static AudioPicker Instance { get; private set; }

    void Awake(){
        if(Instance == null){
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    void Start(){
        SetMixerVolume();
    }

    public AudioMixer mixer;
    [Space]
    public AudioMixerGroup sfxMaster;
    public AudioMixerGroup bgmMaster;
    [Space]
    public AudioMixerGroup buttonMixer;
    public AudioMixerGroup swooshMixer, footstepMixer, minigameMixer, typingMixer, talkingMixer, environmentMixer;
    [Space]
    public float buttonVolume;
    public float menuSwooshVolume, footstepVolume, minigameVolume, typingVolume, talkingVolume, environmentVolume;

    void SetMixerVolume(){
        mixer.SetFloat("buttonVolume", Mathf.Log10(buttonVolume) * 20);
        mixer.SetFloat("swooshVolume", Mathf.Log10(menuSwooshVolume) * 20);
        mixer.SetFloat("footstepVolume", Mathf.Log10(footstepVolume) * 20);
        mixer.SetFloat("minigameVolume", Mathf.Log10(minigameVolume) * 20);
        mixer.SetFloat("typingVolume", Mathf.Log10(typingVolume) * 20);
        mixer.SetFloat("talkingVolume", Mathf.Log10(talkingVolume) * 20);
        mixer.SetFloat("environmentVolume", Mathf.Log10(environmentVolume) * 20);
    }
    
    [Header("Fanfares")]
    public AudioClip successFanfare;

    [Space]
    [Header("Level UI Elements")]
    public AudioClip uiMouseover;
    public AudioClip uiClick, closeClick, toggleClick, playClick, stopClick, sidebarOpen, sidebarClose, menuOpen, menuClose, beep;
    public AudioClip methodRollover, methodClick, resetEditor;
    public AudioClip[] keyPress;

    [Space]
    [Header("G4wain noises")]
    public AudioClip botMove1;
    public AudioClip botMove2, botSay, botRead, botHold, botDrop;
    public AudioClip[] botDead;

    [Space]
    [Header("Environment sounds")]
    public AudioClip gemPickup;
    public AudioClip buttonOn, buttonOff, buttonLatch, gateOpen, gateClose, trap, acceptorFull;
    public AudioClip[] rockMove, acceptorUp;

    [Space]
    [Header("Dialogue sounds")]
    public AudioClip errorMessage;
    public AudioClip talkingSound, talkingDone;

    [Space]
    [Header("Title and Main Menu sounds")]
    public AudioClip titleRollover;
    public AudioClip levelRollover, titleSelect, levelSelect, modeSwitchLeft, modeSwitchRight, titleWindowOpen, titleWindowClose;

    [Space]
    [Header("Quiz sounds")]
    public AudioClip[] question;
    public AudioClip wrongAnswer, correctAnswer, nextLocked, failSting, passSting;

    [Space]
    [Header("Minigame sounds")]
    public AudioClip minigameSuccess;
    public AudioClip minigameError;
    public AudioClip[] boxClick, boxDrop;

    [Space]
    [Header("Persistent Clicks")]
    public AudioClip persistBeep;
    public AudioClip persistBell, persistConf;
    
    [Space]
    [Header("Test sounds")]
    public AudioClip sfxTestSound;
    public AudioClip bgmTestSound;
}
