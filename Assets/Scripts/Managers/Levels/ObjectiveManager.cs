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
    public ObjectiveBase objScript;
    public Conversation failDialogue;
    public bool dontUpdate = false;

    public void Awake(){ 
        if(Instance == null){
            Instance = this;
        } else {
            Destroy(gameObject);
        }

        primObj.text = string.Format(">{0}", primaryObjectiveText);
        
        if(hasSecondObjs){
            secObjHeader.transform.gameObject.SetActive(true);
            secObj1.transform.gameObject.SetActive(true);
            secObj1.text = string.Format(">{0}", secondObjective1Text);
            
            if(secObj2.text != null){
                secObj2.transform.gameObject.SetActive(true);
                secObj2.text = string.Format(">{0}", secondObjective2Text);
            }  
        }
    }

    void Start(){
        levelIndex = LevelSaveLoad.Instance.indexHolder;
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
        if(dontUpdate){
            return;
        }
        
        if(objScript == null){
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
    }
    
    public void LevelComplete(){   
        if(objScript == null && PrimaryObjective()){
            Postlevel(PrimaryObjective(), SecondObjective1(), SecondObjective2());
        } else if(objScript != null && objScript.IsComplete()){
            Postlevel(objScript.Objective1(), objScript.Objective2(), objScript.Objective3());
        } else {
            DialogueManager.Instance.startDialogue(failDialogue);
        }
    }

    void Postlevel(bool obj1, bool obj2, bool obj3){
        AudioSource source = GetComponent<AudioSource>();
        source.volume = GlobalSettings.Instance.sfxVolume;
        source.clip = AudioPicker.Instance.successFanfare;
        source.Play();

        var maptacks = GameObject.Find("Maptacks");

        //destroy maptacks
        if(maptacks != null){
            Destroy(maptacks);
        }
        
        //save editor state
        EditorSaveLoad.Instance.SaveEditorState();
        //save level
        SaveLevel(obj1, obj2, obj3);
        
        //open postlevel
        Time.timeScale = 1;
        var postlevelCanvas = FindObjectOfType<PostlevelCanvas>(true);
        postlevelCanvas.gameObject.SetActive(true);

        PostlevelCanvas.Instance.OpenCanvas();
        PostlevelCanvas.Instance.SetStars(obj1, obj2, obj3);
    }

    public void SaveLevel(bool goal1, bool goal2, bool goal3){
        LastSceneHolder.Instance.SetLastScene();
        LevelSaveLoad.Instance.EndLevelSave(levelIndex, goal1, goal2, goal3, true);
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