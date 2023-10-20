using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuControl : MonoBehaviour{
    public static MenuControl Instance { get; private set; }

    [SerializeField] private Canvas menuCanvas;
    [SerializeField] private Canvas optionsCanvas;
    [SerializeField] private Animator panelAnimator;
    [SerializeField] private Animator submenuAnimator;
    [SerializeField] private EditorSaveLoad saveLoad;
    [SerializeField] private AlmanacManager fsHelp;

    private void Awake(){
        if(Instance == null){
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    public void openMenu(){
        menuCanvas.gameObject.SetActive(true);
        panelAnimator.SetBool("isOpen", true);
        submenuAnimator.SetBool("isOpen", true);
    }

    public void closeMenu(){
        panelAnimator.SetBool("isOpen", false);
        submenuAnimator.SetBool("isOpen", false);
        Invoke("disableMenu", 0.25f);
    }

    private void disableMenu(){
        menuCanvas.gameObject.SetActive(false);
    }

    public void options(){
        //do something
        //should open options submenu

        optionsCanvas.gameObject.SetActive(true);
        Animator panelAnim = optionsCanvas.transform.GetChild(0).transform.gameObject.GetComponent<Animator>();
        panelAnim.SetBool("isOpen", true);
    }

    public void controls(){
        //do something
        closeMenu();
        fsHelp.OpenHelp();
    }

    public void exitGame(){
        var currentScene = SceneManager.GetActiveScene();
        if(currentScene.name == "Main Menu"){
            Application.Quit();
        }

        var maptacks = GameObject.Find("Maptacks");
        if(maptacks != null){
            Destroy(maptacks);
        }
        
        closeMenu();

        //Saves editor state on exit
        if(saveLoad != null && currentScene.name != "mg-level"){
            saveLoad.SaveEditorState();
        }

        LastSceneHolder.Instance.lastScene = SceneManager.GetActiveScene().buildIndex;
        LastSceneHolder.Instance.lastSceneName = SceneManager.GetActiveScene().name;

        SceneManager.LoadScene("Main Menu");

        //Destroy all objects with the DontDestroy class
        //Works, but is kind of an inelegant solution???
        //look for something else eventually
        DontDestroy[] persistents = FindObjectsOfType<DontDestroy>();
        foreach(DontDestroy obj in persistents){
            Destroy(obj.gameObject);
        }
    }
}
