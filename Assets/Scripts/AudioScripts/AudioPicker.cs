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
    public AudioMixerGroup buttonMixer, swooshMixer, footstepMixer, minigameMixer, typingMixer, talkingMixer;
    
    public float buttonVolume, menuSwooshVolume, footstepVolume, minigameVolume, typingVolume, talkingVolume;

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
