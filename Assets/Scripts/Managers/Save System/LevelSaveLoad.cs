using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelSaveLoad : MonoBehaviour{
    public static LevelSaveLoad Instance;

    private string levelSave;
    public List<LevelInfo> savedLevels = new List<LevelInfo>();

    void Awake(){
        if(Instance == null){
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }

        levelSave = Application.dataPath + "/SaveLevels.json";

        LoadLevelInfo();
    }
    
    //called after level success
    public void SaveLevelInfo(int levelIndex, bool star1, bool star2, bool star3){
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

        //checks if there's a next level
        //if there is, unlocks the next level if it isn't yet
        if(levelIndex + 1 != savedLevels.Count){
            if(!savedLevels[levelIndex + 1].isUnlocked){
                savedLevels[levelIndex + 1].isUnlocked = true;
            }
        }

        //writes to save file
        string content = JsonHelper.ToJson<LevelInfo>(savedLevels.ToArray(), true);
        File.WriteAllText(levelSave, content);
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