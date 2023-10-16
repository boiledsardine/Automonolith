using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IfElseStage5 : MonoBehaviour{
    public TileBase actualD0, actualC1, actualB2, actualA3;
    public TrapTile trapD0, trapC1, trapB2, trapA3;
    public FloorButton floorButton1, floorButton2, floorButton3, floorButton4;
    public WallPanel checkedPanel;
    void OnTriggerEnter(Collider col){
        if(col.tag != "Player"){
            return;
        }

        Start();
    }

    void Start(){
        if(checkedPanel.storedInt == 1){
            ActivateAll(true, false, false, false);
        } else if(checkedPanel.storedInt == 2){
            ActivateAll(false, true, false, false);
        } else if(checkedPanel.storedInt == 3){
            ActivateAll(false, false, true, false);
        } else {
            ActivateAll(false, false, false, true);
        }
    }

    void ActivateAll(bool d0, bool c1, bool b2, bool a3){
        ActivateD0(d0);
        ActivateC1(c1);
        ActivateB2(b2);
        ActivateA3(a3);
    }

    void ActivateD0(bool actual){
        if(actual){
            actualD0.gameObject.SetActive(true);
            trapD0.gameObject.SetActive(false);

            floorButton1.isTrap = false;
            floorButton1.gameObject.AddComponent<BindFloorButton>();
        } else {
            actualD0.gameObject.SetActive(false);
            trapD0.gameObject.SetActive(true);

            floorButton1.isTrap = true;
            Destroy(floorButton1.gameObject.GetComponent("BindFloorButton"));
        }
    }

    void ActivateC1(bool actual){
        if(actual){
            actualC1.gameObject.SetActive(true);
            trapC1.gameObject.SetActive(false);

            floorButton2.isTrap = false;
            floorButton2.gameObject.AddComponent<BindFloorButton>();
        } else {
            actualC1.gameObject.SetActive(false);
            trapC1.gameObject.SetActive(true);

            floorButton2.isTrap = true;
            Destroy(floorButton2.gameObject.GetComponent("BindFloorButton"));
        }
    }

    void ActivateB2(bool actual){
        if(actual){
            actualB2.gameObject.SetActive(true);
            trapB2.gameObject.SetActive(false);

            floorButton3.isTrap = false;
            floorButton3.gameObject.AddComponent<BindFloorButton>();
        } else {
            actualB2.gameObject.SetActive(false);
            trapB2.gameObject.SetActive(true);

            floorButton3.isTrap = true;
            Destroy(floorButton3.gameObject.GetComponent("BindFloorButton"));
        }
    }

    void ActivateA3(bool actual){
        if(actual){
            actualA3.gameObject.SetActive(true);
            trapA3.gameObject.SetActive(false);

            floorButton4.isTrap = false;
            floorButton4.gameObject.AddComponent<BindFloorButton>();
        } else {
            actualA3.gameObject.SetActive(false);
            trapA3.gameObject.SetActive(true);

            floorButton4.isTrap = true;
            Destroy(floorButton4.gameObject.GetComponent("BindFloorButton"));
        }
    }
}
