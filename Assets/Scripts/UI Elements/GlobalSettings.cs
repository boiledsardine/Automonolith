using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System.IO;

public class GlobalSettings : MonoBehaviour{
    public static GlobalSettings Instance { get; private set; }

    public ScreenResMode resolution;
    public bool isFullscreen;
    public float bgmVolume, sfxVolume;
    public string optionsSave;
    public float forceWaitTime = 1f;
    public AudioMixer mixer;
    void Awake(){
        if(Instance == null){
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }

        optionsSave = Application.dataPath + "/Saves/Options.json";
        GenerateSave();
        LoadOptions();
    }

    void Start(){
        SetScreenRes();
        SetVolume();
    }

    void SetDefault(){
        resolution = ScreenResMode._1366x768;
        isFullscreen = false;
        bgmVolume = 1f;
        sfxVolume = 1f;
    }

    void SetScreenRes(){
        switch(resolution){
            case ScreenResMode._1280x720:
                Screen.SetResolution(1280, 720, isFullscreen);
            break;
            case ScreenResMode._1366x768:
                Screen.SetResolution(1366, 768, isFullscreen);
            break;
            case ScreenResMode._1600x900:
                Screen.SetResolution(1600, 900, isFullscreen);
            break;
            case ScreenResMode._1920x1080:
                Screen.SetResolution(1920, 1080, isFullscreen);
            break;
        }
    }

    void SetVolume(){
        mixer.SetFloat("sfxMasterVolume", Mathf.Log10(sfxVolume) * 20);
        mixer.SetFloat("bgmMasterVolume", Mathf.Log10(bgmVolume) * 20);
    }

    //save and load options
    void GenerateSave(){
        string saveDir = Application.dataPath + "/Saves";
        if(!Directory.Exists(saveDir)){
            Directory.CreateDirectory(saveDir);
        }

        if(!File.Exists(optionsSave)){
            string savedSettings = JsonUtility.ToJson(new GameSettings(ScreenResMode._1366x768, false, 1f, 1f), true);
            File.WriteAllText(optionsSave, savedSettings);
        }
    }

    public void LoadOptions(){
        string content = File.ReadAllText(optionsSave);

        if(string.IsNullOrEmpty(content) || content == "{}"){
            Debug.LogAssertion("Empty save entry");
            return;
        }

        GameSettings options = JsonUtility.FromJson<GameSettings>(content);

        resolution = options.resolution;
        isFullscreen = options.isFullscreen;
        bgmVolume = options.bgmVolume;
        sfxVolume = options.sfxVolume;

        if(bgmVolume == 0){
            bgmVolume = 0.0001f;
        }
        if(sfxVolume == 0){
            sfxVolume = 0.0001f;
        }
    }

    public void SaveSettings(){
        string savedSettings = JsonUtility.ToJson(new GameSettings(resolution, isFullscreen, bgmVolume, sfxVolume), true);
        File.WriteAllText(optionsSave, savedSettings);
    }

    void OnApplicationQuit(){
        SaveSettings();
    }
}

public class GameSettings{
    public GameSettings(ScreenResMode resolution, bool isFullscreen, float bgmVolume, float sfxVolume){
        this.resolution = resolution;
        this.isFullscreen = isFullscreen;
        this.bgmVolume = bgmVolume;
        this.sfxVolume = sfxVolume;
    }

    public ScreenResMode resolution;
    public bool isFullscreen;
    public float bgmVolume, sfxVolume;
}

public enum ScreenResMode{
    _1280x720,
    _1366x768,
    _1600x900,
    _1920x1080
}