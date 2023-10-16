using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "New Theme", menuName = "Theme")]
public class ColorizerTheme : ScriptableObject{
    public Color commentColor = new Color(68,115,67);
    public Color loopColor = new Color(146,91,179);
    public Color conditionalColor = new Color(146,91,179);
    public Color operatorColor = Color.white;
    public Color stringColor = new Color(186,116,76);
    public Color numColor = Color.white; 
    public Color braceColor1 = new Color(216,190,16);
    public Color braceColor2 = new Color(216,190,16);
    public Color braceColor3 = new Color(216,190,16);
    public Color varColor = new Color(98,168,230);
    public Color keywordColor = new Color(43,108,201);
    public Color funcColor = new Color(202,218,169);
    public Color botColor = new Color(64,193,172);
}
