using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StoryManager : MonoBehaviour{
    public int sceneIndex;
    public int[] scenesWithStory;
    public Conversation[] convoList;
    public Canvas loadCanvas, dialogueCanvas;
    public Slider loadBar;
    Dictionary<int,Conversation> convoDict = new Dictionary<int,Conversation>();

    void Awake(){
        sceneIndex = LevelSaveLoad.Instance.indexHolder;

        for(int i = 0; i < scenesWithStory.Length; i++){
            convoDict.Add(scenesWithStory[i], convoList[i]);
        }
    }

    void Start(){
        if(!convoDict.ContainsKey(sceneIndex)){
            Debug.LogAssertion("No cutscene");
            return;
        }
        
        dialogueCanvas.transform.gameObject.SetActive(true);
        DialogueManager.Instance.startDialogue(convoDict[sceneIndex]);
    }

    public void DialogueEnd(){
        LevelSaveLoad.Instance.EndStorySceneSave(sceneIndex);

        Destroy(GameObject.Find("Essentials"));
        loadCanvas.gameObject.SetActive(true);
        StartCoroutine(loadAsync(sceneIndex + 2));
    }

    public IEnumerator loadAsync(int sceneIndex){
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneIndex);

        while(!loadOp.isDone){
            float progress = Mathf.Clamp01(loadOp.progress / 0.9f);
            loadBar.value = progress;
            yield return null;
        }
    }
}
