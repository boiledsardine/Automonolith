using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectiveBase : MonoBehaviour{
    public abstract bool IsComplete();
    public abstract bool Objective1();
    public abstract bool Objective2();
    public abstract bool Objective3();
    public Color defaultColor = Color.white;
}
