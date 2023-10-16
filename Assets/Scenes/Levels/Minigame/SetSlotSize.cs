using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSlotSize : MonoBehaviour{
    public string setText;
    TMPro.TMP_Text childText;
    public float sizeOffset = 4;
    [SerializeField] private GameObject emptyText;
    void Start(){
        childText = transform.GetChild(0).GetComponent<TMPro.TMP_Text>();
        childText.text = setText;

        var thisrt = GetComponent<RectTransform>();
        thisrt.sizeDelta = new Vector2(childText.preferredWidth + sizeOffset + 2, childText.preferredHeight + sizeOffset);

        Destroy(childText.transform.gameObject);
    }

    public void Resize(){
        var newEmpty = Instantiate(emptyText);
        newEmpty.transform.SetParent(gameObject.transform);
        newEmpty.transform.localScale = new Vector3(1,1,1);

        childText = transform.GetChild(1).GetComponent<TMPro.TMP_Text>();
        childText.text = setText;

        var thisrt = GetComponent<RectTransform>();
        thisrt.sizeDelta = new Vector2(childText.preferredWidth + sizeOffset + 2, childText.preferredHeight + sizeOffset);

        Destroy(childText.transform.gameObject);
    }
}
