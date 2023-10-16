using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Minigame Stage", menuName = "MGStage")]
public class MinigameStageInfo : ScriptableObject{
    public HelpArticle help;
    public Conversation hint;
    public string levelName;
    
    [TextArea(3,3)]
    public string levelDesc;
    
    [TextArea(3,3)]
    public string levelGoal;
    
    public List<Dialogue> startConvo, passConvo;

    [TextArea(10,20)]
    public string mainText;

    public List<MinigameWords> hidWords = new List<MinigameWords>();
    public List<string> wordBank;
    public List<DictInfo> levelDictionary = new List<DictInfo>();

    public void InitializeInfo(){
        foreach(var entry in hidWords){
            if(!wordBank.Contains(entry.hiddenText)){
                wordBank.Add(entry.hiddenText);
            }

            if(!entry.correctAnswers.Contains(entry.hiddenText)){
                entry.correctAnswers.Add(entry.hiddenText);
            }
        }
    }
}

[Serializable]
public struct MinigameWords{
    public string hiddenText;
    public List<string> correctAnswers;

    [TextArea(3,10)]
    public string mistakeMessage;
}