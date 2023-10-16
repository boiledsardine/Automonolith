using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameLevelButton : MonoBehaviour{
    public int levelIndex;
    public MinigameStageInfo stageInfo;

    public void OnClick(){
        MinigameLevelSelector.Instance.sceneToLoad = "mg-level";
        MinigameLevelSelector.Instance.loadLevelDetails(levelIndex);

        MinigameLoader.Instance.stageInfo = stageInfo;
        MinigameLoader.Instance.levelIndex = levelIndex;
    }
}
