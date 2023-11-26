using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotKiller : MonoBehaviour, IActivate{
    GameObject playerCharacter;
    AudioSource source;

    void Awake(){
        source = GetComponent<AudioSource>();
    }

    public void activate(){
        Compiler.Instance.terminateExecution();

        source.volume = GlobalSettings.Instance.sfxVolume;
        source.clip = AudioPicker.Instance.trap;
        source.Play();

        playerCharacter.GetComponent<Animator>().SetBool("ded", true);
        Invoke("DestroyBot", 1f);
    }

    public void deactivate(){
        //do nothing
    }

    void Start(){
        playerCharacter = GameObject.Find("PlayerCharacter");
    }

    void DestroyBot(){
        PlayBotKill();
        Destroy(playerCharacter);
    }

    void PlayBotKill(){
        source.volume = GlobalSettings.Instance.sfxVolume;
        System.Random rnd = new System.Random();
        int maxIndex = AudioPicker.Instance.botDead.Length;
        source.clip = AudioPicker.Instance.botDead[rnd.Next(maxIndex)];
        source.Play();
    }
}
