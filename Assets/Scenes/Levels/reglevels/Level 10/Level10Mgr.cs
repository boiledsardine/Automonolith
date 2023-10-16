using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level10Mgr : MonoBehaviour, IActivate{
    public List<string> coordinates;
    public WallPanel wallPanel;
    public MultiActivate multiActivator;
    public FloorButton[] buttons;
    public int amount;
    Dictionary<string,FloorButton> dict;

    public void Start(){

        string[] strArr = new string[amount];
        coordinates.Shuffle();
        for(int i = 0; i < strArr.Length; i++){
            strArr[i] = coordinates[i];
        }
        wallPanel.storedTextArr = strArr;

        var button = GameObject.Find(wallPanel.storedTextArr[0] + "button");
        var buttonScript = button.GetComponent<FloorButton>();
        buttonScript.isTrap = false;
        buttonScript.boundObject = this.gameObject;

        var tile = GameObject.Find(wallPanel.storedTextArr[0]);
        tile.AddComponent<TileBase>();
        Destroy(tile.GetComponent<TrapTile>());

        multiActivator.activators = new GameObject[wallPanel.storedTextArr.Length];
        for(int i = 0; i < wallPanel.storedTextArr.Length; i++){
            multiActivator.activators[i] = GameObject.Find(wallPanel.storedTextArr[i] + "button");
        }
    }

    int activateCount = 0;
    public void activate(){
        activateCount++;

        if(activateCount < amount){
            var button = GameObject.Find(wallPanel.storedTextArr[activateCount] + "button");
            var buttonScript = button.GetComponent<FloorButton>();
            buttonScript.isTrap = false;
            buttonScript.boundObject = this.gameObject;

            var tile = GameObject.Find(wallPanel.storedTextArr[activateCount]);
            tile.AddComponent<TileBase>();
            Destroy(tile.GetComponent<TrapTile>());
        }
        
        multiActivator.activate();
    }

    public void deactivate(){
        throw new System.NotImplementedException();
    }
}
