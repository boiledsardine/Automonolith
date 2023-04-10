using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue{
    public Dialogue(string[] lines, string npcName, char npcPos, bool showNpc, Texture npcSprite){
        this.lines = lines;
        this.npcName = npcName;
        this.npcPos = npcPos;
        this.showNpc = showNpc;
        this.npcSprite = npcSprite;
    }

    public string npcName;
    public bool showNpc;
    public char npcPos;
    public Texture npcSprite;

    [TextArea(3, 10)]
    public string[] lines;
}