using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ObjectiveManager : MonoBehaviour {
    public static ObjectiveManager Instance { get; private set; }
    public int levelIndex;
    public MainObjectiveType mainObjective;
    public SecondaryObjectiveType secondaryObjective1;
    public SecondaryObjectiveType secondaryObjective2;
    public string primaryObjectiveText;
    public string secondObjective1Text;
    public string secondObjective2Text;
    public TMP_Text primObj, secObj1, secObj2, secObjHeader;
    public bool hasSecondObjs;
    public Color successColor = new Color(100f, 200f, 50f, 255f);
    public bool exitTouched = false;

    public void Awake(){ 
        if(Instance == null){
            Instance = this;
        } else {
            Destroy(gameObject);
        }

        primObj.text = string.Format(">{0}", primaryObjectiveText);
        
        if(hasSecondObjs){
            secObjHeader.gameObject.SetActive(true);
            secObj1.gameObject.SetActive(true);
            secObj1.text = string.Format(">{0}", secondObjective1Text);
            
            if(secObj2 != null){
                secObj2.gameObject.SetActive(true);
                secObj2.text = string.Format(">{0}", secondObjective2Text);
            }  
        }
    }

    public bool PrimaryObjective(){
        switch(mainObjective){
            case MainObjectiveType.CollectGems:
                GemPickup[] activeGems = FindObjectsOfType<GemPickup>();
                if(activeGems.Length == 0){
                    return true;
                } else {
                    return false;
                }
            case MainObjectiveType.FinishLevel:
                if(exitTouched) {
                    return true;
                } else {
                    return false;
                }
            case MainObjectiveType.None:
                return true;
            default:
                return false;
        }
    }
    public bool SecondObjective1(){
        switch(secondaryObjective1){
            case SecondaryObjectiveType.None:
                return true;
            default:
                return false;
        }
    }
    public bool SecondObjective2(){
        switch(secondaryObjective2){
            case SecondaryObjectiveType.None:
                return true;
            default:
                return false;
        }
    }

    public void Update(){
        if(PrimaryObjective()){
            primObj.color = successColor;
        }
        if(SecondObjective1()){
            secObj1.color = successColor;
        }
        if(SecondObjective2()){
            secObj2.color = successColor;
        }
    }
    
    public void LevelComplete(){
        if(PrimaryObjective()){
            //save editor state
            EditorSaveLoad.Instance.SaveEditorState();
            //save level
            SaveLevel(PrimaryObjective(), SecondObjective1(), SecondObjective2());
            //then return to main menu
            SceneManager.LoadScene("Main Menu");
            //destroy persistents
            DontDestroy[] persistents = FindObjectsOfType<DontDestroy>();
            foreach(DontDestroy obj in persistents){
                Destroy(obj.gameObject);
            }
        } else {
            Debug.Log("Objective isn't complete!");
        }
    }

    public void SaveLevel(bool goal1, bool goal2, bool goal3){
        LevelSaveLoad.Instance.SaveLevelInfo(levelIndex, goal1, goal2, goal3);
    }

    public enum MainObjectiveType{
        CollectGems,
        FinishLevel,
        None
    }

    public enum SecondaryObjectiveType{
        None,
        LineCount,
        StepCount
    }
}