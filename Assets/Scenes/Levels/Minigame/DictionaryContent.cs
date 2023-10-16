using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New DictInfo", menuName = "DictInfo")]
public class DictionaryContent : ScriptableObject{
    public List<DictInfo> content;
}

[Serializable]
public struct DictInfo{
    public DictInfo(string keyword, string description){
        this.keyword = keyword;
        this.description = description;
    }

    public string keyword;

    [TextArea(3,10)]
    public string description;
}
