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

        float globalVolume = GlobalSettings.Instance.sfxVolume;
        float multiplier = AudioPicker.Instance.menuSwooshVolume;
        source.volume = globalVolume * multiplier;

        source.Play();
    }

    public void PlayCloseSound(){
        source.clip = AudioPicker.Instance.sidebarClose;

        float globalVolume = GlobalSettings.Instance.sfxVolume;
        float multiplier = AudioPicker.Instance.menuSwooshVolume;
        source.volume = globalVolume * multiplier;

        source.Play();
    }
}
