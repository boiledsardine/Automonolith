using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Almanac Entry", menuName = "Almanac Entry")]
public class AlmanacEntry : ScriptableObject{
    public string articleName;
    [TextArea(10, 20)]
    public string articleText;
    [TextArea(10, 20)]
    public string articleExample;
}