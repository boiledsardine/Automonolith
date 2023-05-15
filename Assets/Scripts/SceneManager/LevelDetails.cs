using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New LevelDetails", menuName = "Level Details")]
public class LevelDetails : ScriptableObject{
    public List<LevelDetail> levelDetails;
}

[System.Serializable]
public class LevelDetail{
    public LevelDetail(string levelName, string levelDesc){
        this.levelName = levelName;
        this.levelDesc = levelDesc;
    }

    public string levelName;
    
    [TextArea(3, 10)]
    public string levelDesc; 
}