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
    }

    public void SetStars(bool star1Open, bool star2Open, bool star3Open){
        star1.texture = star1Open ? starActive : starInactive;
        star2.texture = star2Open ? starActive : starInactive;
        star3.texture = star3Open ? starActive : starInactive;
    }

    public void EndLevel(){
        //then return to main menu
        SceneManager.LoadScene("Main Menu");
        //destroy persistents
        DontDestroy[] persistents = FindObjectsOfType<DontDestroy>();
        foreach(DontDestroy obj in persistents){
            Destroy(obj.gameObject);
        }
    }
}
