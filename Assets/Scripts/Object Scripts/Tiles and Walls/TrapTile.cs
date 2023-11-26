using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommandControl;

public class TrapTile : TileBase {
    public bool isHidden = false;
    
    public Mesh trapMesh;
    public Material trapMats;
    public Mesh tileMesh;
    public Material tileMats;

    MeshFilter mf;
    MeshRenderer mr;

    AudioSource source;
    
    new void Awake(){
        base.Awake();

        mf = GetComponent<MeshFilter>();
        mr = GetComponent<MeshRenderer>();

        if(isHidden){
            mf.mesh = tileMesh;
            mr.material = tileMats;
        }

        source = GetComponent<AudioSource>();
    }

    public new void OnTriggerEnter(Collider col){
        base.OnTriggerEnter(col);
        if(occupant.tag == "Player"){
            if(isHidden){
                //unhide
                mf.mesh = trapMesh;
                mr.material = trapMats;
            }

            Compiler.Instance.terminateExecution();

            source.volume = GlobalSettings.Instance.sfxVolume;
            source.clip = AudioPicker.Instance.trap;
            source.Play();
            
            occupant.GetComponent<Animator>().SetBool("ded", true);
            Invoke("DestroyBot", 1f);
            isOccupied = false;
        }
    }

    void DestroyBot(){
        PlayBotKill();
        var chara = GameObject.Find("PlayerCharacter");
        Destroy(chara);
    }

    void PlayBotKill(){
        source.volume = GlobalSettings.Instance.sfxVolume;
        System.Random rnd = new System.Random();
        int maxIndex = AudioPicker.Instance.botDead.Length;
        source.clip = AudioPicker.Instance.botDead[rnd.Next(maxIndex)];
        source.Play();
    }
}
