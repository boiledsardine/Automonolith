using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MinigameLevelSelector : MonoBehaviour{
    public static MinigameLevelSelector Instance;
    public string sceneToLoad = "Main Menu";
    public Canvas loadCanvas;
    public Slider loadBar;
    public TMPro.TMP_Text levelName, levelDesc;
    public RawImage star1;
    public Texture starActive, starInactive;
    public LevelDetails levelDetailsObject;
    public MinigameStageInfo[] minigameStages;
    public GameObject buttonContainer;
    public Button[] buttonArr;
    public Button minigameButton;
    public GameObject buttonObj;

    void Awake(){
        if(Instance == null){
            Instance = this;
        } else {
            Destroy(gameObject);
        }

        //instantiate buttons
        for(int i = 0; i < minigameStages.Length; i++){
            var mgButton = Instantiate(buttonObj);
            mgButton.transform.SetParent(buttonContainer.transform);
            mgButton.transform.localScale = new Vector3(1,1);
        }
    }

    public void Start(){
        buttonArr = buttonContainer.GetComponentsInChildren<Button>(true);

        for(int i = 0; i < buttonArr.Length; i++){
            var buttonText = buttonArr[i].transform.GetChild(0).GetComponent<TMPro.TMP_Text>();
            buttonText.text = minigameStages[i].levelName;
            
            var levelButton = buttonArr[i].GetComponent<MinigameLevelButton>();
            levelButton.levelIndex = i;
            levelButton.stageInfo = minigameStages[i];

            //enable/disable minigame button
            if(MinigameLoader.Instance.savedLevels[0].isUnlocked){
                minigameButton.gameObject.SetActive(true);
            } else {
                minigameButton.gameObject.SetActive(false);
            }

            //enable/disable minigame level buttons
            if(MinigameLoader.Instance.savedLevels[i].isUnlocked){
                buttonArr[i].gameObject.SetActive(true);
            } else {
                buttonArr[i].gameObject.SetActive(false);
            }

            if(MinigameLoader.Instance.savedLevels[i].isDone){
                var buttonStar = buttonArr[i].transform.GetChild(1).GetComponent<RawImage>();
                buttonStar.texture = starActive;
            }
        }

        var resize = buttonContainer.GetComponent<ResizeScrollObject>();
        resize.Resize();
    }

    public void loadLevelDetails(int levelIndex){
        levelName.text = minigameStages[levelIndex].levelName;
        levelDesc.text = minigameStages[levelIndex].levelDesc;

        if(MinigameLoader.Instance.savedLevels[levelIndex].isDone){
            star1.texture = starActive;
        } else {
            star1.texture = starInactive;
        }
    }

    public void LoadScene(){
        loadCanvas.gameObject.SetActive(true);
        StartCoroutine(LoadAsync(sceneToLoad));
    }

    public IEnumerator LoadAsync(string sceneToLoad){
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneToLoad);

        while(!loadOp.isDone){
            float progress = Mathf.Clamp01(loadOp.progress / 0.9f);
            loadBar.value = progress;
            yield return null;
        }
    }
}
