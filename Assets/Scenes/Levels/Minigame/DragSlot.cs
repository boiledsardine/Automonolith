using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Text.RegularExpressions;
using TMPro;

public class DragSlot : MonoBehaviour, IDropHandler{
    public int slotIndex = 0;
    public bool isCorrect = false;
    public List<string> correctAnswers;
    readonly string[] specialChars = {
        ";",
        "{",
        "}",
        "(",
        ")",
        "[",
        "]",
        "//",
        "_"
    };

    public void OnDrop(PointerEventData eventData){
        if(transform.childCount > 0){
            return;
        }
        
        GameObject dropObj = eventData.pointerDrag;
        var draggable = dropObj.GetComponent<DraggableItem>();
        draggable.ogPosition = transform.position;
        draggable.ogParent = gameObject.transform;

        PlayDropSound();
        //Invoke("ReformatText", 0.01f);
    }

    void PlayDropSound(){
        AudioSource source = MinigameManager.Instance.GetComponent<AudioSource>();
        System.Random rnd = new System.Random();
        int maxIndex = AudioPicker.Instance.boxDrop.Length;

        source.outputAudioMixerGroup = AudioPicker.Instance.minigameMixer;
        
        source.clip = AudioPicker.Instance.boxDrop[rnd.Next(maxIndex)];
        source.Play();
    }

    /*void Update(){
        MinigameManager.Instance.mainText.text = ReformatText(MinigameManager.Instance.stageInfo.mainText);
    }*/

    //this ain't it
    //should update only its thang
    int currWordIndex = 0;
    void ReformatText(){
        string result = "";
        var linesArr = MinigameManager.Instance.stageInfo.mainText.Split('\n');
        for(int i = 0; i < linesArr.Length; i++){
            if(string.IsNullOrWhiteSpace(linesArr[i])){
                result += "\n";
                continue;
            }

            string formattedLine = "";
            string[] wordsArr = linesArr[i].Split(' ');
            for(int j = 0; j < wordsArr.Length; j++){
                string currentWord = wordsArr[j];
                for(int k = 0; k < specialChars.Length; k++){
                    currentWord = currentWord.Replace(specialChars[k], " " + specialChars[k] + " ");
                }

                string[] spacedWord = currentWord.Split(' ');

                for(int k = 0; k < spacedWord.Length; k++){
                    spacedWord[k] = spacedWord[k].Replace("#", " ");
                    if(Regex.IsMatch(spacedWord[k], @"^_+$")){
                        //check each slot until it hits this one in particular
                        //check if it has a child
                        //if it does, resize the slot and the underlying text
                        if(currWordIndex == slotIndex && transform.childCount > 0){
                            //draggable item box
                            var dragItem = transform.GetChild(0);
                            if(dragItem.childCount > 0){
                                //draggable item text
                                var dragItemChild = dragItem.GetChild(0).gameObject;
                                var textObject = dragItemChild.GetComponent<TMP_Text>();
                                //IDE says ff line is an error but Unity's compiler is chill
                                if(textObject != null){
                                    spacedWord[k] = "<link=\"slot\">" + WordToUnderscores(textObject.text) + "</link>";
                                    GetComponent<SetSlotSize>().setText = textObject.text;
                                    GetComponent<SetSlotSize>().Resize();
                                }
                                Debug.Log(spacedWord[k]);
                            }
                        } else {
                            spacedWord[k] = "<link=\"slot\">___</link>";
                        }
                        currWordIndex++;
                    } else {
                        spacedWord[k] = "<link=\"key\">" + spacedWord[k] + "</link>";
                    }
                }
                formattedLine += ArrayToString(spacedWord) + " ";
            }
            result += formattedLine.Trim() + "\n";
        }
        currWordIndex = 0;
        MinigameManager.Instance.mainText.text = result;
        MinigameManager.Instance.FindLinks();
    }

    string ArrayToString(string[] array){
        string s = "";
        foreach(string str in array){
            s += str;
        }
        return s;
    }

    string WordToUnderscores(string text){
        string result = "";
        for(int i = 0; i < text.Length; i++){
            result += "_";
        }
        return result;
    }
}
