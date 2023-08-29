using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveGenerator : MonoBehaviour{
    public int saveCounts;

    public List<LevelInfo> savedLevels = new List<LevelInfo>();
    public List<EditorState> editorStates = new List<EditorState>();

    string levelSave;
    string editorSave;

    public void Awake(){
        //check for save files
        //generate new ones if they don't exist
        levelSave = Application.dataPath + "/SaveLevels.json";
        editorSave = Application.dataPath + "/EditorSaves.json";

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
}
