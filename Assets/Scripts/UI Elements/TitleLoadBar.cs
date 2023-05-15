using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleLoadBar : MonoBehaviour{
    public TMPro.TMP_Text loadText;
    public string textToSay;
    public Slider loadBar;

    void Start(){
        StartCoroutine(loadAsync(1));
    }

    public IEnumerator loadAsync(int sceneIndex){
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneIndex);
        loadOp.allowSceneActivation = false;

        while(!loadOp.isDone){
            float progress = Mathf.Clamp01(loadOp.progress / 0.9f);
            loadBar.value = progress;

            if(loadOp.progress >= 0.9f){
                loadText.text = textToSay;
                if(Input.GetKeyDown(KeyCode.Space)){
                    loadOp.allowSceneActivation = true;
                }
            }

            yield return null;
        }
    }
}