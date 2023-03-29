using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using CommandControl;

public class Reset : MonoBehaviour{
    public static Reset Instance { get; private set; }

    private void Awake(){
        if(Instance == null){
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    public void Exterminatus(){
        //Debug.LogAssertion("Declaring Exterminatus");
        Compiler.Instance.clearDictionaries();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public static bool isResetSuccessful(){
        if(SceneManager.GetActiveScene().isDirty){
            return false;
        }
        return true;
    }
}