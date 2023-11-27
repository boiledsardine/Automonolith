using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Ayin called, he wants his TT2 protocol back
public class Timescaler : MonoBehaviour{
    List<GameObject> tt2;
    public int ff1 = 1;
    public int ff2 = 2;
    public int ff3 = 3;
    Color black = new Color(0,0,0,255);
    Color white = new Color(255,255,255,255);
    void Awake(){
        //Time.timeScale = 1;
        tt2 = gameObject.GetChildren();
        if(Time.timeScale == ff1){
            tt2[0].GetComponent<Button>().interactable = false;
            tt2[0].transform.GetChild(0).GetComponent<Image>().color = black;
        } else if(Time.timeScale == ff2){
            tt2[1].GetComponent<Button>().interactable = false;
            tt2[1].transform.GetChild(0).GetComponent<Image>().color = black;
        } else {
            tt2[2].GetComponent<Button>().interactable = false;
            tt2[2].transform.GetChild(0).GetComponent<Image>().color = black;
        }
    }

    public void FF1(){
        Time.timeScale = ff1;
        tt2[0].GetComponent<Button>().interactable = false;
        tt2[1].GetComponent<Button>().interactable = true;
        tt2[2].GetComponent<Button>().interactable = true;

        tt2[0].transform.GetChild(0).GetComponent<Image>().color = black;
        tt2[1].transform.GetChild(0).GetComponent<Image>().color = white;
        tt2[2].transform.GetChild(0).GetComponent<Image>().color = white;

        AdjustPitch();
    }

    public void FF2(){
        Time.timeScale = ff2;
        tt2[0].GetComponent<Button>().interactable = true;
        tt2[1].GetComponent<Button>().interactable = false;
        tt2[2].GetComponent<Button>().interactable = true;

        tt2[0].transform.GetChild(0).GetComponent<Image>().color = white;
        tt2[1].transform.GetChild(0).GetComponent<Image>().color = black;
        tt2[2].transform.GetChild(0).GetComponent<Image>().color = white;

        AdjustPitch();
    }

    public void FF3(){
        Time.timeScale = ff3;
        tt2[0].GetComponent<Button>().interactable = true;
        tt2[1].GetComponent<Button>().interactable = true;
        tt2[2].GetComponent<Button>().interactable = false;

        tt2[0].transform.GetChild(0).GetComponent<Image>().color = white;
        tt2[1].transform.GetChild(0).GetComponent<Image>().color = white;
        tt2[2].transform.GetChild(0).GetComponent<Image>().color = black;

        AdjustPitch();
    }

    //adjust pitch and playback time
    //exclusions: UI objects, BGM, dialogue
    public void AdjustPitch(){
        Debug.Log(Time.timeScale);
        var sources = FindObjectsOfType<AudioSource>();
        foreach(AudioSource source in sources){
            if(source.transform.root.name != "Essentials"){
                source.pitch = Time.timeScale;
            }
        }
    }
}
