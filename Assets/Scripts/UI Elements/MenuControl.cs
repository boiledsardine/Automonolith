using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class MenuControl : MonoBehaviour{
    public static MenuControl Instance { get; private set; }

    [SerializeField] private Canvas menuCanvas;
    [SerializeField] private Canvas optionsCanvas;
    [SerializeField] private Animator panelAnimator;
    [SerializeField] private Animator submenuAnimator;
    [SerializeField] private EditorSaveLoad saveLoad;
    [SerializeField] private AlmanacManager fsHelp;
    AudioSource source;
    [SerializeField] Button[] menuButtons;
    OptionsMenu optionMenu;

    private void Awake(){
        if(Instance == null){
            Instance = this;
        } else {
            Destroy(gameObject);
        }

        source = GetComponent<AudioSource>();
        optionMenu = optionsCanvas.GetComponent<OptionsMenu>();
    }

    public void openMenu(){
        menuCanvas.gameObject.SetActive(true);
        panelAnimator.SetBool("isOpen", true);
        submenuAnimator.SetBool("isOpen", true);
        PlayOpenSound();
        foreach(Button b in menuButtons){
            var buttonAudio = b.GetComponent<AudioSource>();
            buttonAudio.volume = GlobalSettings.Instance.sfxVolume;
        }
    }

    public void closeMenu(bool playCloseSound){
        if(playCloseSound){
            PlayCloseSound();
        }

        foreach(Button b in menuButtons){
            var buttonAudio = b.GetComponent<AudioSource>();
            buttonAudio.volume = 0;
        }
        panelAnimator.SetBool("isOpen", false);
        submenuAnimator.SetBool("isOpen", false);
        Invoke("disableMenu", 0.25f);
    }

    private void disableMenu(){
        menuCanvas.gameObject.SetActive(false);
    }

    public void options(){
        //do something
        //should open options submenu
        
        optionMenu.SetValues();

        optionsCanvas.gameObject.SetActive(true);
        Animator panelAnim = optionsCanvas.transform.GetChild(0).transform.gameObject.GetComponent<Animator>();
        panelAnim.SetBool("isOpen", true);
        PlayBeep();
        
        optionMenu.PlayOpenSound();
    }

    public void controls(){
        //do something
        PlayBeep();
        closeMenu(false);
        fsHelp.OpenHelp();
    }

    public void exitGame(){
        PlayBeep();
        var currentScene = SceneManager.GetActiveScene();
        if(currentScene.name == "Main Menu"){
            Destroy(FindObjectOfType<LevelSaveLoad>().gameObject);
            SceneManager.LoadScene(0);
            return;
        }

        var maptacks = GameObject.Find("Maptacks");
        if(maptacks != null){
            Destroy(maptacks);
        }
        
        closeMenu(true);

        //Saves editor state on exit
        if(saveLoad != null && currentScene.name != "mg-level"){
            saveLoad.SaveEditorState();
        }

        LastSceneHolder.Instance.lastScene = SceneManager.GetActiveScene().buildIndex;
        LastSceneHolder.Instance.lastSceneName = SceneManager.GetActiveScene().name;

        StartCoroutine(LoadAsync());
    }

    public Canvas loadCanvas;

    public IEnumerator LoadAsync(){
        Debug.Log("LOADING!");
        loadCanvas.gameObject.SetActive(true);
        AsyncOperation loadOp = SceneManager.LoadSceneAsync("Main Menu");

        loadOp.allowSceneActivation = false;

        while(!loadOp.isDone){
            Slider loadBar = loadCanvas.transform.Find("Slider").GetComponent<Slider>();
            float progress = Mathf.Clamp01(loadOp.progress / 0.9f);
            loadBar.value = progress;

            if(loadOp.progress >= 0.9f){
                yield return new WaitForSeconds(GlobalSettings.Instance.forceWaitTime);
                loadOp.allowSceneActivation = true;

                //Destroy all objects with the DontDestroy class
                //Works, but is kind of an inelegant solution???
                //look for something else eventually
                DontDestroy[] persistents = FindObjectsOfType<DontDestroy>();
                foreach(DontDestroy obj in persistents){
                    Destroy(obj.gameObject);
                }
            }

            yield return null;
        }
    }

    
    void PlayOpenSound(){
        TempDisableMenuSound();

        float globalVolume = GlobalSettings.Instance.sfxVolume;
        float multiplier = AudioPicker.Instance.menuSwooshVolume;
        source.volume = globalVolume * multiplier;

        source.clip = AudioPicker.Instance.menuOpen;
        source.Play();
    }

    void PlayCloseSound(){
        float globalVolume = GlobalSettings.Instance.sfxVolume;
        float multiplier = AudioPicker.Instance.menuSwooshVolume;
        source.volume = globalVolume * multiplier;

        source.clip = AudioPicker.Instance.menuClose;       
        source.Play();
    }

    void PlayBeep(){
        float globalVolume = GlobalSettings.Instance.sfxVolume;
        float multiplier = AudioPicker.Instance.buttonVolume;
        source.volume = globalVolume * multiplier;

        source.clip = AudioPicker.Instance.beep;
        source.Play();
    }

    void TempDisableMenuSound(){
        foreach(Button b in menuButtons){
            GameUISound menuButtonSound = b.GetComponent<GameUISound>();
            if(menuButtonSound != null){
                menuButtonSound.enable = false;
            }
        }       
        Invoke(nameof(ReEnableMenuSound), 0.3f);
    }

    void ReEnableMenuSound(){
        foreach(Button b in menuButtons){
            GameUISound menuButtonSound = b.GetComponent<GameUISound>();
            if(menuButtonSound != null){
                menuButtonSound.enable = true;
            }
        }
    }
}
