using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timescaler : MonoBehaviour{
    List<GameObject> tt2;
    public int ff1 = 1;
    public int ff2 = 3;
    public int ff3 = 5;
    void Awake(){
        //Time.timeScale = 1;

        tt2 = gameObject.GetChildren();
    }

    public void FF1(){
        Time.timeScale = ff1;
        tt2[1].GetComponent<Button>().interactable = false;
        tt2[2].GetComponent<Button>().interactable = true;
        tt2[3].GetComponent<Button>().interactable = true;

        tt2[1].transform.GetChild(0).GetComponent<Image>().color = new Color(255,255,255,255);
        tt2[2].transform.GetChild(0).GetComponent<Image>().color = new Color(0,0,0,255);
        tt2[3].transform.GetChild(0).GetComponent<Image>().color = new Color(0,0,0,255);
    }

    public void FF2(){
        Time.timeScale = ff2;
        tt2[1].GetComponent<Button>().interactable = true;
        tt2[2].GetComponent<Button>().interactable = false;
        tt2[3].GetComponent<Button>().interactable = true;

        tt2[1].transform.GetChild(0).GetComponent<Image>().color = new Color(0,0,0,255);
        tt2[2].transform.GetChild(0).GetComponent<Image>().color = new Color(255,255,255,255);
        tt2[3].transform.GetChild(0).GetComponent<Image>().color = new Color(0,0,0,255);
    }

    public void FF3(){
        Time.timeScale = ff3;
        tt2[1].GetComponent<Button>().interactable = true;
        tt2[2].GetComponent<Button>().interactable = true;
        tt2[3].GetComponent<Button>().interactable = false;

        tt2[1].transform.GetChild(0).GetComponent<Image>().color = new Color(0,0,0,255);
        tt2[2].transform.GetChild(0).GetComponent<Image>().color = new Color(0,0,0,255);
        tt2[3].transform.GetChild(0).GetComponent<Image>().color = new Color(255,255,255,255);
    }
}
