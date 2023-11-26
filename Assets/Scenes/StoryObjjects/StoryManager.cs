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
        Debug.Log("running story");
        
        dialogueCanvas.gameObject.SetActive(true);
        DialogueManager.Instance.startDialogue(convoDict[sceneIndex]);
    }

    public void DialogueEnd(){
        Debug.Log("ending dialogue");
        LevelSaveLoad.Instance.EndStorySceneSave(sceneIndex);

        loadCanvas.gameObject.SetActive(true);
        StartCoroutine(LoadAsync(sceneIndex + 2));
    }

    public IEnumerator LoadAsync(int sceneIndex){
        Debug.Log("LOADING!");
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneIndex);

        loadOp.allowSceneActivation = false;

        while(!loadOp.isDone){
            float progress = Mathf.Clamp01(loadOp.progress / 0.9f);
            loadBar.value = progress;
            
            if(loadOp.progress >= 0.9f){
                yield return new WaitForSeconds(GlobalSettings.Instance.forceWaitTime);
                loadOp.allowSceneActivation = true;
                Destroy(GameObject.Find("Essentials"));
            }
            
            yield return null;
        }
    }
}
