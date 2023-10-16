using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IfElseStage3 : MonoBehaviour{
    public TileBase actualA0, actualA4;
    public TrapTile trapA0, trapA4;
    public FloorButton floorButtonL, floorButtonR;
    public WallPanel checkedPanel;
    void OnTriggerEnter(Collider col){
        if(col.tag != "Player"){
            return;
        }

        Start();
    }

    void Start(){
        if(checkedPanel.storedInt < 5){
            actualA0.gameObject.SetActive(true);
            floorButtonL.gameObject.AddComponent<BindFloorButton>();
            actualA4.gameObject.SetActive(false);
            Destroy(floorButtonR.gameObject.GetComponent("BindFloorButton"));
            
            trapA0.gameObject.SetActive(false);
            floorButtonL.isTrap = false;
            trapA4.gameObject.SetActive(true);
            floorButtonR.isTrap = true;
        } else {
            actualA0.gameObject.SetActive(false);
            Destroy(floorButtonL.gameObject.GetComponent("BindFloorButton"));
            actualA4.gameObject.SetActive(true);
            floorButtonR.gameObject.AddComponent<BindFloorButton>();
            
            trapA0.gameObject.SetActive(true);
            floorButtonL.isTrap = true;
            trapA4.gameObject.SetActive(false);
            floorButtonR.isTrap = false;
        }
    }
}
