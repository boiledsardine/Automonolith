using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeTile : TileBase{
    MeshRenderer meshRend;
    public Material[] mat;
    
    void Start(){
        meshRend = GetComponent<MeshRenderer>();
        Invoke("ChangeRed", Globals.Instance.timePerStep);
        //StartCoroutine(ChangeRed());
    }

    IEnumerator ChangeRed(){
        yield return new WaitForSeconds(Globals.Instance.timePerStep * 2);
        Debug.Log("red");
        meshRend.material = mat[0];
        StartCoroutine(ChangeGreen());
    }

    IEnumerator ChangeGreen(){
        yield return new WaitForSeconds(Globals.Instance.timePerStep * 2);
        Debug.Log("green");
        meshRend.material = mat[1];
        StartCoroutine(ChangeRed());
    }
}
