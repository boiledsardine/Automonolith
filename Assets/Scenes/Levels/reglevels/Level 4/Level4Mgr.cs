using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level4Mgr : MonoBehaviour{
    public WallPanel stringPanel, invisPanel;
    public WallButton[] buttons;
    public GameObject exitPoint, killBot;
    public WallPanel intPanelLeft, intPanelRight;
    public VoxGate vGate;
    void Start(){
        vGate.password = (intPanelLeft.storedInt + intPanelRight.storedInt).ToString();

        switch(stringPanel.storedText){
            case "E0":
                buttons[0].boundObject = exitPoint;
                buttons[1].boundObject = killBot;
                buttons[2].boundObject = killBot;
            break;
            case "E1":
                buttons[0].boundObject = killBot;
                buttons[1].boundObject = exitPoint;
                buttons[2].boundObject = killBot;
            break;
            case "E2":
                buttons[1].boundObject = killBot;
                buttons[2].boundObject = killBot;
                buttons[2].boundObject = exitPoint;
            break;
        }
    }
}
