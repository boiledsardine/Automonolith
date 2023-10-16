using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level9Mgr : MonoBehaviour{
    public WallPanel wallPanel;
    public VoxGate[] gates;
    public int[] values;

    void Start(){
        values.Shuffle();
        wallPanel.storedIntArr = values;

        //speak the third element
        gates[0].password = wallPanel.storedIntArr[2].ToString();

        //speak the product of the first and the fourth
        gates[1].password = (wallPanel.storedIntArr[0] * wallPanel.storedIntArr[3]).ToString();

        //speak the length of the array
        gates[2].password = wallPanel.storedIntArr.Length.ToString();

        //speak the sum of the first three elements
        gates[3].password = (wallPanel.storedIntArr[0] + wallPanel.storedIntArr[1] + wallPanel.storedIntArr[2]).ToString();
        
        //speak the final element of the array
        gates[4].password = wallPanel.storedIntArr[wallPanel.storedIntArr.Length - 1].ToString();
    }
}
