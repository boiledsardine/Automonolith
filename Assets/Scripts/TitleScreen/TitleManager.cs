using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System.Linq;

public class TitleManager : MonoBehaviour{
    public Button ngButton, contButton, quitButton;
    public Canvas loadCanvas, creditsCanvas, optionsCanvas;
    public Slider loadBar;
    string levelSave, editorSave, minigameSave;
    SaveGenerator saveGen;
    List<LevelInfo> savedLevels;
    public Image ngPanel, ngFrame, crPanel, crFrame;
    AudioSource source;

    void Start(){
        levelSave = Application.dataPath + "/Saves/SaveLevels.json";
        editorSave = Application.dataPath + "/Saves/EditorSaves.json";
        minigameSave = Application.dataPath + "/Saves/MinigameSaves.json";

        saveGen = GetComponent<SaveGenerator>();

        contButton.interactable = CheckContinue();
        source = GetComponent<AudioSource>();
    }

    bool CheckContinue(){
        //TODO: add security measure, check validity of save files

        if(!File.Exists(levelSave)){
            return false;
        }

        LoadLevels();

        if(!File.Exists(editorSave)){
            saveGen.editorStates = new List<EditorState>();

            if(!File.Exists(editorSave)){
                for(int i = 0; i < savedLevels.Count; i++){
                    saveGen.AddEditorSaveEntry(i);
                }
            }
        }

        if(!File.Exists(minigameSave)){
            saveGen.minigameStates = new List<MinigameState>();

            //generate new save
            //tutorial levels in index: 1, 5, 10, 13, 17
            int[] unlockLevels = {1, 5, 10, 13, 17};
            for(int i = 0; i < saveGen.minigameSaveCounts; i++){
                if(unlockLevels.Contains(i)){
                    //open minigame if level after tutorial is unlocked
                    //means tutorial itself is completed
                    saveGen.AddMinigameSaveEntry(i, savedLevels[i + 1].isUnlocked);
                } else {
                    //int i not a tutorial level, don't unlock
                    saveGen.AddMinigameSaveEntry(i, false);
                }
            }
        }

        return true;
    }

    void LoadLevels(){
        string content = File.ReadAllText(levelSave);

        if(string.IsNullOrEmpty(content) || content == "{}"){
            Debug.LogAssertion("Empty save entry");
            return;
        }

        savedLevels = JsonHelper.FromJson<LevelInfo>(content).ToList();
    }

    public void NewGame(){
        if(File.Exists(levelSave)){
            //open new game canvas
            OpenWindow(ngPanel, ngFrame);
        } else {
            saveGen.GenerateSaves();
            loadCanvas.gameObject.SetActive(true);
            StartCoroutine(LoadAsync(1));
        }
    }

    public void ResetAll(){
        //purge saves: Editor, Minigame, Levels
        //then fire savegenerator
        
        saveGen.DeleteSaves();
        saveGen.GenerateSaves();

        loadCanvas.gameObject.SetActive(true);
        StartCoroutine(LoadAsync(1));
    }

    public void CloseResetWindow(){
        StartCoroutine(CloseWindow(ngPanel, ngFrame));
    }

    public void OpenWindow(Image panel, Image frame){
        //open window
        panel.gameObject.SetActive(true);
        frame.gameObject.SetActive(true);

        var panelAnim = panel.GetComponent<Animator>();
        panelAnim.SetBool("isOpen", true);

        var frameAnim = frame.GetComponent<Animator>();
        frameAnim.SetBool("isOpen", true);

        PlayOpenWindow();
    }


    public IEnumerator CloseWindow(Image panel, Image frame){
        //close window
        PlayCloseWindow();

        var panelAnim = panel.GetComponent<Animator>();
        panelAnim.SetBool("isOpen", false);

        var frameAnim = frame.GetComponent<Animator>();
        frameAnim.SetBool("isOpen", false);

        yield return new WaitForSeconds(0.25f);

        panel.gameObject.SetActive(false);
        frame.gameObject.SetActive(false);
    }

    public void Continue(){
        //load scene 1 (main menu)
        loadCanvas.gameObject.SetActive(true);
        StartCoroutine(LoadAsync(1));
    }

    public void Options(){
        //do something
        //should open options submenu
        OptionsMenu optionMenu = optionsCanvas.GetComponent<OptionsMenu>();
        optionMenu.SetValues();

        optionsCanvas.gameObject.SetActive(true);
        Animator panelAnim = optionsCanvas.transform.GetChild(0).transform.gameObject.GetComponent<Animator>();
        panelAnim.SetBool("isOpen", true);
        
        Animator opPanel = GameObject.Find("CreditsCanvas").transform.Find("CR Panel").GetComponent<Animator>();
        opPanel.gameObject.SetActive(true);
        opPanel.SetBool("isOpen", true);
        
        PlayOpenWindow();
    }

    public void Credits(){
        //open credits window
        OpenWindow(crPanel, crFrame);

        creditsPageNum = 0;
        ChangeCreditsPage(0);

        creditsLeft.interactable = false;
        creditsRight.interactable = true;   
    }

    int creditsPageNum = 0;
    public GridLayoutGroup[] creditsPage;
    public Button creditsLeft, creditsRight;

    public void CreditsLeft(){
        creditsPageNum--;
        if(creditsPageNum == 0){
            creditsLeft.interactable = false;
        }
        creditsRight.interactable = true;

        ChangeCreditsPage(creditsPageNum);
    }

    public void CreditsRight(){
        creditsPageNum++;
        if(creditsPageNum == creditsPage.Length - 1){
            creditsRight.interactable = false;
        }
        creditsLeft.interactable = true;

        ChangeCreditsPage(creditsPageNum);
    }

    public void ChangeCreditsPage(int num){
        foreach(GridLayoutGroup gl in creditsPage){
            gl.gameObject.SetActive(false);
        }

        creditsPage[num].gameObject.SetActive(true);
    }

    public void CloseCredits(){
        StartCoroutine(CloseWindow(crPanel, crFrame));
    }

    public void Quit(){
        Invoke(nameof(ExitForRealsies), 0.5f);
    }

    void ExitForRealsies(){
        Application.Quit();
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

    void PlayOpenWindow(){
        source.outputAudioMixerGroup = AudioPicker.Instance.swooshMixer;

        source.clip = AudioPicker.Instance.titleWindowOpen;
        source.Play();
    }

    void PlayCloseWindow(){
        source.outputAudioMixerGroup = AudioPicker.Instance.swooshMixer;
        
        source.clip = AudioPicker.Instance.titleWindowClose;
        source.Play();
    }
}
