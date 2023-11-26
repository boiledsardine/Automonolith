using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizUnlocker : MonoBehaviour{
    public int starCount = 0;
    public int[] starThresholds;
    public Conversation quizLockConvo;
    public Canvas dialogueCanvas;
    public TMPro.TMP_Text starCountText;

    public void CountStars(){
        foreach(LevelInfo level in LevelSaveLoad.Instance.savedLevels){
            if(level.star1){
                starCount++;
            }

            if(level.star2){
                starCount++;
            }

            if(level.star3){
                starCount++;
            }
        }

        starCountText.text = "x" + starCount;
    }

    public bool CheckStars(int loadIndex){
        //intercept signal from play button
        switch(loadIndex){
            case 8:
                if(starCount >= starThresholds[0]){
                    return true;
                } else {
                    StartLockDialogue(0);
                    return false;
                }
            case 16:
                if(starCount >= starThresholds[1]){
                    return true;
                } else {
                    StartLockDialogue(1);
                    return false;
                }
            case 20:
                if(starCount >= starThresholds[2]){
                    return true;
                } else {
                    StartLockDialogue(2);
                    return false;
                }
            default:
            return false;
        }
    }

    void StartLockDialogue(int index){
        quizLockConvo.dialogueBlocks[0].lines[0] = string.Format("Looks like this floor won't open until you have {0} green lights collected... So far, you have {1}.", starThresholds[index], starCount);
        dialogueCanvas.gameObject.SetActive(true);
        DialogueManager.Instance.startDialogue(quizLockConvo);
    }
}
