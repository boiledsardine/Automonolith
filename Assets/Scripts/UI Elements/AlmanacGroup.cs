using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AlmanacGroup", menuName = "Almanac Group")]
public class AlmanacGroup : ScriptableObject{
    public List<AlmanacEntry> entries;
}
