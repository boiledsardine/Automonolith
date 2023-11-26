using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateBase : MonoBehaviour{
    private Vector3 openPos, closedPos;
    [SerializeField] float targetPosY;
    [SerializeField] protected TileBase tileFront, tileBack;
    AudioSource source;
    protected bool isActive;

    void Awake(){
        source = GetComponent<AudioSource>();
    }

    void Update(){
        //divisible by 180, probably facing x axis
        if(gameObject.transform.parent.transform.rotation.eulerAngles.y % 180 == 0){
            Debug.DrawRay(transform.position, Vector3.left * Globals.Instance.distancePerTile, Color.blue);
            Debug.DrawRay(transform.position, Vector3.right * Globals.Instance.distancePerTile, Color.green);
        }

        //probably 270 or 90, facing y axis
        else if(gameObject.transform.parent.transform.rotation.eulerAngles.y % 180 == 90){
            Debug.DrawRay(transform.position, Vector3.forward * Globals.Instance.distancePerTile, Color.blue);
            Debug.DrawRay(transform.position, Vector3.back * Globals.Instance.distancePerTile, Color.green);
        }
    }

    protected void Start(){
        closedPos = transform.position;
        openPos = closedPos + new Vector3(0f, targetPosY, 0f);

        //fire raycasts to find front and back tiles
        int tileMask = 1 << 7;
        float distance = Globals.Instance.distancePerTile;
        
        Vector3 fwd = Vector3.forward;
        Vector3 bck = Vector3.back;

        if(gameObject.transform.parent.transform.rotation.eulerAngles.y % 180 == 90){
            //facing y axis
            fwd = Vector3.forward;
            bck = Vector3.back;
        } else if(gameObject.transform.parent.transform.rotation.eulerAngles.y % 180 == 0) {
            //facing x axis
            fwd = Vector3.left;
            bck = Vector3.right;
        }

        if(Physics.Raycast(transform.position, fwd, out RaycastHit hitF, distance, tileMask)){
            tileFront = hitF.transform.gameObject.GetComponent<TileBase>();
        }

        if(Physics.Raycast(transform.position, bck, out RaycastHit hitB, distance, tileMask)){
            tileBack = hitB.transform.gameObject.GetComponent<TileBase>();
        }
    }

    protected IEnumerator openGate(){
        gameObject.layer = 0;
        float timeElapsed = 0;
        while(timeElapsed < Globals.Instance.timeToMove){
            transform.position = Vector3.Lerp(closedPos, openPos,
            (timeElapsed / Globals.Instance.timeToMove));
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = openPos;
    }

    protected IEnumerator closeGate(){
        float timeElapsed = 0;
        while(timeElapsed < Globals.Instance.timeToMove){
            transform.position = Vector3.Lerp(openPos, closedPos,
            (timeElapsed / Globals.Instance.timeToMove));
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = closedPos;
    }

    protected void PlayOpenSound(){
        if(isActive){
            return;
        }

        source.volume = GlobalSettings.Instance.sfxVolume;
        source.clip = AudioPicker.Instance.gateOpen;
        source.Play();
    }

    protected void PlayCloseSound(){
        if(!isActive){
            return;
        }

        source.volume = GlobalSettings.Instance.sfxVolume;
        source.clip = AudioPicker.Instance.gateClose;
        source.Play();
    }
}
