using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LastSceneHolder : MonoBehaviour{
    public static LastSceneHolder Instance { get; private set; }
    public int lastScene;
    public string lastSceneName;
    void Awake(){
        if(Instance == null){
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    public void SetLastScene(){
        lastScene = SceneManager.GetActiveScene().buildIndex;
        lastSceneName = SceneManager.GetActiveScene().name;
    }
}
