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
    QuizUnlocker quizUnlocker;
    public GameObject buttonContainer;

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
        levelDetails = levelDetailsObject.levelDetails;
        for(int i = 0; i < levelDetails.Count; i++){
            var newButton = Instantiate(buttonContainer);
            newButton.transform.SetParent(buttonGroupContainer.transform);
            newButton.transform.localScale = new Vector3(1,1,1);
        }

        var buttonHolders = buttonGroupContainer.GetChildren();
        foreach(GameObject buttonHolder in buttonHolders){
            buttonArr.Add(buttonHolder.transform.GetChild(1).gameObject.GetComponent<Button>());
        }

        buttonGroupContainer.GetComponent<ResizeScrollObject>().Resize();
        
        int levelCounter = 1;
        for(int i = 0; i < buttonArr.Count; i++){
            var levelButton = buttonArr[i].GetComponent<LevelSelectButton>();
            levelButton.levelIndex = i;
            levelButton.sceneToLoad = i + levelOffset;
            if(LevelSaveLoad.Instance.savedLevels[i].isUnlocked){
                buttonArr[i].interactable = true;
                buttonArr[i].transform.GetChild(0).GetComponent<TMPro.TMP_Text>().color = Color.white;
                buttonHolders[i].transform.GetChild(0).GetComponent<Button>().interactable = true;
                buttonHolders[i].transform.GetChild(0).GetComponentInChildren<TMPro.TMP_Text>().color = Color.white;
            } else {
                buttonArr[i].interactable = false; 
                buttonArr[i].transform.GetChild(0).GetComponent<TMPro.TMP_Text>().color = disabledColor;
                buttonHolders[i].transform.GetChild(0).GetComponent<Button>().interactable = false;
                buttonHolders[i].transform.GetChild(0).GetComponentInChildren<TMPro.TMP_Text>().color = disabledColor;
            }
            if(levelsWithCutscene.Contains(i)){
                buttonArr[i].gameObject.GetComponent<LevelSelectButton>().hasCutscene = true;
            }
            var levelButtonText = buttonArr[i].gameObject.transform.GetChild(0).gameObject.GetComponent<TMPro.TMP_Text>();
            levelButtonText.text = levelDetails[i].levelName;

            var decoText = buttonHolders[i].transform.GetChild(0).GetComponentInChildren<TMPro.TMP_Text>();
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

        quizUnlocker = GetComponent<QuizUnlocker>();
        quizUnlocker.CountStars();
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
        if(sceneToLoad == 0 || sceneToLoad == 1){
            return;
        }

        int loadIndex = sceneToLoad - levelOffset;
        int[] quizLevels = {8, 16, 20};

        //if level has associated story scene, run that first
        if(loadCutscene){
            SceneManager.LoadScene("Story Scene");
        }

        //check star counts to unlock quiz levels
        else if(quizLevels.Contains(loadIndex)){
            if(quizUnlocker.CheckStars(loadIndex)){
                loadCanvas.gameObject.SetActive(true);
                StartCoroutine(LoadAsync(sceneToLoad));    
            }   
        }

        //normal levels
        else {
            loadCanvas.gameObject.SetActive(true);
            StartCoroutine(LoadAsync(sceneToLoad));
        }
    }

    public IEnumerator LoadAsync(int sceneIndex){
        AudioSource bgmSource = GameObject.Find("BGM Source").GetComponent<AudioSource>();
        bgmSource.mute = true;
        
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneIndex);

        loadOp.allowSceneActivation = false;

        while(!loadOp.isDone){
            float progress = Mathf.Clamp01(loadOp.progress / 0.9f);
            loadBar.value = progress;
            
            if(loadOp.progress >= 0.9f){
                yield return new WaitForSeconds(GlobalSettings.Instance.forceWaitTime);
                loadOp.allowSceneActivation = true;
            }
            
            yield return null;
        }
    }
}