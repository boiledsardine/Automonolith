using System;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EyeOfWdjat : MonoBehaviour
{
    public Text input;
    public static EyeOfWdjat Instance { get; private set; }

    public void Awake(){
        if(Instance == null){
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    public void startReading(){
        char[] delims = new[] { '\r', '\n' };
        string[] readLines = input.text.Split(delims, StringSplitOptions.RemoveEmptyEntries);
        foreach(string a in readLines){
            Debug.Log(a);
        }
    }
}
