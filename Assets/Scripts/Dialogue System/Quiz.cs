using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Quiz", menuName = "Quiz")]
public class Quiz : ScriptableObject{
    public Conversation startConvo, passConvo, failConvo;
    public int threshold1, threshold2, threshold3, items;
    public List<QuizItem> quizBlocks;
}

[System.Serializable]
public class QuizItem{
    public QuizItem(string question, string npcName, bool showNpc, Sprite npcSprite, bool hasChoice, string[] choices, Letters correctAnswer){
        this.question = question;
        this.npcName = npcName;
        this.showNpc = showNpc;
        this.npcSprite = npcSprite;
        this.hasChoice = hasChoice;
        this.choices = choices;
        this.correctAnswer = correctAnswer;
    }
    
    [TextArea(10, 10)]
    public string question;
    public string npcName;
    public bool showNpc;
    public Sprite npcSprite;
    public bool hasChoice;
    public string[] choices;
    public Letters correctAnswer;
}