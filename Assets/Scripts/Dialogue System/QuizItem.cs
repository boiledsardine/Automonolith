using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuizItem : Dialogue{
    public bool hasChoice;
    public string[] choices;
    public char correctAnswer;
}