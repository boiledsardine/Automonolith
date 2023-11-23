using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public abstract class DialogueSystemBase : MonoBehaviour{
    [SerializeField] protected Canvas dialogueCanvas;
    [SerializeField] protected TMPro.TMP_Text nameText;
    [SerializeField] protected TMPro.TMP_Text dialogueText;

    protected Queue<string> dialogueLines;
    protected Queue<string> quizLines;
    
    [SerializeField] protected Animator panelAnimator;
    [SerializeField] protected Animator dialogueBoxAnimator;
    [SerializeField] protected Image leftSprite;
    [SerializeField] protected Image rightSprite;
    [SerializeField] protected ColorizerTheme theme;
    protected string currentSentence;
    protected Queue<Dialogue> dialogueBlocks;
    private bool isErrorDialogue = false;
    public bool ErrorDialogue{
        get { return isErrorDialogue; }
        set { isErrorDialogue = value; }
    }

    public void Awake(){
        dialogueLines = new Queue<string>();
        dialogueBlocks = new Queue<Dialogue>();
        quizLines = new Queue<string>();
    }

    public abstract void startDialogue(Conversation convoToLoad);
    public Color darkColor = new Color(60,60,60);

    //issue: types out the tags
    /*public IEnumerator typeSentence(string sentence){
        dialogueText.text = "";
        bool inColorTag = false;
        string endTag = "</color>";
        List<char> charList = new List<char>();
        foreach(char letter in sentence.ToCharArray()){
            if(inColorTag){
                int insertIndex = charList.Count - endTag.Length;
                charList.Insert(insertIndex, letter);
                dialogueText.text = ListToString(charList);
            } else if(!inColorTag && !(letter == '~' || letter == '`')){
                charList.Insert(dialogueText.text.Length, letter);
                dialogueText.text = ListToString(charList);
            }

            if(letter == '~'){
                foreach(char c in CodeColorizer.Colorize("", false, theme)){
                    charList.Add(c);
                }
                inColorTag = true;
            } else if(letter == '`'){
                inColorTag = false;
            }
            
            yield return null;
        }
    }*/

    //navigates to the next line on the list
    public abstract void nextLine();

    public void endDialogue(){
        Invoke("disableCanvas", 0.25f);
        
        dialogueLines?.Clear();
        dialogueBlocks?.Clear();

        currentSentence = null;

        dialogueBoxAnimator.SetBool("isOpen", false);
        panelAnimator.SetBool("isOpen", false);
        leftSprite.GetComponent<Image>().enabled = false;
        rightSprite.GetComponent<Image>().enabled = false;
       
        if(!isErrorDialogue){
            Debug.LogWarning("BROADCASTING ENDDIALOGUE");
            StoryManager stryMgr = FindObjectOfType<StoryManager>();
            stryMgr?.BroadcastMessage("DialogueEnd");

            TutorialBase tutBase = FindObjectOfType<TutorialBase>();
            tutBase?.BroadcastMessage("DialogueEnd");

            QuizLevel1 quizLvl = FindObjectOfType<QuizLevel1>();
            quizLvl?.BroadcastMessage("DialogueEnd");   
        } else {
            isErrorDialogue = false;
            return;
        }
    }

    public void enableCanvas(){
        dialogueCanvas.gameObject.SetActive(true);
    }

    public void disableCanvas(){
        dialogueCanvas.gameObject.SetActive(false);
    }

    public void npcHighlight(Dialogue dialogue){
        if(dialogue.showNpc){
            Image ImageLeft = leftSprite.GetComponent<Image>();
            Image ImageRight = rightSprite.GetComponent<Image>();
            switch(dialogue.npcPos){
                case 'L':
                    ImageLeft.enabled = true;
                    ImageLeft.sprite = dialogue.npcSprite;
                    ImageLeft.color = Color.white;
                    ImageRight.color = darkColor;
                    break;
                case 'R':
                    ImageRight.enabled = true;
                    ImageRight.sprite = dialogue.npcSprite;
                    ImageRight.color = Color.white;
                    ImageLeft.color = darkColor;
                    break;
                case 'B':
                    ImageLeft.color = Color.white;
                    ImageRight.color = Color.white;
                    break;
                case 'C':
                    ImageLeft.enabled = false;
                    ImageRight.enabled = false;
                    break;
            }
        } else {
            leftSprite.GetComponent<Image>().color = darkColor;
            rightSprite.GetComponent<Image>().color = darkColor;
        }
    }
}
