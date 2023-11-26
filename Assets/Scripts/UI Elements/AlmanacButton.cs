using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlmanacButton : MonoBehaviour{
    public int index = 0;
    public string managerName;
    public void OnClick(){
        var manager = GameObject.Find(managerName).GetComponent<AlmanacManager>();
        manager.lastClicked = GetComponent<Image>();
        manager.LoadEntry(index);
        manager.SetClickedButton();
    }
}
