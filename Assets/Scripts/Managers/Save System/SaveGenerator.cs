using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

//TODO: add to levelsaves a boolean that tracks if that level's associated cutscene has been played
//if it has, open that cutscene in the story repository and disable that cutscene on level play (except tutorials)
//so like 14 (10 levels + 3 quizzes + day 1 scene)?
public class SaveGenerator : MonoBehaviour{
    public int saveCounts;
    public int minigameSaveCounts;

    public List<LevelInfo> savedLevels = new List<LevelInfo>();
    public List<EditorState> editorStates = new List<EditorState>();
    public List<MinigameState> minigameStates = new List<MinigameState>();

    string levelSave, editorSave, minigameSave, etcSave, saveDir;
    public bool unlockAllLevels, unlockAllMinigames;

    public void Awake(){
        //check for directory
        saveDir = Application.dataPath + "/Saves";
        if(!Directory.Exists(saveDir)){
            Directory.CreateDirectory(saveDir);
        }
        
        //check for save files
        //generate new ones if they don't exist
        levelSave = Application.dataPath + "/Saves/SaveLevels.json";
        editorSave = Application.dataPath + "/Saves/EditorSaves.json";
        minigameSave = Application.dataPath + "/Saves/MinigameSaves.json";
        etcSave = Application.dataPath + "/Saves/EtcSave.json";

        if(!File.Exists(levelSave)){
            for(int i = 0; i < saveCounts; i++){
                AddLevelSaveEntry();
            }
        }

        if(!File.Exists(editorSave)){
            for(int i = 0; i < saveCounts; i++){
                AddEditorSaveEntry();
            }
        }

        if(!File.Exists(minigameSave)){
            for(int i = 0; i < minigameSaveCounts; i++){
                AddMinigameSaveEntry();
            }
        }
    }

    public void AddLevelSaveEntry(){
        if(unlockAllLevels){
            savedLevels.Add(new LevelInfo(true, false, false, false, false));
        } else {
            savedLevels.Add(new LevelInfo(false, false, false, false, false));
        }

        savedLevels[0] = new LevelInfo(true, false, false, false, false);
        
        string levelContent = JsonHelper.ToJson<LevelInfo>(savedLevels.ToArray(), true);
        File.WriteAllText(levelSave, levelContent);
    }

    public void AddEditorSaveEntry(){
        editorStates.Add(new EditorState(""));

        string editorContent = JsonHelper.ToJson<EditorState>(editorStates.ToArray(), true);
        File.WriteAllText(editorSave, editorContent);
    }

    public void AddMinigameSaveEntry(){
        if(unlockAllMinigames){
            minigameStates.Add(new MinigameState(true, false));
        } else {
            minigameStates.Add(new MinigameState(false, false));
        }

        string minigameContent = JsonHelper.ToJson<MinigameState>(minigameStates.ToArray(), true);
        File.WriteAllText(minigameSave, minigameContent);
    }
}

[System.Serializable]
public class LevelInfo{
    public LevelInfo(bool isUnlocked, bool star1, bool star2, bool star3, bool sceneWatched){
        this.isUnlocked = isUnlocked;
        this.star1 = star1;
        this.star2 = star2;
        this.star3 = star3;
        this.sceneWatched = sceneWatched;
    }

    public LevelInfo(bool isUnlocked, bool star1, bool star2, bool star3){
        this.isUnlocked = isUnlocked;
        this.star1 = star1;
        this.star2 = star2;
        this.star3 = star3;
    }

    public bool isUnlocked, star1, star2, star3, sceneWatched;
}

[System.Serializable]
public class EditorState{
    public EditorState(string editorContent){
        this.editorContent = editorContent;
    }
    public string editorContent;
}

[System.Serializable]
public class MinigameState{
    public MinigameState(bool isUnlocked, bool isDone){
        this.isUnlocked = isUnlocked;
        this.isDone = isDone;
    }
    public bool isUnlocked;
    public bool isDone;
}