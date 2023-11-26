 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DialogueManager : DialogueSystemBase, IPointerClickHandler{
    public static DialogueManager Instance;
    public Button nextButton, skipButton;
    public bool allowLineSkipping;
    public bool allowFullSkipping;
    public bool nextLineTimer;
    bool currentSentenceDone = false;
    [SerializeField] TMPro.TMP_Text nextLineCountdown;
    AudioSource source;
    public int talkFrequency;
    string talkingChar;
    public float arthurPitch = 0.75f, morganPitch = 2.25f, nimuePitch = 1.55f, merlinPitch = 0.5f, gawainPitch = 1f, defaultPitch = 1f;

    new void Awake(){
        base.Awake();

        if(Instance == null){
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }

        source = transform.Find("TalkingSource").GetComponent<AudioSource>();
    }

    void Start(){
        skipButton.gameObject.SetActive(allowFullSkipping);
    }

    public override void startDialogue(Conversation convoToLoad){
        dialogueBlocks.Clear();

        foreach(Dialogue d in convoToLoad.dialogueBlocks){
            dialogueBlocks.Enqueue(d);
        }
        
        dialogueBoxAnimator.SetBool("isOpen", true);
        panelAnimator.SetBool("isOpen", true);
        PlayOpenSound();

        loadDialogue();

        StopAllCoroutines();
        currentSentence = dialogueLines.Dequeue();
        currentSentenceDone = false;
        StartCoroutine(typeSentence(currentSentence));
    }

    public IEnumerator typeSentence(string sentence){
        //next two lines disable colorizer by removing ~` tags
        //don't @ me i'm too lazy to remove the tags from each dialogue line
        sentence = sentence.Replace("~", "");
        sentence = sentence.Replace("`", "");

        dialogueText.text = "";
        int num = 0;
        foreach(char letter in CodeColorizer.Colorize(sentence, false, theme).ToCharArray()){
            num++;
            if(num == talkFrequency){
                PlayTalkingSound();
                num = 0;
            }
            dialogueText.text += letter;
            yield return null;
        }
        
        PlayDoneSound();
        StartCoroutine(ActivateNextLine());
    }

    void PlayTalkingSound(){
        AdjustPitch();

        float globalVolume = GlobalSettings.Instance.sfxVolume;
        float multiplier = AudioPicker.Instance.talkingVolume;
        source.volume = globalVolume * multiplier;

        source.clip = AudioPicker.Instance.talkingSound;
        source.Play();
    }

    void AdjustPitch(){
        switch(talkingChar){
            case "Arthur":
                source.pitch = arthurPitch;
            break;
            case "Morgan":
                source.pitch = morganPitch;
            break;
            case "G4wain":
                source.pitch = gawainPitch;
            break;
            case "Nimue":
                source.pitch = nimuePitch;
            break;
            case "Merlin":
                source.pitch = merlinPitch;
            break;
            default:
                source.pitch = defaultPitch;
            break;
        }
    }

    float lastPressTime = 0;
    public override void nextLine(){
        //makes it so you can't spamclick nextLine easily
        float pressTime = Time.fixedTime;
        if(Mathf.Abs(pressTime - lastPressTime) < 0.25f) return;

        lastPressTime = Time.fixedTime;
        if(dialogueLines.Count == 0){
            //check if there's another block in the queue
            //if yes, end dialogue
            //if no, load the next one
            if(gameObject.activeInHierarchy && dialogueBlocks.Count == 0){
                endDialogue();
                return;
            } else {
                loadDialogue();
            }
        }
        nextButton.gameObject.SetActive(false);

        StopAllCoroutines();
        currentSentence = dialogueLines.Dequeue();
        currentSentenceDone = false;
        StartCoroutine(typeSentence(currentSentence));
    }

    public void loadDialogue(){
        Dialogue dialogue = dialogueBlocks.Dequeue();
        dialogueLines.Clear();

        npcHighlight(dialogue);

        nameText.text = dialogue.npcName;
        talkingChar = dialogue.npcName;

        foreach(string s in dialogue.lines){
            dialogueLines.Enqueue(s);
        }
    }

    //autocomplete
    void ForceDialogueComplete(){
        StopAllCoroutines();
        dialogueText.text = CodeColorizer.Colorize(currentSentence, false, theme);

        PlayDoneSound();
        StartCoroutine(ActivateNextLine());
    }

    //full skip
    public void SkipDialogue(){
        StopAllCoroutines();
        source.Stop();
        dialogueBlocks.Clear();
        dialogueLines.Clear();
        endDialogue();
    }

    //only here because i'm evil and want to keep the player *gasp* READING
    //sets a countdown after each dialogue line which prevents the player from going forward
    //not currently in use but it's here if my sadism flares up
    IEnumerator ActivateNextLine(){
        currentSentenceDone = true;
        if(nextLineTimer){
            nextButton.gameObject.SetActive(true);
            nextButton.interactable = false;
            nextLineCountdown.transform.gameObject.SetActive(true);

            float countdown = 4;
            while(countdown > 1){
                countdown -= Time.fixedDeltaTime;
                nextLineCountdown.text = Mathf.FloorToInt(countdown).ToString();
                yield return null;
            }

            nextButton.interactable = true;
            nextLineCountdown.transform.gameObject.SetActive(false);
        } else {
            nextButton.gameObject.SetActive(true);
        }
    }

    void PlayDoneSound(){
        source.pitch = 0.75f;
        
        float globalVolume = GlobalSettings.Instance.sfxVolume;
        float multiplier = AudioPicker.Instance.talkingVolume;
        source.volume = globalVolume * multiplier;
        
        source.clip = AudioPicker.Instance.talkingDone;
        source.Play();
    }

    public void OnPointerClick(PointerEventData eventData){
        if(currentSentenceDone && nextButton.interactable){
            nextButton.onClick.Invoke();
        } else {
            if(allowLineSkipping){
                ForceDialogueComplete();
            }
        }
    }
}
