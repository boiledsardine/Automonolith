using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuizItem : Dialogue{
    public static string[] help;

    public static Texture texture;

    public QuizItem() : base(help, "we", 'n', true, texture){
    }

    public bool hasChoice;
    public string[] choices;
    public char correctAnswer;
}