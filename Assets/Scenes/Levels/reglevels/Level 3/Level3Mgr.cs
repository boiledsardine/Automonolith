using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level3Mgr : MonoBehaviour{
    public WallPanel intPanel, stringPanel;
    public FloorButton[] buttons;
    public MechGate mGate;
    public VoxGate vGate;
    public EnergyLine[] energyLines;
    public BotKiller botKiller;
    
    // Start is called before the first frame update
    void Start(){
        SetBotKiller();
        SetLineActivator(buttons[intPanel.storedInt]);
        vGate.password = stringPanel.storedText;
    }

    void SetLineActivator(FloorButton floorButton){
        foreach(EnergyLine el in energyLines){
            el.activator = floorButton.gameObject;
        }
        floorButton.boundObject = mGate.gameObject;
    }

    void SetBotKiller(){
        foreach(FloorButton fb in buttons){
            fb.boundObject = botKiller.gameObject;
        }
    }
}
