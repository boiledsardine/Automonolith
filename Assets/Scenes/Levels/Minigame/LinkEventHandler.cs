using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using System.Linq;

public class LinkEventHandler : MonoBehaviour{
    [SerializeField] TMP_Text dictName, dictText;

    Dictionary<string,string> dictContent = new Dictionary<string,string>();

    void Awake(){
        //add contents of general dictionary
        foreach(var entry in ReadDictionaryFromJSON()){
            if(!dictContent.ContainsKey(entry.keyword)){
                dictContent.Add(entry.keyword, entry.description);
            } else {
                Debug.LogWarning("Not added: " + entry.keyword);
            }
        }

        //add contents of level-specific dictionary
        foreach(var entry in MinigameManager.Instance.stageInfo.levelDictionary){
            if(!dictContent.ContainsKey(entry.keyword)){
                dictContent.Add(entry.keyword, entry.description);
            } else {
                Debug.LogWarning("Not added: " + entry.keyword);
            }
        }
    }

    List<DictInfo> ReadDictionaryFromJSON(){
        string dictFile = Application.dataPath + "/Resources/MinigameDictionary.json";

        if(!File.Exists(dictFile)){
            Debug.LogAssertion("No minigame dictionary found");
            DictInfo[] emptyEntry = {new DictInfo("???", "???")};
            return emptyEntry.ToList();
        }
        
        string content = File.ReadAllText(dictFile);

        if(string.IsNullOrEmpty(content) || content == "{}"){
            Debug.LogAssertion("Empty dictionary");
            DictInfo[] emptyEntry = {new DictInfo("???", "???")};
            return emptyEntry.ToList();
        }

        return JsonHelper.FromJson<DictInfo>(content).ToList();
    }

    void OnEnable(){
        TMPLinkHandler.OnClickedOnLinkEvent += SetDictText;
        TextItem.OnClickedOnLinkEvent += SetDictText;
    }

    void OnDisable(){
        TMPLinkHandler.OnClickedOnLinkEvent -= SetDictText;
        TextItem.OnClickedOnLinkEvent -= SetDictText;
    }

    void SetDictText(string keyword){
        dictName.text = keyword;
        dictText.text = dictContent.ContainsKey(keyword) ? dictContent[keyword] : "entry not found";
    }
}
