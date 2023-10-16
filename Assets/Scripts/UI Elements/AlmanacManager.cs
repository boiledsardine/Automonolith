using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AlmanacManager : MonoBehaviour{
    public TMP_Text titleText, mainText, exampleText;
    public AlmanacEntry[] entries;
    public Animator helpPanelAnim;
    public Canvas helpCanvas;
    public GameObject buttonContainer;
    public GameObject almanacButton;

    void Start(){
        for(int i = 0; i < entries.Length; i++){
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
        OpenHelp();
    }

    public void LoadEntry(int index){
        Debug.Log(index);
        titleText.text = entries[index].articleName;
        mainText.text = entries[index].articleText;
        exampleText.text = entries[index].articleExample;
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
