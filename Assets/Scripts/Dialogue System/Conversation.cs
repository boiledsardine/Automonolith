using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Conversation", menuName = "Conversation")]
public class Conversation : ScriptableObject{
    public List<Dialogue> dialogueBlocks;
    public int LineCount(){
        int lineCount = 0;
        foreach(Dialogue block in dialogueBlocks){
            foreach(string line in block.lines){
                lineCount++;
            }
        }
        return lineCount;
    }
}

[System.Serializable]
public class Dialogue{
    public Dialogue(string[] lines, string npcName, char npcPos, bool showNpc, Sprite npcSprite){
        this.lines = lines;
        this.npcName = npcName;
        this.npcPos = npcPos;
        this.showNpc = showNpc;
        this.npcSprite = npcSprite;
    }

    public string npcName;
    public bool showNpc;
    public char npcPos;
    public Sprite npcSprite;

    [TextArea(3, 10)]
    public string[] lines;
}