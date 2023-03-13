using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnvironment{
    //interface for environment detection functions

    bool neighborIsValid(char direction);
    void OnTriggerEnter(Collider col);
}
