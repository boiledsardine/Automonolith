using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelpManager : MonoBehaviour{
    public Canvas helpCanvas;
    public TMPro.TMP_Text articleTitle, articleText, pageNumbers;
    public Button leftButton, rightButton;
    public Animator helpPanelAnim;

    public HelpArticle helpArticle;
    int currentIndex = 0;
    int lastLoadedIndex = 0;

    public void openHelp(){
        helpCanvas.gameObject.SetActive(true);
        loadArticle(lastLoadedIndex);
        helpPanelAnim.SetBool("isOpen", true);
    }

    public void closeHelp(){
        lastLoadedIndex = currentIndex;
        helpPanelAnim.SetBool("isOpen", false);
        Invoke("disableHelp", 0.25f);
    }

    void disableHelp(){
        helpCanvas.gameObject.SetActive(false);
    }

    public void loadArticle(int loadIndex){
        articleTitle.text = helpArticle.articleBlocks[loadIndex].articleName;
        articleText.text = helpArticle.articleBlocks[loadIndex].articleText;
        pageNumbers.text = string.Format("{0} / {1}", loadIndex + 1, helpArticle.articleBlocks.Count);

        leftButton.interactable = true;
        rightButton.interactable = true;

        if(loadIndex == 0) {
            leftButton.interactable = false;
        }

        if(loadIndex + 1 == helpArticle.articleBlocks.Count){
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
