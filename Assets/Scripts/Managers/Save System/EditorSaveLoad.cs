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
    public int levelIndex;
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
    //does nothing if the editor state is empty
    public void LoadEditorState(){
        if(!File.Exists(editorSave)){
            return;
        }

        string content = File.ReadAllText(editorSave);

        if(string.IsNullOrEmpty(content) || content == "{}"){
            return;
        }

        editorStates = JsonHelper.FromJson<EditorState>(content).ToList();

        if(!string.IsNullOrWhiteSpace(editorStates[levelIndex].editorContent)){
            string savedText = editorStates[levelIndex].editorContent;
            
            editor.code = savedText;
            editorText.text = savedText;

            //update lineIndex and charIndex
            string[] codeLines = savedText.Split('\n');
            string finalLine = codeLines[codeLines.Length - 1];
            int lineMaxIndex = finalLine.Length;

            editor.lineIndex = codeLines.Length - 1;
            editor.charIndex = lineMaxIndex;
        }
    }
}

[System.Serializable]
public class EditorState{
    public EditorState(string editorContent){
        this.editorContent = editorContent;
    }
    public string editorContent;
}