using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Help Article", menuName = "Help Article")]
public class HelpArticle : ScriptableObject{
    public List<Article> articleBlocks;
}

[System.Serializable]
public class Article{
    public string articleName;

    [TextArea(3, 10)]
    public string articleText;
}