using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PostlevelCanvas : MonoBehaviour{
    public static PostlevelCanvas Instance { get; private set; }

    public RawImage star1, star2, star3;
    public Texture starActive, starInactive;

    void Awake(){
        if(Instance == null){
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    public void OpenCanvas(){
        Animator anim = gameObject.GetComponent<Animator>();
        anim.SetBool("isOpen", true);
        TempMuteSource();
    }

    public void SetStars(bool star1Open, bool star2Open, bool star3Open){
        star1.texture = star1Open ? starActive : starInactive;
        star2.texture = star2Open ? starActive : starInactive;
        star3.texture = star3Open ? starActive : starInactive;
    }

    public void EndLevel(){        
        StartCoroutine(LoadAsync());
    }

    void TempMuteSource(){
        AudioSource[] sources = GetComponentsInChildren<AudioSource>();
        foreach(AudioSource source in sources){
            source.enabled = false;
        }
        Invoke(nameof(UnmuteSource), 0.20f);
    }

    void UnmuteSource(){
        AudioSource[] sources = GetComponentsInChildren<AudioSource>();
        foreach(AudioSource source in sources){
            source.enabled = true;
        }
    }

    public Canvas loadCanvas;

    public IEnumerator LoadAsync(){
        Debug.Log("LOADING!");
        loadCanvas.gameObject.SetActive(true);
        AsyncOperation loadOp = SceneManager.LoadSceneAsync("Main Menu");

        loadOp.allowSceneActivation = false;

        while(!loadOp.isDone){
            Slider loadBar = loadCanvas.transform.Find("Slider").GetComponent<Slider>();
            float progress = Mathf.Clamp01(loadOp.progress / 0.9f);
            loadBar.value = progress;

            if(loadOp.progress >= 0.9f){
                yield return new WaitForSeconds(GlobalSettings.Instance.forceWaitTime);
                loadOp.allowSceneActivation = true;

                //destroy persistents
                DontDestroy[] persistents = FindObjectsOfType<DontDestroy>();
                foreach(DontDestroy obj in persistents){
                    if(obj.transform.gameObject.tag != "Save Manager"){
                        Destroy(obj.gameObject);
                    }
                }
            }

            yield return null;
        }
    }
}
