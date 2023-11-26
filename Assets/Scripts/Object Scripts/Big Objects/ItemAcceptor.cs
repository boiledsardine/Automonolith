using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemAcceptor : ObjectBase, IActivator{
    private ObjectEnvironment envirScript;
    public int itemLimit;
    public int itemCount = 0;
    public Material[] material;
    public GameObject boundObject;
    private GameObject cube;
    private MeshRenderer meshRend;
    [SerializeField] private GameObject speechbubble;
    GameObject bubble = null;
    bool isAccepting = true;
    public CubeColor cubeColor;
    AudioSource source;
    
    string color{
        get{
            switch(cubeColor){
                case CubeColor.Blue:
                    return "blue";
                case CubeColor.Green:
                    return "green";
                default:
                    return "red";
            }
        }
    }

    void Awake(){
        isMovable = false;
        envirScript = gameObject.GetComponent<ObjectEnvironment>();
        //envirScript.tileUnder.isOccupied = true;
        meshRend = GetComponent<MeshRenderer>();
        source = GetComponent<AudioSource>();
    }

    void Update(){
        if(!isAccepting || cubeColor == CubeColor.Black){
            return;
        }

        if(bubble == null){
            bubble = Instantiate(speechbubble);
            var bubbleSet = bubble.GetComponent<SpeechBubble>();
            bubbleSet.boundObject = gameObject;
            bubbleSet.followMode = SpeechBubble.FollowMode.FollowBoundObject;
            bubbleSet.destroyTime = 0.25f;
        } else {
            Destroy(bubble);
            bubble = Instantiate(speechbubble);
            var bubbleSet = bubble.GetComponent<SpeechBubble>();
            bubbleSet.boundObject = gameObject;
            bubbleSet.followMode = SpeechBubble.FollowMode.FollowBoundObject;
            bubbleSet.destroyTime = 0.25f;
        }

        var speechText = bubble.transform.GetChild(0);
        int remaining = itemLimit - itemCount;
        speechText.GetComponent<TextMesh>().text = remaining.ToString();
    }

    void OnTriggerEnter(Collider col){
        if(col.transform.gameObject.name != "CubeKey"){
            return;
        }
        
        cube = col.transform.gameObject;
        if(cubeColor != CubeColor.Black && cube.GetComponent<CubePickup>().color == color){
            StartCoroutine(ConsumeCube(false));   
        } else if(cubeColor == CubeColor.Black){
            StartCoroutine(ConsumeCube(true));
        }
    }

    IEnumerator ConsumeCube(bool destroy){
        yield return new WaitForSeconds(Globals.Instance.timePerStep);
        cube.GetComponent<Animator>().SetBool("cubeExists", false);
        if(!destroy){
            itemCount++;
            CheckItemCount();
        }
        Invoke("DestroyCube", 0.25f);
    }

    void DestroyCube(){
        Destroy(cube);
    }

    bool hasActivated = false;
    void CheckItemCount(){
        if(itemCount >= itemLimit){
            if(hasActivated){
                PlayUpSound();
            }
            meshRend.materials = material;
            isAccepting = false;
            if(boundObject != null && !hasActivated){
                hasActivated = true;
                PlayFullSound();
                boundObject.GetComponent<IActivate>().activate();
            }
        } else {
            PlayUpSound();
        }
    }

    void PlayUpSound(){
        source.volume = GlobalSettings.Instance.sfxVolume;
        System.Random rnd = new System.Random();
        int maxIndex = AudioPicker.Instance.acceptorUp.Length;
        source.clip = AudioPicker.Instance.acceptorUp[rnd.Next(maxIndex)];
        source.Play();
    }

    void PlayFullSound(){
        source.volume = GlobalSettings.Instance.sfxVolume;
        source.clip = AudioPicker.Instance.acceptorFull;
        source.Play();
    }

    public bool IsActive(){
        return hasActivated;
    }

    public enum CubeColor{
        Blue,
        Green,
        Red,
        Black
    }
}
