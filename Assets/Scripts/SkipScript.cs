using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkipScript : MonoBehaviour{
    public Image panel, frame;
    public ExitPoint exit;

    public void Awake(){
        try{
            exit = GameObject.Find("exit-point").GetComponent<ExitPoint>();
        } catch {
            Debug.LogAssertion("didn't find any exit point");
        }
    }

    public void OpenSkipWindow(){
        panel.gameObject.SetActive(true);
        frame.gameObject.SetActive(true);

        var panelAnim = panel.GetComponent<Animator>();
        panelAnim.SetBool("isOpen", true);

        var frameAnim = frame.GetComponent<Animator>();
        frameAnim.SetBool("isOpen", true);
    }

    public void CloseSkipWindow(){
        StartCoroutine(CloseWindow());
    }

    public IEnumerator CloseWindow(){
        var panelAnim = panel.GetComponent<Animator>();
        panelAnim.SetBool("isOpen", false);

        var frameAnim = frame.GetComponent<Animator>();
        frameAnim.SetBool("isOpen", false);

        yield return new WaitForSeconds(0.25f);

        panel.gameObject.SetActive(false);
        frame.gameObject.SetActive(false);
    }

    public void SkipTutorial(){
        CloseSkipWindow();
        exit.activate();
    }
}
