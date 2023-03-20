using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CommandControl{

public class Bot : MonoBehaviour {
    public static Bot Instance { get; private set; }
    
    private LimitedQueue<string> _commandQueue = new LimitedQueue<string>();
    private LimitedQueue<int> _intArgQueue = new LimitedQueue<int>();
    private Queue<string> _stringArgQueue = new Queue<string>();

    private GameObject playerCharacter;
    private Movement playerMovement;
    private Environment playerEnvironment;
    private Interaction playerInteraction;
    private Vector3 originPos;
    public static string readLine = "";

    void Awake(){
        if(Instance == null){
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
        setPlayerCharacter();
    }

    private void checkQueue(){
        if(commandQueue.Count == 999){
            Destroy(GameObject.Find("CompileManager"));
            Debug.Log("The Ordo Malleus was here");
            Destroy(this.gameObject);
        }
    }

    //finds player character and sets it as player character
    //makes sure Bot calls only the player character's movement and environment scripts
    public void setPlayerCharacter(){
        playerCharacter = GameObject.Find("PlayerCharacter");
        playerMovement = playerCharacter.GetComponent<Movement>();
        playerEnvironment = playerCharacter.GetComponent<Environment>();
        playerInteraction = playerCharacter.GetComponent<Interaction>();
    }

    public Queue<string> commandQueue{
        get { return _commandQueue; }
    }

    public Queue<int> intArgQueue{
        get { return _intArgQueue; }
    }

    public Queue<string> stringArgQueue{
        get { return _stringArgQueue; }
    }

    //commands to be written by the player
    //enqueues command words into a queue
    public static void moveUp(){
        Instance.checkQueue();
        Instance.commandQueue.Enqueue("movU");
    }

    public static void moveUp(int num){
        Instance.checkQueue();
        Instance.commandQueue.Enqueue("movU_Int");
        Instance.intArgQueue.Enqueue(num);
    }

    public static void moveDown(){
        Instance.checkQueue();
        Instance.commandQueue.Enqueue("movD");
    }

    public static void moveDown(int num){
        Instance.checkQueue();
        Instance.commandQueue.Enqueue("movD_Int");
        Instance.intArgQueue.Enqueue(num);
    }

    public static void moveLeft(){
        Instance.checkQueue();
        Instance.commandQueue.Enqueue("movL");
    }

    public static void moveLeft(int num){
        Instance.checkQueue();
        Instance.commandQueue.Enqueue("movL_Int");
        Instance.intArgQueue.Enqueue(num);
    }

    public static void moveRight(){
        Instance.checkQueue();
        Instance.commandQueue.Enqueue("movR");
    }

    public static void moveRight(int num){
        Instance.checkQueue();
        Instance.commandQueue.Enqueue("movR_Int");
        Instance.intArgQueue.Enqueue(num);
    }

    public static void turnUp(){
        Instance.checkQueue();
        Instance.commandQueue.Enqueue("rotU");
    }

    public static void turnDown(){
        Instance.checkQueue();
        Instance.commandQueue.Enqueue("rotD");
    }

    public static void turnLeft(){
        Instance.commandQueue.Enqueue("rotL");
    }

    public static void turnRight(){
        Instance.checkQueue();
        Instance.commandQueue.Enqueue("rotR");
    }

    public static void hold(){
        Instance.checkQueue();
        Instance.commandQueue.Enqueue("hold");
    }

    public static void release(){
        Instance.checkQueue();
        Instance.commandQueue.Enqueue("drop");
    }

    public static void interact(){
        Instance.checkQueue();
        Instance.commandQueue.Enqueue("act");
    }

    public static void read(){
        Instance.checkQueue();
        Instance.commandQueue.Enqueue("read");
    }
    
    public static void write(string input){
        Instance.checkQueue();
        Instance.stringArgQueue.Enqueue(input);
        Instance.commandQueue.Enqueue("write");
    }

    public static void say(string input){
        Instance.checkQueue();
        Instance.stringArgQueue.Enqueue(input);
        Instance.commandQueue.Enqueue("say");
    }

    //runs the command queue
    //if command queue is empty, just says "execution complete"
    public void execute(){
        setPlayerCharacter();
        if(commandQueue.Count > 0){
            debugDeclareInstruction();
            commandExecution(_commandQueue.Dequeue());
        } else if(commandQueue.Count == 0){
            terminateExecution();
            Debug.LogWarning("Execution completed successfully");
        }
    }

    //called by anything that should stop execution
    //invalid direction errors, stepping on a trap etc
    public void terminateExecution(){
        commandQueue.Clear();
        StartCoroutine(killTimer());
        Debug.LogAssertion("Execution terminated");
    }

    //delays the global coroutine stop by the global time to move
    //allows motion animations to finish before terminating
    private IEnumerator killTimer(){
        yield return new WaitForSeconds(Globals.Instance.timeToMove);
        foreach(var o in FindObjectsOfType<MonoBehaviour>()){
            o.StopAllCoroutines();
        }
    }
    
    //prints command currently being executed in console
    //this is just a debug method
    //called by execute(); can be taken out without consequence
    private void debugDeclareInstruction(){
        if(commandQueue.Peek().Contains("Int")){
            Debug.Log("Executing " + commandQueue.Peek() + " with args: " + intArgQueue.Peek());
        } else {
            Debug.Log("Executing " + commandQueue.Peek());
        }
    }

    //iterates through queue
    //calls methods from other scripts based on value of cmd
    public void commandExecution(string cmd){
        switch(cmd){
            case "movU":
                StartCoroutine(playerMovement.moveUp());
                break;
            case "movD":
                StartCoroutine(playerMovement.moveDown());
                break;
            case "movL":
                StartCoroutine(playerMovement.moveLeft());
                break;
            case "movR":
                StartCoroutine(playerMovement.moveRight());
                break;
            case "movU_Int":
                StartCoroutine(playerMovement.moveUp(intArgQueue.Dequeue()));
                break;
            case "movD_Int":
                StartCoroutine(playerMovement.moveDown(intArgQueue.Dequeue()));
                break;
            case "movL_Int":
                StartCoroutine(playerMovement.moveLeft(intArgQueue.Dequeue()));
                break;
            case "movR_Int":
                StartCoroutine(playerMovement.moveRight(intArgQueue.Dequeue()));
                break;
            case "rotU":
                StartCoroutine(playerMovement.turnUp());
                break;
            case "rotD":
                StartCoroutine(playerMovement.turnDown());
                break;
            case "rotL":
                StartCoroutine(playerMovement.turnLeft());
                break;
            case "rotR":
                StartCoroutine(playerMovement.turnRight());
                break;
            case "hold":
                StartCoroutine(playerInteraction.hold());
                break;
            case "drop":
                StartCoroutine(playerInteraction.release());
                break;
            case "act":
                StartCoroutine(playerInteraction.interact());
                break;
            case "read":
                StartCoroutine(playerInteraction.read());
                break;
            case "write":
                StartCoroutine(playerInteraction.write(stringArgQueue.Dequeue()));
                break;
            case "say":
                StartCoroutine(playerInteraction.say(stringArgQueue.Dequeue()));
                break;
            default:
                Debug.Log("void CommandExecution: No such instruction");
                break;
        }
    }
}

}