using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;

public class CoordGrid : MonoBehaviour{
    public int xGridSize, yGridSize;
    public GameObject coordItem;

    public float offset = 5f;
    public bool showSecondSet = false;
    // Start is called before the first frame update
    void Start(){
        var empty = new GameObject("name");
        
        var xCoords = Instantiate(empty);
        xCoords.transform.name = "XCoords";
        xCoords.transform.parent = gameObject.transform;
        xCoords.transform.localPosition = Vector3.zero;
        
        var yCoords = Instantiate(empty);
        yCoords.transform.name = "YCoords";
        yCoords.transform.parent = gameObject.transform;
        yCoords.transform.localPosition = Vector3.zero;

        float dist = Globals.Instance.distancePerTile;

        for(int i = 0; i < xGridSize; i++){
            var coord = Instantiate(coordItem);
            coord.name = i.ToString() + "x";
            coord.transform.SetParent(transform.GetChild(0));
            coord.transform.localPosition = new Vector3(i * dist + offset, 0, -dist);
        }

        for(int i = 0; i < yGridSize; i++){
            var coord = Instantiate(coordItem);
            coord.name = NumToString(i) + "y";
            coord.transform.SetParent(transform.GetChild(1));
            coord.transform.localPosition = new Vector3(-dist, 0, i * dist - offset);
        }

        if(showSecondSet){
            var xCoords2 = Instantiate(transform.GetChild(0));
            xCoords2.transform.name = "XCoords2";
            xCoords2.transform.parent = gameObject.transform;
            xCoords2.transform.localPosition = new Vector3(0, 0, yGridSize * 100 + 100);

            var yCoords2 = Instantiate(transform.GetChild(1));
            yCoords2.transform.name = "XCoords2";
            yCoords2.transform.parent = gameObject.transform;
            yCoords2.transform.localPosition = new Vector3(xGridSize * 100 + 100, 0, 0);
        }
    }

    string NumToString(int i){
        return i switch{
            0 => "A", 1 => "B", 2 => "C", 3 => "D", 4 => "E",
            5 => "F", 6 => "G", 7 => "H", 8 => "I", 9 => "J",
            10 => "K", 11 => "L", 12 => "M", 13 => "N", 14 => "O",
            15 => "P", 16 => "Q", 17 => "R", 18 => "S", 19 => "T",
            20 => "U", 21 => "V", 22 => "W", 23 => "X", 24 => "Y",
            25 => "Z", _ => "?",
        };
    }
}