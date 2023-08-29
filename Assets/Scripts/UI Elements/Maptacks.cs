using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maptacks : MonoBehaviour{
    public int gridSizeX;
    public int gridSizeY;
    public GameObject coordinateGrid;
    public GameObject compassArrows;

    public bool showCompassArrowsOnStart = true;
    public bool showCoordinateGridOnStart = true;
    public bool showSecondCoordSet;

    void Start(){
        var coordGrid = Instantiate(coordinateGrid);
        coordGrid.name = "CoordinateXY";
        coordGrid.transform.position = gameObject.transform.position;
        coordGrid.transform.SetParent(gameObject.transform);
        var coordScript = coordGrid.GetComponent<CoordGrid>();
        coordScript.xGridSize = gridSizeX;
        coordScript.yGridSize = gridSizeY;
        coordScript.showSecondSet = showSecondCoordSet;

        if(!showCoordinateGridOnStart){
            coordGrid.SetActive(false);
        }

        var compass = Instantiate(compassArrows);
        compass.name = "CompassPoints";
        compass.transform.position = gameObject.transform.position;
        compass.transform.SetParent(gameObject.transform);
        var compassScript = compass.GetComponent<Compass>();
        compassScript.gridWidth = gridSizeX;
        compassScript.gridLength = gridSizeY;

        if(!showCompassArrowsOnStart){
            compass.SetActive(false);
        }
    }

    bool coordsShown = true;
    public void ToggleCoordinates(){
        if(coordsShown){
            HideCoordinates();
        } else {
            ShowCoordinates();
        }
    }

    public void HideCoordinates(){
        transform.GetChild(0).gameObject.SetActive(false);
        coordsShown = false;
    }

    public void ShowCoordinates(){
        transform.GetChild(0).gameObject.SetActive(true);
        coordsShown = true;
    }

    bool compassShown = true;
    public void ToggleCompass(){
        if(compassShown){
            HideCompass();
        } else {
            ShowCompass();
        }
    }

    public void HideCompass(){
        transform.GetChild(0).gameObject.SetActive(false);
        compassShown = false;
    }

    public void ShowCompass(){
        transform.GetChild(0).gameObject.SetActive(true);
        compassShown = true;
    }
}
