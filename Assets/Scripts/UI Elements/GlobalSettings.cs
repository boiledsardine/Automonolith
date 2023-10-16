using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class GlobalSettings : MonoBehaviour{
    public static GlobalSettings Instance { get; private set; }

    public ScreenResMode resolution;
    public bool isFullscreen;
    public string optionsSave;
    void Awake(){
        if(Instance == null){
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }

        optionsSave = Application.dataPath + "/Options.json";

        resolution = ScreenResMode._1366x768;
        isFullscreen = false;

        LoadOptions();

        SetScreenRes();
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

    //save and load options
    public void LoadOptions(){
        if(!File.Exists(optionsSave)){
            string savedSettings = JsonUtility.ToJson(new GameSettings(ScreenResMode._1366x768, false), true);
            File.WriteAllText(optionsSave, savedSettings);
        }

        string content = File.ReadAllText(optionsSave);

        if(string.IsNullOrEmpty(content) || content == "{}"){
            Debug.LogAssertion("Empty save entry");
            return;
        }

        GameSettings options = JsonUtility.FromJson<GameSettings>(content);

        resolution = options.resolution;
        isFullscreen = options.isFullscreen;
    }

    public void SaveSettings(){
        string savedSettings = JsonUtility.ToJson(new GameSettings(resolution, isFullscreen), true);
        File.WriteAllText(optionsSave, savedSettings);
    }

    void OnApplicationQuit(){
        SaveSettings();
    }
}

public class GameSettings{
    public GameSettings(ScreenResMode resolution, bool isFullscreen){
        this.resolution = resolution;
        this.isFullscreen = isFullscreen;
    }

    public ScreenResMode resolution;
    public bool isFullscreen;
}

public enum ScreenResMode{
    _1280x720,
    _1366x768,
    _1600x900,
    _1920x1080
}
