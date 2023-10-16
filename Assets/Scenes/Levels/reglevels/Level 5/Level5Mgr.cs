using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level5Mgr : MonoBehaviour{
    public WallPanel panel1, panel2;
    public VoxGate[] gates;
    public int[] leftValues, rightValues;

    void Start(){
        System.Random rnd = new System.Random();
        leftValues.Shuffle();
        panel1.storedInt = leftValues[rnd.Next(leftValues.Length)];
        int num1 = panel1.storedInt;

        rightValues.Shuffle();
        panel2.storedInt = rightValues[rnd.Next(rightValues.Length)];
        int num2 = panel2.storedInt;

        //speak the lesser of the two numbers
        gates[0].password = (num1 < num2 ? num1 : num2).ToString();

        //speak the greater of the two numbers
        gates[1].password = (num1 > num2 ? num1 : num2).ToString();

        //which is smaller: 10, or the sum of both numbers?
        //gates[2].password = (10 < num1 + num2 ? 10 : num1 + num2).ToString();
    }
}
