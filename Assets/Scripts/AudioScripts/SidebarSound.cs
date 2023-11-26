using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SidebarSound : MonoBehaviour{
    AudioSource source;
    void Awake(){
        source = GetComponent<AudioSource>();
    }

    public void PlayOpenSound(){
        source.clip = AudioPicker.Instance.sidebarOpen;

        source.outputAudioMixerGroup = AudioPicker.Instance.swooshMixer;

        source.Play();
    }

    public void PlayCloseSound(){
        source.clip = AudioPicker.Instance.sidebarClose;

        source.outputAudioMixerGroup = AudioPicker.Instance.swooshMixer;

        source.Play();
    }
}
