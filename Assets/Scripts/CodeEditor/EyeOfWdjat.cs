using System;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CommandControl;

public class EyeOfWdjat : MonoBehaviour
{
    [SerializeField] private Text input;
    public static EyeOfWdjat Instance { get; private set; }

    private void Awake(){
        if(Instance == null){
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    public void Run(){
        Reset.Instance.Exterminatus();
        StartCoroutine(readLines());
    }

    private IEnumerator readLines(){
        yield return new WaitForSeconds(0.5f);
        char[] delim = new[] { '\r', '\n' };
        string[] lines = normalizer().Split(delim, StringSplitOptions.RemoveEmptyEntries);
        foreach(string read in lines){
            understanding(read);
        }
        Bot.Instance.execute();
    }

    private string normalizer(){
        string normalString = input.text;
        normalString = Regex.Replace(normalString, "\\s+\\{", "{");
        normalString = normalString.Replace("\t", "");
        normalString = Regex.Replace(normalString, "\n+\\{", "{");
        return normalString;
    }

    private void understanding(string str){
        if(str == "Bot.moveUp();"){
            Bot.moveUp();
        } else if(str == "Bot.moveDown();"){
            Bot.moveDown();
        } else if(str == "Bot.moveLeft();"){
            Bot.moveLeft();
        } else if(str == "Bot.moveRight();"){
            Bot.moveRight();
        } else if(Regex.IsMatch(str, @"^Bot\.moveUp\([0-9]+\);$")){
            Match m = Regex.Match(str, @"[0-9]+");
            Bot.moveUp(int.Parse(m.Value));
        } else if(Regex.IsMatch(str, @"^Bot\.moveDown\([0-9]+\);$")){
            Match m = Regex.Match(str, @"[0-9]+");
            Bot.moveDown(int.Parse(m.Value));
        } else if(Regex.IsMatch(str, @"^Bot\.moveLeft\([0-9]+\);$")){
            Match m = Regex.Match(str, @"[0-9]+");
            Bot.moveLeft(int.Parse(m.Value));
        } else if(Regex.IsMatch(str, @"^Bot\.moveRight\([0-9]+\);$")){
            Match m = Regex.Match(str, @"[0-9]+");
            Bot.moveRight(int.Parse(m.Value));
        } else if(str == "Bot.turnUp();"){
            Bot.turnUp();
        } else if(str == "Bot.turnDown();"){
            Bot.turnDown();
        } else if(str == "Bot.turnLeft();"){
            Bot.turnLeft();
        } else if(str == "Bot.turnRight();"){
            Bot.turnRight();
        } else if(str == "Bot.hold();"){
            Bot.hold();
        } else if(str == "Bot.release();"){
            Bot.release();
        }
    }
}
