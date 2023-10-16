using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameOpener : MonoBehaviour{
    public static MinigameOpener Instance { get; private set; }
    public Conversation[] minigameOpenConvos;
    public Canvas dialogueCanvas;
    List<MinigameState> minigameSaves;
    public List<MinigameLevelThreshold> lvlThresh;

    void Awake(){
        if(Instance == null){
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    void Start(){
        minigameSaves = MinigameLoader.Instance.savedLevels;
    }

    //change these if ever the amount of minigame levels change
    public void OpenMinigames(int lastScene){
        switch(lastScene){
            case 3:
                if(CheckOpenLevels(lvlThresh[0].startIndex, lvlThresh[0].endIndex, 3)){
                    OpenLevels(lvlThresh[0].startIndex, lvlThresh[0].endIndex, 0);
                }
            break;
            case 7:
                if(CheckOpenLevels(lvlThresh[1].startIndex, lvlThresh[1].endIndex, 7)){
                    OpenLevels(lvlThresh[1].startIndex, lvlThresh[1].endIndex, 1);
                }
            break;
            case 12:
                if(CheckOpenLevels(lvlThresh[2].startIndex, lvlThresh[2].endIndex, 12)){
                    OpenLevels(lvlThresh[2].startIndex, lvlThresh[2].endIndex, 2);
                }
            break;
            case 15:
                if(CheckOpenLevels(lvlThresh[3].startIndex, lvlThresh[3].endIndex, 15)){
                    OpenLevels(lvlThresh[3].startIndex, lvlThresh[3].endIndex, 3);
                }
            break;
            case 19:
                if(CheckOpenLevels(lvlThresh[4].startIndex, lvlThresh[4].endIndex, 19)){
                    OpenLevels(lvlThresh[4].startIndex, lvlThresh[4].endIndex, 4);
                }
            break;
            default: break;
        }
    }

    bool CheckOpenLevels(int startIndex, int endIndex, int levelIndex){
        //check if associated level has been completed first
        if(!LevelSaveLoad.Instance.savedLevels[levelIndex - 2].star1){
            return false;
        }

        //if any of the levels iterated over are not unlocked, unlock all of them 
        for(int i = startIndex; i <= endIndex; i++){
            if(!minigameSaves[i].isUnlocked){
                return true;
            }
        }

        return false;
    }

    void OpenLevels(int startIndex, int endIndex, int convoIndex){
        for(int i = startIndex; i <= endIndex; i++){
            minigameSaves[i].isUnlocked = true;
        }
        MinigameLoader.Instance.SaveLevels(minigameSaves.ToArray());

        MinigameLevelSelector.Instance.Start();

        dialogueCanvas.gameObject.SetActive(true);
        DialogueManager.Instance.startDialogue(minigameOpenConvos[convoIndex]);
    }
}

[Serializable]
public struct MinigameLevelThreshold{
    public int startIndex, endIndex;
}