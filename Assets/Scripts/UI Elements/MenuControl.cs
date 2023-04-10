using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuControl : MonoBehaviour{
    public static MenuControl Instance { get; private set; }

    [SerializeField] private Animator panelAnimator;
    [SerializeField] private Animator submenuAnimator;
    [SerializeField] private GameObject[] persistents;

    private void Awake(){
        if(Instance == null){
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    public void openMenu(){
        gameObject.transform.parent.gameObject.SetActive(true);
        panelAnimator.SetBool("isOpen", true);
        submenuAnimator.SetBool("isOpen", true);
    }

    public void closeMenu(){
        panelAnimator.SetBool("isOpen", false);
        submenuAnimator.SetBool("isOpen", false);
        Invoke("disableMenu", 0.25f);
    }

    private void disableMenu(){
        gameObject.transform.parent.gameObject.SetActive(false);
    }

    public void options(){
        //do something
    }

    public void controls(){
        //do something
    }

    public void exitGame(){
        closeMenu();
        SceneManager.LoadScene("Main Menu");

        //Destroy all objects marked with DontDestroyOnLoad
        //Works, but is kind of an inelegant solution???
        //look for something else eventually
        foreach(GameObject obj in persistents){
            Destroy(obj);
        }
    }
}
