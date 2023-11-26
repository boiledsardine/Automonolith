using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelpManager : MonoBehaviour{
    public bool disableSidebar;
    public string disableTitle;
    [TextArea(5,10)]
    public string disableText;
    public Canvas helpCanvas;
    public TMPro.TMP_Text articleTitle, articleText, articleExample, pageNumbers;
    public Button leftButton, rightButton;
    public Animator helpPanelAnim;

    public AlmanacGroup almGroup;
    int currentIndex = 0;
    int lastLoadedIndex = 0;
    public ColorizerTheme theme;

    public void openHelp(){
        helpCanvas.gameObject.SetActive(true);
        helpPanelAnim.SetBool("isOpen", true);

        SendMessage("PlayOpenSound");

        if(disableSidebar){
            articleTitle.text = disableTitle;
            articleText.text = disableText;
            articleExample.text = "";
            return;
        }

        loadArticle(lastLoadedIndex);
    }

    public void closeHelp(){
        lastLoadedIndex = currentIndex;
        helpPanelAnim.SetBool("isOpen", false);

        SendMessage("PlayCloseSound");

        Invoke("disableHelp", 0.25f);
    }

    void disableHelp(){
        helpCanvas.gameObject.SetActive(false);
    }

    public void loadArticle(int loadIndex){
        articleTitle.text = almGroup.entries[loadIndex].articleName;
        articleText.text = CodeColorizer.Colorize(almGroup.entries[loadIndex].articleText, true, theme);
        articleExample.text = CodeColorizer.Colorize(almGroup.entries[loadIndex].articleExample, true, theme);
        pageNumbers.text = string.Format("{0} / {1}", loadIndex + 1, almGroup.entries.Count);

        leftButton.interactable = true;
        rightButton.interactable = true;

        if(loadIndex == 0) {
            leftButton.interactable = false;
        }

        if(loadIndex + 1 == almGroup.entries.Count){
            rightButton.interactable = false;
        }
    }

    public void nextPage(){
        loadArticle(currentIndex + 1);
        currentIndex++;
    }

    public void lastPage(){
        loadArticle(currentIndex - 1);
        currentIndex--;
    }
}
