using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class LevelSelectManager : MonoBehaviour{
    public static LevelSelectManager Instance { get; private set; }
    public int sceneToLoad;
    public Canvas loadCanvas;
    public Slider loadBar;
    public TMPro.TMP_Text levelName, levelDesc;
    public RawImage star1, star2, star3;
    public Texture starActive, starInactive;
    public LevelDetails levelDetailsObject;
    private List<LevelDetail> levelDetails;
    public List<Button> buttonArr;
    public GameObject buttonGroupContainer;
    public int levelOffset = 2;
    public bool loadCutscene = false;
    public ScrollRect levelSelectScroll;
    public Color disabledColor = new Color(200,200,200,128);
    public int[] levelsWithCutscene;

    void Awake(){
        if(Instance == null){
            Instance = this;
        } else {
            Destroy(gameObject);
        }

        //resets timescale changes from closed levels
        Time.timeScale = 1;
    }

    void Start(){
        var buttonHolders = buttonGroupContainer.GetChildren();
        foreach(GameObject buttonHolder in buttonHolders){
            buttonArr.Add(buttonHolder.transform.GetChild(0).gameObject.GetComponent<Button>());
        }

        buttonGroupContainer.GetComponent<ResizeScrollObject>().Resize();
        levelDetails = levelDetailsObject.levelDetails;
        
        int levelCounter = 1;
        for(int i = 0; i < buttonArr.Count; i++){
            var levelButton = buttonArr[i].GetComponent<LevelSelectButton>();
            levelButton.levelIndex = i;
            levelButton.sceneToLoad = i + levelOffset;
            if(LevelSaveLoad.Instance.savedLevels[i].isUnlocked){
                buttonArr[i].interactable = true;
                buttonArr[i].transform.GetChild(0).GetComponent<TMPro.TMP_Text>().color = Color.white;
                buttonArr[i].transform.GetChild(1).GetComponent<Button>().interactable = true;
                buttonArr[i].transform.GetChild(1).GetComponentInChildren<TMPro.TMP_Text>().color = Color.white;
            } else {
                buttonArr[i].interactable = false; 
                buttonArr[i].transform.GetChild(0).GetComponent<TMPro.TMP_Text>().color = disabledColor;
                buttonArr[i].transform.GetChild(1).GetComponent<Button>().interactable = false;
                buttonArr[i].transform.GetChild(1).GetComponentInChildren<TMPro.TMP_Text>().color = disabledColor;
            }
            if(levelsWithCutscene.Contains(i)){
                buttonArr[i].gameObject.GetComponent<LevelSelectButton>().hasCutscene = true;
            }
            var levelButtonText = buttonArr[i].gameObject.transform.GetChild(0).gameObject.GetComponent<TMPro.TMP_Text>();
            levelButtonText.text = levelDetails[i].levelName;

            var decoText = buttonArr[i].transform.GetChild(1).GetComponentInChildren<TMPro.TMP_Text>();
            if(levelDetails[i].isTutorial){
                decoText.text = "Tutorial";
            } else if(levelDetails[i].isQuiz){
                decoText.text = "Quiz";
            } else {
                decoText.text = "Level " + levelCounter;
                levelCounter++;
            }
        }

        //focus on last button
        /*if(LastSceneHolder.Instance.lastScene < 23 && LastSceneHolder.Instance.lastScene > 1){
            int indexOfButton = LastSceneHolder.Instance.lastScene - levelOffset;
            var focusedButtonRect = buttonGroupContainer.transform.GetChild(indexOfButton).GetComponent<RectTransform>();
            levelSelectScroll.FocusOnItem(focusedButtonRect);
        }*/
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

    //called by play button
    public void loadScene(){
        //if level has associated story scene, run that first
        if(loadCutscene){
            SceneManager.LoadScene("Story Scene");
        } else {
            loadCanvas.gameObject.SetActive(true);
            StartCoroutine(loadAsync(sceneToLoad));
        }
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