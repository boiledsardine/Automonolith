using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommandControl{

public class Interaction : MonoBehaviour {
    private GameObject _heldObject;
    private ObjectMovement objMove;
    private bool _isHolding;
    private bool _isHoldingBig;
    private bool _isHoldingSmall;
    private Vector3 originPos, targetPos;
    [SerializeField] private GameObject speechBubble;
    private GameObject bubble = null;

    private Environment envirScript;
    private Animator anim;
    public RuntimeAnimatorController[] animStates; 
    AudioSource source;

    private void Awake(){
        envirScript = gameObject.GetComponent<Environment>();
        anim = gameObject.GetComponent<Animator>();
        source = gameObject.GetComponent<AudioSource>();
    }

    void Update(){
        Vector3 pos = new Vector3(transform.position.x, transform.position.y + 50, transform.position.z);
        Vector3 fwd = transform.TransformDirection(Vector3.back);
        Vector3 bck = transform.TransformDirection(Vector3.right);
        Vector3 lft = transform.TransformDirection(Vector3.left);
        Vector3 rgt = transform.TransformDirection(Vector3.forward);
        Debug.DrawRay(pos, fwd * Globals.Instance.distancePerTile, Color.red);
        Debug.DrawRay(pos, bck * Globals.Instance.distancePerTile, Color.red);
        Debug.DrawRay(pos, lft * Globals.Instance.distancePerTile, Color.red);
        Debug.DrawRay(pos, rgt * Globals.Instance.distancePerTile, Color.red);
    }

    public GameObject heldObject{
        get { return _heldObject; }
        set { _heldObject = value; }
    }

    public bool isHolding{
        get { return _isHolding; }
        set { _isHolding = value; }
    }

    public bool isHoldingBig{
        get { return _isHoldingBig; }
        set { _isHoldingBig = value; }
    }

    public bool isHoldingSmall{
        get { return _isHoldingSmall; }
        set { _isHoldingSmall = value; }
    }

    public IEnumerator hold(){
        Vector3 pos = new Vector3(transform.position.x, transform.position.y + 50, transform.position.z);
        
        //fire raycast to detect object
        int layermask = 1 << 9;
        float distance = Globals.Instance.distancePerTile;
        Vector3 fwd = transform.TransformDirection(Vector3.back);
        if(Physics.Raycast(pos, fwd, out RaycastHit hit, distance, layermask)){
            if(!isHolding){
                var hitObject = hit.transform.gameObject;
                if(hitObject.GetComponent<ObjectBase>().isMovable){
                    heldObject = hitObject;
                    isHolding = true;
                    
                    PlayHoldSound();

                    if(heldObject.tag == "Big Object"){
                        anim.runtimeAnimatorController = animStates[1];
                        anim.SetBool("armsForward", true);
                        yield return new WaitForSeconds(Globals.Instance.timePerStep);
                        hitObject.GetComponent<ObjectBase>().isHeld = true;
                        isHoldingBig = true;
                        objMove = heldObject.GetComponent<ObjectMovement>();
                        
                        //changes tileUnder state to unoccupied
                        heldObject.GetComponent<BoxCollider>().enabled = false;
                        var tile = heldObject.GetComponent<ObjectEnvironment>().GetTileUnder();
                        tile.isOccupied = false;
                    } else if(heldObject.tag == "Small Object"){
                        anim.runtimeAnimatorController = animStates[1];
                        anim.SetBool("armsUp", true);
                        yield return new WaitForSeconds(Globals.Instance.timePerStep);
                        hitObject.GetComponent<ObjectBase>().isHeld = true;
                        isHoldingSmall = true;
                    }
                } else {
                    yield return new WaitForSeconds(Globals.Instance.timePerStep);
                    Compiler.Instance.addErr(string.Format("This isn't a holdable object!"));
                    Compiler.Instance.errorChecker.writeError();
                    Compiler.Instance.killTimer();
                    Debug.LogWarning("Object is not movable!");
                }
            } else if(isHolding) {
                yield return new WaitForSeconds(Globals.Instance.timePerStep);
                Compiler.Instance.addErr(string.Format("G4wain is already holding something!"));
                Compiler.Instance.errorChecker.writeError();
                Compiler.Instance.killTimer();
                Debug.LogWarning("Already holding something!");
            }
        } else {
            yield return new WaitForSeconds(Globals.Instance.timePerStep);
            Compiler.Instance.addErr(string.Format("There's nothing for G4wain to hold!"));
            Compiler.Instance.errorChecker.writeError();
            Compiler.Instance.killTimer();
            Debug.LogWarning("Nothing to hold!");
        }
    }

    public IEnumerator drop(){
        Vector3 pos = new Vector3(transform.position.x, transform.position.y + 50, transform.position.z);
        yield return new WaitForSeconds(Globals.Instance.timePerStep);

        if(!isHolding){
            Compiler.Instance.addErr(string.Format("G4wain isn't holding anything!"));
            Compiler.Instance.errorChecker.writeError();
            Compiler.Instance.killTimer();
            Debug.LogWarning("not holding!");
            yield break;
        }
        
        //TODO: change this to be small-object specific if it messes with big stuff
        //get the release tile
        float distance = Globals.Instance.distancePerTile;
        Vector3 fwd = transform.TransformDirection(Vector3.back);
        TileBase releaseTile;
        if(Physics.Raycast(pos, fwd, out RaycastHit hit, distance, 1 << 7)){
            releaseTile = hit.transform.gameObject.GetComponent<TileBase>();
        } else {
            Compiler.Instance.addErr(string.Format("G4wain cannot put a held object where there's no tile!"));
            Compiler.Instance.errorChecker.writeError();
            Compiler.Instance.killTimer();
            releaseTile = null;
        }

        bool isObstructed = false;
        //fire raycast to check for walls
        if(Physics.Raycast(pos, fwd, out RaycastHit hitWall, distance, 1 << 6)){
            Compiler.Instance.addErr(string.Format("G4wain's line of sight is obstructed by a wall!"));
            Compiler.Instance.errorChecker.writeError();
            Compiler.Instance.killTimer();
            Debug.LogWarning("Wall found!");
            isObstructed = true;
        }

        //fire raycast to check for objects
        if(!isHoldingBig && Physics.Raycast(pos, fwd, out RaycastHit hitObject, distance)){
            if(hitObject.transform.gameObject.tag == "Small Object" || hitObject.transform.gameObject.tag == "Big Object"){
                Compiler.Instance.addErr(string.Format("This tile is already occupied!"));
                Compiler.Instance.errorChecker.writeError();
                Compiler.Instance.killTimer();
                Debug.LogWarning("Already occupied!");
                isObstructed = true;
            }
        }

        if(releaseTile != null && !isObstructed){
            anim.SetBool("armsUp", false);
            anim.SetBool("armsForward", false);
            Invoke(nameof(ResetAnimState), Globals.Instance.timePerStep);
            PlayDropSound();
            
            drop_NoExecute(releaseTile);
        }
    }

    void ResetAnimState(){
        anim.runtimeAnimatorController = animStates[0];
    }

    public void drop_NoExecute(TileBase releaseTile){
        if(isHoldingSmall){
            heldObject.GetComponent<Positioning>().release(releaseTile);
        }
        if(isHoldingBig){
            heldObject.GetComponent<BoxCollider>().enabled = true;
            var tile = heldObject.GetComponent<ObjectEnvironment>().GetTileUnder();
            tile.isOccupied = true;
        }
        if(isHolding){
            heldObject.GetComponent<ObjectBase>().isHeld = false;
            heldObject = null;
            objMove = null;
            isHolding = false;
            isHoldingBig = false;
            isHoldingSmall = false;
        }
    }

    public void OnDestroy(){
        drop_NoExecute(envirScript.currentTile);
    }

    public IEnumerator interact(){
        Vector3 pos = new Vector3(transform.position.x, transform.position.y + 50, transform.position.z);
        yield return new WaitForSeconds(Globals.Instance.timePerStep);
        Vector3 fwd = transform.TransformDirection(Vector3.back);
        float distance = Globals.Instance.distancePerTile;

        if(Physics.Raycast(pos, fwd, out RaycastHit hit, distance)){
            GameObject hitObject = hit.transform.gameObject;
            if(hitObject.tag == "Interactable"){
                IActivate activation = hitObject.GetComponent<IActivate>();
                activation.activate();
                //Debug.Log("Interacted with " + hitObject.name);
            } else {
                Compiler.Instance.addErr(string.Format("This object isn't interactable!"));
                Compiler.Instance.errorChecker.writeError();
                Compiler.Instance.killTimer();
                Debug.Log(hitObject.name + " is not interactable");
            }
        }
    }

    public string read(){
        Vector3 pos = new Vector3(transform.position.x, transform.position.y + 50, transform.position.z);
        //yield return new WaitForSeconds(Globals.Instance.timePerStep);
        Vector3 fwd = transform.TransformDirection(Vector3.back);
        string readString = null;
        bool foundPanel = false;
        //fire raycast in 4 directions
        Vector3 north = new Vector3(0, 0, 1);
        Vector3 south = new Vector3(0, 0, -1);
        Vector3 east = new Vector3(1, 0, 0);
        Vector3 west = new Vector3(-1, 0, 0);
        float distance = Globals.Instance.distancePerObject;
        if(Physics.Raycast(pos, north, out RaycastHit hitN, distance) && hitN.transform.gameObject.tag == "WallPanel"){
            readString = GetString(hitN);
            foundPanel = true;
        }
        if(Physics.Raycast(pos, south, out RaycastHit hitS, distance) && hitS.transform.gameObject.tag == "WallPanel"){
            readString = GetString(hitS);
            foundPanel = true;
        }
        if(Physics.Raycast(pos, east, out RaycastHit hitE, distance) && hitE.transform.gameObject.tag == "WallPanel"){
            readString = GetString(hitE);
            foundPanel = true;
        }
        if(Physics.Raycast(pos, west, out RaycastHit hitW, distance) && hitW.transform.gameObject.tag == "WallPanel"){
            readString = GetString(hitW);
            foundPanel = true;
        }
        
        if(!foundPanel){
            Compiler.Instance.addErr(string.Format("There are no wall panels around G4wain!"));
            Compiler.Instance.errorChecker.writeError();
            Compiler.Instance.killTimer();
            Debug.LogWarning("No panels found!");
        }

        PlayReadSound();
        return readString;
    }

    private string GetString(RaycastHit hit){
        string s = "";
        WallPanel panel = hit.transform.gameObject.GetComponent<WallPanel>();
        if(panel.panelType == PanelType.stringPanel){
            s = panel.storedText;
        } else {
            Compiler.Instance.addErr(string.Format("This wall panel isn't storing a string!"));
            Compiler.Instance.errorChecker.writeError();
            Compiler.Instance.killTimer();
            Debug.LogWarning("Panel isn't storing a string!");
        }
        return s;
    }

    public int readInt(){
        Vector3 pos = new Vector3(transform.position.x, transform.position.y + 50, transform.position.z);
        //yield return new WaitForSeconds(Globals.Instance.timePerStep);
        Vector3 fwd = transform.TransformDirection(Vector3.back);
        int readInt = 0;
        bool foundPanel = false;
        //fire raycast in 4 directions
        Vector3 north = new Vector3(0, 0, 1);
        Vector3 south = new Vector3(0, 0, -1);
        Vector3 east = new Vector3(1, 0, 0);
        Vector3 west = new Vector3(-1, 0, 0);
        float distance = Globals.Instance.distancePerObject;
        if(Physics.Raycast(pos, north, out RaycastHit hitN, distance) && hitN.transform.gameObject.tag == "WallPanel"){
            readInt = GetInt(hitN);
            foundPanel = true;
        }
        if(Physics.Raycast(pos, south, out RaycastHit hitS, distance) && hitS.transform.gameObject.tag == "WallPanel"){
            readInt = GetInt(hitS);
            foundPanel = true;
        }
        if(Physics.Raycast(pos, east, out RaycastHit hitE, distance) && hitE.transform.gameObject.tag == "WallPanel"){
            readInt = GetInt(hitE);
            foundPanel = true;
        }
        if(Physics.Raycast(pos, west, out RaycastHit hitW, distance) && hitW.transform.gameObject.tag == "WallPanel"){
            readInt = GetInt(hitW);
            foundPanel = true;
        }
        
        if(!foundPanel){
            Compiler.Instance.addErr(string.Format("There are no wall panels around G4wain!"));
            Compiler.Instance.errorChecker.writeError();
            Compiler.Instance.killTimer();
            Debug.LogWarning("No panels found!");
        }

        PlayReadSound();
        return readInt;
    }

    private int GetInt(RaycastHit hit){
        int i = 0;
        WallPanel panel = hit.transform.gameObject.GetComponent<WallPanel>();
        if(panel.panelType == PanelType.intPanel){
            i = panel.storedInt;
        } else {
            Compiler.Instance.addErr(string.Format("This wall panel isn't storing an int!"));
            Compiler.Instance.errorChecker.writeError();
            Compiler.Instance.killTimer();
            Debug.LogWarning("Panel isn't storing an int!");
        }
        return i;
    }

    public bool readBool(){
        Vector3 pos = new Vector3(transform.position.x, transform.position.y + 50, transform.position.z);
        //yield return new WaitForSeconds(Globals.Instance.timePerStep);
        Vector3 fwd = transform.TransformDirection(Vector3.back);
        bool readBool = false;
        bool foundPanel = false;
        //fire raycast in 4 directions
        Vector3 north = new Vector3(0, 0, 1);
        Vector3 south = new Vector3(0, 0, -1);
        Vector3 east = new Vector3(1, 0, 0);
        Vector3 west = new Vector3(-1, 0, 0);
        float distance = Globals.Instance.distancePerObject;
        if(Physics.Raycast(pos, north, out RaycastHit hitN, distance) && hitN.transform.gameObject.tag == "WallPanel"){
            readBool = GetBool(hitN);
            foundPanel = true;
        }
        if(Physics.Raycast(pos, south, out RaycastHit hitS, distance) && hitS.transform.gameObject.tag == "WallPanel"){
            readBool = GetBool(hitS);
            foundPanel = true;
        }
        if(Physics.Raycast(pos, east, out RaycastHit hitE, distance) && hitE.transform.gameObject.tag == "WallPanel"){
            readBool = GetBool(hitE);
            foundPanel = true;
        }
        if(Physics.Raycast(pos, west, out RaycastHit hitW, distance) && hitW.transform.gameObject.tag == "WallPanel"){
            readBool = GetBool(hitW);
            foundPanel = true;
        }
        
        if(!foundPanel){
            Compiler.Instance.addErr(string.Format("There are no wall panels around G4wain!"));
            Compiler.Instance.errorChecker.writeError();
            Compiler.Instance.killTimer();
            Debug.LogWarning("No panels found!");
        }

        PlayReadSound();
        return readBool;
    }
    
    private bool GetBool(RaycastHit hit){
        bool b = false;
        WallPanel panel = hit.transform.gameObject.GetComponent<WallPanel>();
        if(panel.panelType == PanelType.boolPanel){
            b = panel.storedBool;
        } else {
            Compiler.Instance.addErr(string.Format("This wall panel isn't storing a bool!"));
            Compiler.Instance.errorChecker.writeError();
            Compiler.Instance.killTimer();
            Debug.LogWarning("Panel isn't storing a bool!");
        }
        return b;
    }
    
    public int[] ReadIntArr(){
        Vector3 pos = new Vector3(transform.position.x, transform.position.y + 50, transform.position.z);
        //yield return new WaitForSeconds(Globals.Instance.timePerStep);
        Vector3 fwd = transform.TransformDirection(Vector3.back);
        int[] arr = new int[0];
        bool foundPanel = false;
        //fire raycast in 4 directions
        Vector3 north = new Vector3(0, 0, 1);
        Vector3 south = new Vector3(0, 0, -1);
        Vector3 east = new Vector3(1, 0, 0);
        Vector3 west = new Vector3(-1, 0, 0);
        float distance = Globals.Instance.distancePerObject;
        if(Physics.Raycast(pos, north, out RaycastHit hitN, distance) && hitN.transform.gameObject.tag == "WallPanel"){
            arr = GetIntArr(hitN);
            foundPanel = true;
        }
        if(Physics.Raycast(pos, south, out RaycastHit hitS, distance) && hitS.transform.gameObject.tag == "WallPanel"){
            arr = GetIntArr(hitS);
            foundPanel = true;
        }
        if(Physics.Raycast(pos, east, out RaycastHit hitE, distance) && hitE.transform.gameObject.tag == "WallPanel"){
            arr = GetIntArr(hitE);
            foundPanel = true;
        }
        if(Physics.Raycast(pos, west, out RaycastHit hitW, distance) && hitW.transform.gameObject.tag == "WallPanel"){
            arr = GetIntArr(hitW);
            foundPanel = true;
        }
        
        if(!foundPanel){
            Compiler.Instance.addErr(string.Format("There are no wall panels around G4wain!"));
            Compiler.Instance.errorChecker.writeError();
            Compiler.Instance.killTimer();
            Debug.LogWarning("No panels found!");
        }

        PlayReadSound();
        return arr;
    }
    
    private int[] GetIntArr(RaycastHit hit){
        int[] arr = new int[0];
        WallPanel panel = hit.transform.gameObject.GetComponent<WallPanel>();
        if(panel.panelType == PanelType.intArrPanel){
            arr = panel.storedIntArr;
        } else {
            Compiler.Instance.addErr(string.Format("This wall panel isn't storing an int array!"));
            Compiler.Instance.errorChecker.writeError();
            Compiler.Instance.killTimer();
            Debug.LogWarning("Panel isn't storing an int array!");
        }
        return arr;
    }

    public string[] ReadStringArr(){
        Vector3 pos = new Vector3(transform.position.x, transform.position.y + 50, transform.position.z);
        //yield return new WaitForSeconds(Globals.Instance.timePerStep);
        Vector3 fwd = transform.TransformDirection(Vector3.back);
        string[] arr = new string[0];
        bool foundPanel = false;
        //fire raycast in 4 directions
        Vector3 north = new Vector3(0, 0, 1);
        Vector3 south = new Vector3(0, 0, -1);
        Vector3 east = new Vector3(1, 0, 0);
        Vector3 west = new Vector3(-1, 0, 0);
        float distance = Globals.Instance.distancePerObject;
        if(Physics.Raycast(pos, north, out RaycastHit hitN, distance) && hitN.transform.gameObject.tag == "WallPanel"){
            arr = GetStringArr(hitN);
            foundPanel = true;
        }
        if(Physics.Raycast(pos, south, out RaycastHit hitS, distance) && hitS.transform.gameObject.tag == "WallPanel"){
            arr = GetStringArr(hitS);
            foundPanel = true;
        }
        if(Physics.Raycast(pos, east, out RaycastHit hitE, distance) && hitE.transform.gameObject.tag == "WallPanel"){
            arr = GetStringArr(hitE);
            foundPanel = true;
        }
        if(Physics.Raycast(pos, west, out RaycastHit hitW, distance) && hitW.transform.gameObject.tag == "WallPanel"){
            arr = GetStringArr(hitW);
            foundPanel = true;
        }
        
        if(!foundPanel){
            Compiler.Instance.addErr(string.Format("There are no wall panels around G4wain!"));
            Compiler.Instance.errorChecker.writeError();
            Compiler.Instance.killTimer();
            Debug.LogWarning("No panels found!");
        }

        PlayReadSound();
        return arr;
    }
    
    private string[] GetStringArr(RaycastHit hit){
        string[] arr = new string[0];
        WallPanel panel = hit.transform.gameObject.GetComponent<WallPanel>();
        if(panel.panelType == PanelType.stringArrPanel){
            arr = panel.storedTextArr;
        } else {
            Compiler.Instance.addErr(string.Format("This wall panel isn't storing a string array!"));
            Compiler.Instance.errorChecker.writeError();
            Compiler.Instance.killTimer();
            Debug.LogWarning("Panel isn't storing a string array!");
        }
        return arr;
    }

    public bool[] ReadBoolArr(){
        Vector3 pos = new Vector3(transform.position.x, transform.position.y + 50, transform.position.z);
        //yield return new WaitForSeconds(Globals.Instance.timePerStep);
        Vector3 fwd = transform.TransformDirection(Vector3.back);
        bool[] arr = new bool[0];
        bool foundPanel = false;
        //fire raycast in 4 directions
        Vector3 north = new Vector3(0, 0, 1);
        Vector3 south = new Vector3(0, 0, -1);
        Vector3 east = new Vector3(1, 0, 0);
        Vector3 west = new Vector3(-1, 0, 0);
        float distance = Globals.Instance.distancePerObject;
        if(Physics.Raycast(pos, north, out RaycastHit hitN, distance) && hitN.transform.gameObject.tag == "WallPanel"){
            arr = GetBoolArr(hitN);
            foundPanel = true;
        }
        if(Physics.Raycast(pos, south, out RaycastHit hitS, distance) && hitS.transform.gameObject.tag == "WallPanel"){
            arr = GetBoolArr(hitS);
            foundPanel = true;
        }
        if(Physics.Raycast(pos, east, out RaycastHit hitE, distance) && hitE.transform.gameObject.tag == "WallPanel"){
            arr = GetBoolArr(hitE);
            foundPanel = true;
        }
        if(Physics.Raycast(pos, west, out RaycastHit hitW, distance) && hitW.transform.gameObject.tag == "WallPanel"){
            arr = GetBoolArr(hitW);
            foundPanel = true;
        }
        
        if(!foundPanel){
            Compiler.Instance.addErr(string.Format("There are no wall panels around G4wain!"));
            Compiler.Instance.errorChecker.writeError();
            Compiler.Instance.killTimer();
            Debug.LogWarning("No panels found!");
        }

        PlayReadSound();
        return arr;
    }
    
    private bool[] GetBoolArr(RaycastHit hit){
        bool[] arr = new bool[0];
        WallPanel panel = hit.transform.gameObject.GetComponent<WallPanel>();
        if(panel.panelType == PanelType.boolArrPanel){
            arr = panel.storedBoolArr;
        } else {
            Compiler.Instance.addErr(string.Format("This wall panel isn't storing a bool array!"));
            Compiler.Instance.errorChecker.writeError();
            Compiler.Instance.killTimer();
            Debug.LogWarning("Panel isn't storing a bool array!");
        }
        return arr;
    }

    public bool CheckCube(string cubeColor, bool isFirstPass){
        if(isFirstPass){
            return false;
        }

        //check if holding a cube
        if(!isHolding || heldObject.name != "CubeKey"){
            Compiler.Instance.addErr(string.Format("You must be holding a cube to use CheckCube()!"));
            Compiler.Instance.errorChecker.writeError();
            Compiler.Instance.killTimer();
            return false;
        }

        //check if match
        var cube = heldObject.GetComponent<CubePickup>();
        if(cubeColor == cube.color){
            return true;
        } else {
            return false;
        }
    }

    public IEnumerator digTile(){
        yield return new WaitForSeconds(Globals.Instance.timePerStep);

        //check if diggable
        if(envirScript.currentTile.isDirtTile){
            envirScript.currentTile.GetComponent<DirtTile>().DigTile();
        } else {
            //error
            Debug.LogWarning("function error: not usable");
        }
    }

    public IEnumerator say(string input){
        Vector3 pos = new Vector3(transform.position.x, transform.position.y + 50, transform.position.z);
        yield return new WaitForSeconds(Globals.Instance.timePerStep);
        Vector3 point = transform.position;
        
        if(bubble == null){
            bubble = Instantiate(speechBubble);
            var bubbleSet = bubble.GetComponent<SpeechBubble>();
            bubbleSet.followMode = SpeechBubble.FollowMode.FollowPlayer;
        } else {
            Destroy(bubble);
            bubble = Instantiate(speechBubble);
            var bubbleSet = bubble.GetComponent<SpeechBubble>();
            bubbleSet.followMode = SpeechBubble.FollowMode.FollowPlayer;
        }

        var speechText = bubble.transform.GetChild(0);
        speechText.GetComponent<TextMesh>().text = input;
        PlaySaySound();

        //fire raycast in 4 directions
        Vector3 north = new Vector3(0, 0, 1);
        Vector3 south = new Vector3(0, 0, -1);
        Vector3 east = new Vector3(1, 0, 0);
        Vector3 west = new Vector3(-1, 0, 0);
        float distance = Globals.Instance.distancePerTile;
        if(Physics.Raycast(pos, north, out RaycastHit hitN, distance)){
            ActivateVox(hitN, input);
        }
        if(Physics.Raycast(pos, south, out RaycastHit hitS, distance)){
            ActivateVox(hitS, input);
        }
        if(Physics.Raycast(pos, east, out RaycastHit hitW, distance)){
            ActivateVox(hitW, input);
        }
        if(Physics.Raycast(pos, west, out RaycastHit hitE, distance)){
            ActivateVox(hitE, input);
        }

        yield return new WaitForSeconds(Globals.Instance.timePerStep * 3);
    }

    private void ActivateVox(RaycastHit hit, string input){
        if(hit.transform.gameObject.tag == "Vox"){
            var voxObject = hit.transform.gameObject.GetComponent<IVox>();
            voxObject.activateVox(input);
        }
    }

    void PlaySaySound(){
        source.outputAudioMixerGroup = AudioPicker.Instance.sfxMaster;
        source.clip = AudioPicker.Instance.botSay;
        source.Play();
    }

    void PlayReadSound(){
        source.outputAudioMixerGroup = AudioPicker.Instance.sfxMaster;
        source.clip = AudioPicker.Instance.botRead;
        source.Play();
    }

    void PlayHoldSound(){
        source.outputAudioMixerGroup = AudioPicker.Instance.sfxMaster;
        source.clip = AudioPicker.Instance.botHold;
        source.Play();
    }

    void PlayDropSound(){
        source.outputAudioMixerGroup = AudioPicker.Instance.sfxMaster;
        source.clip = AudioPicker.Instance.botDrop;
        source.Play();
    }

    public void moveObject(char direction){
        if(objMove != null){
            objMove.dir = direction;
            switch(direction){
                case 'N':
                    StartCoroutine(objMove.moveUp());
                    break;
                case 'S':
                    StartCoroutine(objMove.moveDown());
                    break;
                case 'W':
                    StartCoroutine(objMove.moveLeft());
                    break;
                case 'E':
                    StartCoroutine(objMove.moveRight());
                    break;
                default:
                    break;
            }
        }
    }
}

}