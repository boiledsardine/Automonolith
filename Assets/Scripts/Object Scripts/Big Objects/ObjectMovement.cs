using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommandControl;

public class ObjectMovement : MonoBehaviour, IMovement{
    private Vector3 originPos, targetPos;
    private char _dir;

    private ObjectEnvironment envirScript;

    private void Awake(){
        envirScript = GetComponent<ObjectEnvironment>();
    }

    public char dir{
        get { return _dir; }
        set { _dir = value; }
    }

    public IEnumerator Move(Vector3 moveDir){
        if(envirScript.neighborIsValid(dir) && !envirScript.checkForWalls(dir)){
            PlayMoveSound();

            float timeElapsed = 0;
            originPos = transform.position;
            targetPos = originPos + moveDir;
            while(timeElapsed < Globals.Instance.timeToMove){
                transform.position = Vector3.Lerp(originPos, targetPos,
                (timeElapsed / Globals.Instance.timeToMove));
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            transform.position = targetPos;
        } else {
            envirScript.tileUnder.isOccupied = true;
            Compiler.Instance.terminateExecution();
        }
    }

    public IEnumerator moveDown(){
        yield return StartCoroutine(Move(new Vector3(0f, 0f, -100f)));
    }

    public IEnumerator moveLeft(){
        yield return StartCoroutine(Move(new Vector3(-100f, 0f, 0f)));
    }

    public IEnumerator moveRight(){
        yield return StartCoroutine(Move(new Vector3(100f, 0f, 0f)));
    }

    public IEnumerator moveUp(){
        yield return StartCoroutine(Move(new Vector3(0f, 0f, 100f)));
    }

    void PlayMoveSound(){
        AudioSource source = GetComponent<AudioSource>();
        source.outputAudioMixerGroup = AudioPicker.Instance.environmentMixer;
        System.Random rnd = new System.Random();
        int maxIndex = AudioPicker.Instance.rockMove.Length;
        source.clip = AudioPicker.Instance.rockMove[rnd.Next(maxIndex)];
        source.Play();
    }
}