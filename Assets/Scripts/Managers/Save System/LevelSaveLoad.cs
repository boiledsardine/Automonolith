using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelSaveLoad : MonoBehaviour{
    public static LevelSaveLoad Instance;

    public List<LevelInfo> savedLevels = new List<LevelInfo>();
    public List<EditorState> editorStates = new List<EditorState>();
    public string levelSave;
    public string editorSave;
    public int saveCounts = 10;

    void Awake(){
        if(Instance == null){
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }

        levelSave = Application.dataPath + "/SaveLevels.json";
        editorSave = Application.dataPath + "/EditorSaves.json";

        GenerateSaveFiles();
    }

    void GenerateSaveFiles(){
        //check for save files
        //generate new ones if they don't exist
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

        LoadLevelInfo();

        if(!savedLevels[0].isUnlocked){
            savedLevels[0].isUnlocked = true;
            SaveLevels();
        }
    }

    public void AddLevelSaveEntry(){
        savedLevels.Add(new LevelInfo(false, false, false, false));
        
        string levelContent = JsonHelper.ToJson<LevelInfo>(savedLevels.ToArray(), true);
        File.WriteAllText(levelSave, levelContent);
    }

    public void AddEditorSaveEntry(){
        editorStates.Add(new EditorState(""));

        string editorContent = JsonHelper.ToJson<EditorState>(editorStates.ToArray(), true);
        File.WriteAllText(editorSave, editorContent);
    }
    
    //called after level success
    public void SaveLevels(){
        //writes to save file
        string content = JsonHelper.ToJson<LevelInfo>(savedLevels.ToArray(), true);
        File.WriteAllText(levelSave, content);
    }

    public void EndLevelSave(int levelIndex, bool star1, bool star2, bool star3, bool openNext){
        //checks if level stars are already achieved
        //if they are they the three if statements will ensure that they are always true
        //even though SaveLevelInfo takes false for the stars
        bool star1Open = savedLevels[levelIndex].star1;
        bool star2Open = savedLevels[levelIndex].star2;
        bool star3Open = savedLevels[levelIndex].star3;

        if(star1Open){
            star1 = true;
        }
        if(star2Open){
            star2 = true;
        }
        if(star3Open){
            star3 = true;
        }

        //saves the current level
        savedLevels[levelIndex] = new LevelInfo(true, star1, star2, star3);

        //opens the next level if it exists
        if(openNext){
            OpenNextLevel(levelIndex + 1);
        }

        //saves everything
        SaveLevels();
    }

    public void OpenNextLevel(int nextLevelIndex){
        //checks if there's a next level
        //if there is, unlocks the next level if it isn't yet
        if(nextLevelIndex <= savedLevels.Count){
            if(!savedLevels[nextLevelIndex].isUnlocked){
                savedLevels[nextLevelIndex].isUnlocked = true;
            }
        }
    }
    
    //called when the main menu is opened
    public void LoadLevelInfo(){
        if(!File.Exists(levelSave)){
            return;
        }

        string content = File.ReadAllText(levelSave);

        if(string.IsNullOrEmpty(content) || content == "{}"){
            return;
        }

        savedLevels = JsonHelper.FromJson<LevelInfo>(content).ToList();
    }

    public void DebugList(){
        Debug.Log("Entities in list: " + savedLevels.Count);
        if(savedLevels.Count > 0){
            int currentIndex = 0;

            foreach(LevelInfo level in savedLevels){
                Debug.Log(currentIndex + ": " + level.isUnlocked);
                Debug.Log(currentIndex + ": " + level.star1);
                Debug.Log(currentIndex + ": " + level.star2);
                Debug.Log(currentIndex + ": " + level.star3);
                currentIndex++;
            }
        }
    }
}

[System.Serializable]
public class LevelInfo{
    public LevelInfo(bool isUnlocked, bool star1, bool star2, bool star3){
        this.isUnlocked = isUnlocked;
        this.star1 = star1;
        this.star2 = star2;
        this.star3 = star3;
    }
    public bool isUnlocked, star1, star2, star3;
}