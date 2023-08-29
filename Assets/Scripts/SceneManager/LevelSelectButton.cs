using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectButton : MonoBehaviour{
    public int levelIndex;
    public int sceneToLoad;

    public void OnClick(){
        LevelSaveLoad.Instance.indexHolder = levelIndex;

        LevelSelectManager.Instance.loadLevelDetails(levelIndex);
        LevelSelectManager.Instance.sceneToLoad = sceneToLoad;
    }
}
