using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class MinigameLoader : MonoBehaviour{
    public int levelIndex;
    public MinigameStageInfo stageInfo;
    public List<MinigameState> savedLevels = new List<MinigameState>();
    public static MinigameLoader Instance { get; private set; }
    string minigameSave;

    void Awake(){
        if(Instance == null){
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
        
        minigameSave = Application.dataPath + "/MinigameSaves.json";

        LoadLevelInfo();
    }

    public void LoadLevelInfo(){
        if(!File.Exists(minigameSave)){
            Debug.LogAssertion("NO SAVE FILES?");
            return;
        }

        string content = File.ReadAllText(minigameSave);

        if(string.IsNullOrEmpty(content) || content == "{}"){
            Debug.LogAssertion("Empty save entry");
            return;
        }

        savedLevels = JsonHelper.FromJson<MinigameState>(content).ToList();
    }

    public void EndLevelSave(int levelIndex, bool done){
        //saves the current level
        savedLevels[levelIndex] = new MinigameState(true, done);

        //saves everything
        SaveLevels();
    }

    public void SaveLevels(){
        //writes to save file
        string content = JsonHelper.ToJson<MinigameState>(savedLevels.ToArray(), true);
        File.WriteAllText(minigameSave, content);
    }

    public void SaveLevels(MinigameState[] minigameStates){
        //writes to save file
        string content = JsonHelper.ToJson<MinigameState>(minigameStates.ToArray(), true);
        File.WriteAllText(minigameSave, content);
    }
}
