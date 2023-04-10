using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour{

    public string sceneToLoad;

    public void loadScene(){
        SceneManager.LoadScene(sceneToLoad);
    }
}
