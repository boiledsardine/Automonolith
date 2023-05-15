using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelectManager : MonoBehaviour{
    public static LevelSelectManager Instance;
    public int sceneToLoad;
    public Canvas loadCanvas;
    public Slider loadBar;
    public TMPro.TMP_Text levelName, levelDesc;
    public RawImage star1, star2, star3;
    public Texture starActive, starInactive;
    public LevelDetails levelDetailsObject;
    private List<LevelDetail> levelDetails;
    public Button[] buttonArr;

    void Awake(){
        if(Instance == null){
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    void Start(){
        levelDetails = levelDetailsObject.levelDetails;
        
        for(int i = 0; i < buttonArr.Length; i++){
            if(LevelSaveLoad.Instance.savedLevels[i].isUnlocked){
                buttonArr[i].interactable = true; 
            }
        }
    }

    public void loadLevelDetails(int levelIndex){
        levelName.text = levelDetails[levelIndex].levelName;
        levelDesc.text = levelDetails[levelIndex].levelDesc;

        if(LevelSaveLoad.Instance.savedLevels[levelIndex].star1){
            star1.texture = starActive;
        } else {
            star1.texture = starInactive;
        }

        if(LevelSaveLoad.Instance.savedLevels[levelIndex].star2){
            star2.texture = starActive;
        } else {
            star2.texture = starInactive;
        }

        if(LevelSaveLoad.Instance.savedLevels[levelIndex].star3){
            star3.texture = starActive;
        } else {
            star3.texture = starInactive;
        }
    }

    public void loadScene(){
        loadCanvas.gameObject.SetActive(true);
        StartCoroutine(loadAsync(sceneToLoad));
    }

    public IEnumerator loadAsync(int sceneIndex){
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneIndex);

        while(!loadOp.isDone){
            float progress = Mathf.Clamp01(loadOp.progress / 0.9f);
            loadBar.value = progress;
            yield return null;
        }
    }
}