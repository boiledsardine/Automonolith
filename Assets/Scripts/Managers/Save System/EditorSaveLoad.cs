using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//works great
public class EditorSaveLoad : MonoBehaviour{
    public static EditorSaveLoad Instance;
    public TMPro.TMP_Text editorText;
    public CodeEditor editor;
    public int levelIndex; //0 is the testing map
    private string editorSave;
    private List<EditorState> editorStates = new List<EditorState>();

    void Awake(){
        if(Instance == null){
            Instance = this;
        } else {
            Destroy(this.gameObject);
        }
    }

    void Start(){
        editorSave = Application.dataPath + "/EditorSaves.json";

        LoadEditorState();
    }  

    //saves whatever's written in the editor
    public void SaveEditorState(){
        editorStates[levelIndex].editorContent = editor.code;

        string content = JsonHelper.ToJson<EditorState>(editorStates.ToArray(), true);
        File.WriteAllText(editorSave, content);
    }

    //replaces editor text with whatever's in the savefile
    //called when the level is opened
    public void LoadEditorState(){
        if(!File.Exists(editorSave)){
            return;
        }

        string content = File.ReadAllText(editorSave);

        if(string.IsNullOrEmpty(content) || content == "{}"){
            return;
        }

        editorStates = JsonHelper.FromJson<EditorState>(content).ToList();
        editor.code = editorStates[levelIndex].editorContent;
        editorText.text = editorStates[levelIndex].editorContent;
    }
}

[System.Serializable]
public class EditorState{
    public EditorState(string editorContent){
        this.editorContent = editorContent;
    }
    public string editorContent;
}