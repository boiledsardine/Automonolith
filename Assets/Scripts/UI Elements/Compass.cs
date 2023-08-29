using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Compass : MonoBehaviour{
    public GameObject compassArrow;
    public int gridWidth, gridLength;
    public Color northColor;
    
    // Start is called before the first frame update
    void Start(){
        List<GameObject> arrows = new List<GameObject>();
        for(int i = 0; i < 4; i++){
            var arrow = Instantiate(compassArrow);
            arrow.name = "arrow" + 0;
            arrow.transform.SetParent(gameObject.transform);
            arrows.Add(arrow);
        }

        //order: N, E, S, W
        string[] letters = {"N", "E", "S", "W"};

        Vector3[] positions = {
            new Vector3((gridWidth - 1) * 100 / 2, 0, gridLength * 100 + 100),
            new Vector3(gridWidth * 100 + 100, 0, (gridLength - 1) * 100 / 2),
            new Vector3((gridWidth - 1) * 100 / 2, 0, -200),
            new Vector3(-200, 0, (gridLength - 1) * 100 / 2)
        };

        Quaternion[] rotations = {
            Quaternion.Euler(90, 0, 0),
            Quaternion.Euler(90, 90, 0),
            Quaternion.Euler(90, 180, 0),
            Quaternion.Euler(90, 270, 0)
        };

        for(int i = 0; i < arrows.Count; i++){
            arrows[i].transform.localPosition = positions[i];
            arrows[i].transform.rotation = rotations[i];
            var text = arrows[i].GetComponent<TMPro.TMP_Text>();
            text.text = letters[i];
        }
        
        var rend = arrows[0].transform.GetChild(0).GetComponent<SpriteRenderer>();
        rend.color = northColor;
    }
}