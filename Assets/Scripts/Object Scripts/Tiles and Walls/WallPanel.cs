using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class WallPanel : MonoBehaviour{
    [SerializeField] string _storedText;
    [SerializeField] int _storedInt;
    [SerializeField] char _storedChar;
    [SerializeField] double _storedDouble;
    [SerializeField] bool _storedBool;

    [SerializeField] string[] _storedTextArr;
    [SerializeField] int[] _storedIntArr;
    [SerializeField] char[] _storedCharArr;
    [SerializeField] double[] _storedDoubleArr;
    [SerializeField] bool[] _storedBoolArr;
    public PanelType panelType;
    public bool isTalking;
    [SerializeField] private GameObject speechbubble;
    private GameObject bubble;

    public string storedText{
        get { return _storedText; }
        set { _storedText = value; }
    }

    public string[] storedTextArr{
        get { return _storedTextArr; }
        set { _storedTextArr = value; }
    }

    public int storedInt{
        get { return _storedInt; }
        set { _storedInt = value; }
    }

    public int[] storedIntArr{
        get { return _storedIntArr; }
        set { _storedIntArr = value; }
    }

    public bool storedBool{
        get { return _storedBool; }
        set { _storedBool = value; }
    }

    public bool[] storedBoolArr{
        get { return _storedBoolArr; }
        set { _storedBoolArr = value; }
    }

    public double storedDouble{
        get { return _storedDouble; }
        set { _storedDouble = value; }
    }

    public double[] storedDoubleArr{
        get { return _storedDoubleArr; }
        set { _storedDoubleArr = value; }
    }

    public char storedChar{
        get { return _storedChar; }
        set { _storedChar = value; }
    }

    public char[] storedCharArr{
        get { return _storedCharArr; }
        set { _storedCharArr = value; }
    }

    public bool randomize;
    public int[] valueRange;
    public string[] stringRange;

    void Awake(){
        if(randomize){
            System.Random rnd = new System.Random();
            switch(panelType){
                case PanelType.intPanel:
                    storedInt = valueRange[rnd.Next(valueRange.Length - 1)];
                break;
                case PanelType.stringPanel:
                    storedText = stringRange[rnd.Next(stringRange.Length - 1)];
                break;
            }
        }
    }

    void Update(){
        if(!isTalking){
            return;
        }

        if(bubble == null){
            bubble = Instantiate(speechbubble);
            var bubbleSet = bubble.GetComponent<SpeechBubble>();
            bubbleSet.boundObject = gameObject;
            bubbleSet.followMode = SpeechBubble.FollowMode.FollowBoundObject;
            bubbleSet.destroyTime = 0.25f;
            bubbleSet.yPos = 110f;
        } else {
            Destroy(bubble);
            bubble = Instantiate(speechbubble);
            var bubbleSet = bubble.GetComponent<SpeechBubble>();
            bubbleSet.boundObject = gameObject;
            bubbleSet.followMode = SpeechBubble.FollowMode.FollowBoundObject;
            bubbleSet.destroyTime = 0.25f;
            bubbleSet.yPos = 110f;
        }

        var speechText = bubble.transform.GetChild(0);
        if(panelType == PanelType.stringPanel){
            speechText.GetComponent<TextMesh>().text = storedText;
        } else if(panelType == PanelType.intPanel){
            speechText.GetComponent<TextMesh>().text = storedInt.ToString();
        }
    }
}

public enum PanelType{
    stringPanel,
    intPanel,
    boolPanel,
    charPanel,
    doublePanel,
    intArrPanel,
    stringArrPanel,
    boolArrPanel,
    charArrPanel,
    doubleArrPanel
}