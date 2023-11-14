using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipManager : MonoBehaviour{
    public static TooltipManager Instance;
    public Tooltip tooltip;

    private void Awake(){
        if(Instance == null){
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    public void ShowTooltip(string content, string header, bool isUIElement){
        tooltip.SetText(content, header);
        tooltip.isOverUI = isUIElement;
        tooltip.gameObject.SetActive(true);
    }

    public void HideTooltip(){
        tooltip.isOverUI = false;
        tooltip.gameObject.SetActive(false);
    }
}
