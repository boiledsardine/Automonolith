using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsMenu : MonoBehaviour{
    public TMP_Dropdown dropdown;
    public Toggle fullscreenToggle;
    public Animator panelAnim;

    // Start is called before the first frame update
    void Awake(){
        switch(GlobalSettings.Instance.resolution){
            case ScreenResMode._1280x720:
                dropdown.value = 0;
                SetScreenRes(1280, 720);
            break;
            case ScreenResMode._1366x768:
                dropdown.value = 1;
                SetScreenRes(1366, 768);
            break;
            case ScreenResMode._1600x900:
                dropdown.value = 2;
                SetScreenRes(1600, 900);
            break;
            case ScreenResMode._1920x1080:
                dropdown.value = 3;
                SetScreenRes(1920, 1080);
            break;
        }

        fullscreenToggle.isOn = GlobalSettings.Instance.isFullscreen;
    }

    void SetScreenRes(int width, int height){
        Screen.SetResolution(width, height, GlobalSettings.Instance.isFullscreen);
    }

    public void DropdownChanged(){
        Debug.Log("Changing res");
        switch(dropdown.value){
            case 0:
                GlobalSettings.Instance.resolution = ScreenResMode._1280x720;
                SetScreenRes(1280, 720);
            break;
            case 1:
                GlobalSettings.Instance.resolution = ScreenResMode._1366x768;
                SetScreenRes(1366, 768);
            break;
            case 2:
                GlobalSettings.Instance.resolution = ScreenResMode._1600x900;
                SetScreenRes(1600, 900);
            break;
            case 3:
                GlobalSettings.Instance.resolution = ScreenResMode._1920x1080;
                SetScreenRes(1920, 1080);
            break;
        }
    }

    public void SetFullScreen(){
        Debug.Log("Changing fullscreen");
        GlobalSettings.Instance.isFullscreen = fullscreenToggle.isOn;
        Screen.fullScreen = fullscreenToggle.isOn;
    }

    public void CloseOptions(){
        panelAnim.SetBool("isOpen", false);
        GlobalSettings.Instance.SaveSettings();
    }
}
