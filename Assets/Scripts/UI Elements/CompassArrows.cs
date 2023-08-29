using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CompassArrows : MonoBehaviour{
    public Mode letter;
    public Color northColor;
    void Start(){
        var text = gameObject.GetComponent<TMP_Text>();

        switch(letter){
            case Mode.N:
                gameObject.transform.rotation = Quaternion.Euler(90, 0, 0);
                text.text = "N";
                
                var child = transform.GetChild(0);
                var renderer = child.GetComponent<SpriteRenderer>();
                renderer.color = northColor;
            break;
            case Mode.E:
                text.text = "E";
                gameObject.transform.rotation = Quaternion.Euler(90, 90, 0);
            break;
            case Mode.S:
                text.text = "S";
                gameObject.transform.rotation = Quaternion.Euler(90, 180, 0);
            break;
            case Mode.W:
                text.text = "W";
                gameObject.transform.rotation = Quaternion.Euler(90, 270, 0);
            break;
        }
    }

    public enum Mode{N,S,E,W}
}
