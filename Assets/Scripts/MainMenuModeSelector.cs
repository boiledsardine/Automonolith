using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuModeSelector : MonoBehaviour{
    Animator anim;
    public TMP_Text levelName, levelDesc, minigameLevelName, minigameLevelDesc;
    public RawImage star1, star2, star3, isDone;
    public Texture starInactive;
    public GameObject mainLevels, minigameLevels;
    AudioSource source;

    void Start(){
        anim = GetComponent<Animator>();

        int lastScene = LastSceneHolder.Instance.lastScene;
        string lastSceneName = LastSceneHolder.Instance.lastSceneName;

        source = GetComponent<AudioSource>();

        //from 2 to 22: regular level
        if(lastSceneName != "mg-level"){
            MainLevelsMode(false);

            //trigger minigame open - 3, 7, 12, 15, 19
            //if any change happens to these here levels
            //also change MinigameOpener's OpenMinigames to account for it
            int[] tutLevels = { 3,7,12,15,19 };
            if(tutLevels.Contains(lastScene)){
                //trigger dialogue, open the minigames, and force the manager to reload
                Invoke("OpenMinigame", 0.5f);
            }
        }
        //23: minigame level
        else {
            MinigameMode(false);
        }
    }

    void OpenMinigame(){
        int lastScene = LastSceneHolder.Instance.lastScene;
        MinigameOpener.Instance.OpenMinigames(lastScene);
    }

    public void MainLevelsMode(bool playSound){
        anim.SetBool("isMinigame", false);
        
        if(playSound){
            PlayCoreModeSound();
            TempMuteAll();
        }

        ResetSelectors();
    }

    public void MinigameMode(bool playSound){
        anim.SetBool("isMinigame", true);
        
        if(playSound){
            PlayMinigameModeSound();
            TempMuteAll();
        }

        ResetSelectors();
    }

    void ResetSelectors(){
        levelName.text = "Select a level";
        levelDesc.text = "Pick a level of the monolith.";
        star1.texture = starInactive;
        star2.texture = starInactive;
        star3.texture = starInactive;
        LevelSelectManager.Instance.sceneToLoad = 1;

        minigameLevelName.text = "Select a level";
        minigameLevelDesc.text = "Pick a task to help Merlin with.";
        isDone.texture = starInactive;
        MinigameLevelSelector.Instance.sceneToLoad = "Main Menu";
    }

    void PlayMinigameModeSound(){
        source.outputAudioMixerGroup = AudioPicker.Instance.swooshMixer;
        
        source.clip = AudioPicker.Instance.modeSwitchLeft;
        source.Play();
    }

    void PlayCoreModeSound(){
        source.outputAudioMixerGroup = AudioPicker.Instance.swooshMixer;
        
        source.clip = AudioPicker.Instance.modeSwitchRight;
        source.Play();
    }

    //potential issue: might nuke BGM source
    //add exclusion line later
    void TempMuteAll(){
        var allAudioSource = FindObjectsOfType<AudioSource>();
        foreach(AudioSource aud in allAudioSource){
            if(aud != source){
                aud.enabled = false;
            }
        }
        Invoke(nameof(UnmuteAll), 0.25f);
    }

    void UnmuteAll(){
        var allAudioSource = FindObjectsOfType<AudioSource>();
        foreach(AudioSource aud in allAudioSource){
            aud.enabled = true; 
        }
    }
}
