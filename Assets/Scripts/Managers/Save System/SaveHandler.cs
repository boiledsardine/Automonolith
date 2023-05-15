using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Use to add new entries to the savefiles
//Does nothing in-game
//This is a dev tool
public class SaveHandler : MonoBehaviour{
    List<LevelInfo> savedLevels = new List<LevelInfo>();
    List<EditorState> editorStates = new List<EditorState>();
    string levelSave;
    string editorSave;

    void Start(){
        levelSave = Application.dataPath + "/SaveLevels.json";
        editorSave = Application.dataPath + "/EditorSaves.json";

        LoadSaveFiles();
        DebugListCount();
    }

    public void AddSaveEntry(){
        savedLevels.Add(new LevelInfo(false, false, false, false));
        editorStates.Add(new EditorState(""));

        string levelContent = JsonHelper.ToJson<LevelInfo>(savedLevels.ToArray(), true);
        File.WriteAllText(levelSave, levelContent);

        string editorContent = JsonHelper.ToJson<EditorState>(editorStates.ToArray(), true);
        File.WriteAllText(editorSave, editorContent);
        
        Debug.Log("Added new entry to level and editor saves");
        DebugListCount();
    }

    void DebugListCount(){
        Debug.Log("Entries in level save: " + savedLevels.Count);
        Debug.Log("Entries in editor save: " + editorStates.Count);
    }

    public void LoadSaveFiles(){
        if(!File.Exists(levelSave) || !File.Exists(editorSave)){
            return;
        }
        
        string levelSaveContent = File.ReadAllText(levelSave);
        string editorSaveContent = File.ReadAllText(editorSave);

        if(string.IsNullOrEmpty(levelSaveContent) || levelSaveContent == "{}"){
            Debug.Log("Level save is empty");
        } else {
            savedLevels = JsonHelper.FromJson<LevelInfo>(levelSaveContent).ToList();
        }

        if(string.IsNullOrEmpty(editorSaveContent) || editorSaveContent == "{}"){
            Debug.Log("Editor save is empty");
        } else {
            editorStates = JsonHelper.FromJson<EditorState>(editorSaveContent).ToList();
        }
    }
}
