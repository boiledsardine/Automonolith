using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class AlmanacManager : MonoBehaviour{
    public TMP_Text titleText, mainText, exampleText;
    public AlmanacGroup[] groups;
    List<AlmanacEntry> entries;
    List<string> addedEntries;
    public Animator helpPanelAnim;
    public Canvas helpCanvas;
    public GameObject buttonContainer;
    public GameObject almanacButton;
    public ColorizerTheme theme;
    public ScrollRect scrollView;

    void Start(){
        string content = File.ReadAllText(Application.dataPath + "/Saves/SaveLevels.json");

        if(string.IsNullOrEmpty(content) || content == "{}"){
            Debug.LogAssertion("Empty save entry");
            return;
        }

        var savedLevels = JsonHelper.FromJson<LevelInfo>(content).ToList();
        
        entries = new List<AlmanacEntry>();
        addedEntries = new List<string>();

        //tutorial level indices: 0, 1 / 4, 5 / 9, 10 / 13 / 17
        AddEntries(groups[0]);
        if(savedLevels[4].isUnlocked){
            AddEntries(groups[1]);
        }
        if(savedLevels[9].isUnlocked){
            AddEntries(groups[2]);
            AddEntries(groups[3]);
        }
        if(savedLevels[13].isUnlocked){
            AddEntries(groups[4]);
        }
        if(savedLevels[17].isUnlocked){
            AddEntries(groups[5]);
        }

        for(int i = 0; i < entries.Count; i++){
            var almButton = Instantiate(almanacButton);
            var textObj = almButton.transform.GetChild(0);
            textObj.gameObject.GetComponent<TMP_Text>().text = entries[i].articleName;
            var button = almButton.GetComponent<AlmanacButton>();
            button.managerName = gameObject.name;
            button.index = i;
            almButton.transform.SetParent(buttonContainer.transform);
            almButton.transform.localScale = new Vector3(1,1,1);
        }
        buttonContainer.GetComponent<ResizeScrollObject>().Resize();
        //scrollView.FocusOnItem(buttonContainer.transform.GetChild(0).GetComponent<RectTransform>());
    }

    void AddEntries(AlmanacGroup group){
        foreach(AlmanacEntry entry in group.entries){
            if(!addedEntries.Contains(entry.articleName)){
                entries.Add(entry);
                addedEntries.Add(entry.articleName);
            }
        }
    }

    public void LoadEntry(int index){
        Debug.Log(index);
        titleText.text = entries[index].articleName;
        mainText.text = CodeColorizer.Colorize(entries[index].articleText, true, theme);
        exampleText.text = CodeColorizer.Colorize(entries[index].articleExample, true, theme);
    }

    public void OpenHelp(){
        helpCanvas.gameObject.SetActive(true);
        helpPanelAnim.SetBool("isOpen", true);
    }

    public void CloseHelp(){
        helpPanelAnim.SetBool("isOpen", false);
        Invoke("disableHelp", 0.25f);
    }

    void DisableHelp(){
        helpCanvas.gameObject.SetActive(false);
    }
}
