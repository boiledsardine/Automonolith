using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Use to add new entries to the savefiles
//Does nothing in-game
//OBSOLETE
public class SaveHandler : MonoBehaviour{
    public List<LevelInfo> savedLevels = new List<LevelInfo>();
    public List<EditorState> editorStates = new List<EditorState>();
    public string levelSave;
    public string editorSave;

    void Start(){
        levelSave = Application.dataPath + "/Saves/SaveLevels.json";
        editorSave = Application.dataPath + "/Saves/EditorSaves.json";

        LoadSaveFiles();
        DebugListCount();
    }

    public void AddLevelSaveEntry(){
        savedLevels.Add(new LevelInfo(false, false, false, false));
        
        string levelContent = JsonHelper.ToJson<LevelInfo>(savedLevels.ToArray(), true);
        File.WriteAllText(levelSave, levelContent);
    }

    public void AddEditorSaveEntry(){
        editorStates.Add(new EditorState(0, ""));

        string editorContent = JsonHelper.ToJson<EditorState>(editorStates.ToArray(), true);
        File.WriteAllText(editorSave, editorContent);
    }

    void DebugListCount(){
        Debug.Log("Entries in level save: " + savedLevels.Count);
        Debug.Log("Entries in editor save: " + editorStates.Count);
    }

    public void LoadSaveFiles(){
        if(!File.Exists(levelSave)){
            return;
        }

        if(!File.Exists(editorSave)){
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
