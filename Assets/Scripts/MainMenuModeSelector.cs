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

    void Start(){
        anim = GetComponent<Animator>();

        int lastScene = LastSceneHolder.Instance.lastScene;
        string lastSceneName = LastSceneHolder.Instance.lastSceneName;
        //from 2 to 22: regular level
        if(lastSceneName != "mg-level"){
            MainLevelsMode();

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
            MinigameMode();
        }
    }

    void OpenMinigame(){
        int lastScene = LastSceneHolder.Instance.lastScene;
        MinigameOpener.Instance.OpenMinigames(lastScene);
    }

    public void MainLevelsMode(){
        anim.SetBool("isMinigame", false);
        ResetSelectors();
    }

    public void MinigameMode(){
        anim.SetBool("isMinigame", true);
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
}
