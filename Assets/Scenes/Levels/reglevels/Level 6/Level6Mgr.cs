using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level6Mgr : MonoBehaviour{
    public Mode mode;
    public Level6Mgr trueNode, falseNode;
    public FloorButton[] buttons;
    public TrapTile[] traps;
    public ExitPoint exit;
    public WallPanel intPanel;
    public int comparisonInt;

    //Check startNode, then activate midNode depending on condition
    //then midNode activates endNode depending on condition
    void Start(){
        if(mode != Mode.startNode){
            return;
        }

        if(intPanel.storedInt > comparisonInt){
            trueNode.ActivateNode();
        } else {
            falseNode.ActivateNode();
        }
    }

    void ActivateNode(){
        if(mode == Mode.midNode){
            if(intPanel.storedInt == comparisonInt){
                trueNode.ActivateNode();
            } else {
                falseNode.ActivateNode();
            }
        } else if(mode == Mode.endNode){
            int trueIndex = intPanel.storedInt < comparisonInt ? 0 : 1;
            buttons[trueIndex].boundObject = exit.gameObject;
            buttons[trueIndex].isTrap = false;

            var tile = traps[trueIndex].gameObject;
            tile.AddComponent<TileBase>();
            Destroy(tile.GetComponent<TrapTile>());
        }
    }

    public enum Mode{
        startNode, midNode, endNode
    }
}