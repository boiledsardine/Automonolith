using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CommandControl{

public class Bot : MonoBehaviour {
    public static Bot Instance { get; private set; }
    
    private Queue<string> commandQueue = new Queue<string>();
    private Queue<int> intArgQueue = new Queue<int>();

    private GameObject playerCharacter;
    private Vector3 originPos;

    void Awake(){
        if(Instance == null){
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    public GameObject getPC(){
        return playerCharacter;
    }

    public Vector3 getPCOriginPos(){
        return originPos;
    }

    public Queue<string> getCommandQueue(){
        return commandQueue;
    }

    public Queue<int> getIntQueue(){
        return intArgQueue;
    }

    public static void moveUp(){
        Instance.commandQueue.Enqueue("movU");
    }

    public static void moveUp(int num){
        Instance.commandQueue.Enqueue("movU_Int");
        Instance.intArgQueue.Enqueue(num);
    }

    public static void moveDown(){
        Instance.commandQueue.Enqueue("movD");
    }

    public static void moveDown(int num){
        Instance.commandQueue.Enqueue("movD_Int");
        Instance.intArgQueue.Enqueue(num);
    }

    public static void moveLeft(){
        Instance.commandQueue.Enqueue("movL");
    }

    public static void moveLeft(int num){
        Instance.commandQueue.Enqueue("movL_Int");
        Instance.intArgQueue.Enqueue(num);
    }

    public static void moveRight(){
        Instance.commandQueue.Enqueue("movR");
    }

    public static void moveRight(int num){
        Instance.commandQueue.Enqueue("movR_Int");
        Instance.intArgQueue.Enqueue(num);
    }

    public static void stand(){
        Instance.commandQueue.Enqueue("stand");
    }

    public void execute(){
        if(commandQueue.Count > 0){
            declareInstruction();
            commandExecution(commandQueue.Dequeue());
        } else if(commandQueue.Count == 0){
            Environment.Instance.resetStartTile();
            Debug.LogWarning("Execution complete");
        }
    }

    public void terminateExecution(){
        commandQueue.Clear();
        Environment.Instance.resetStartTile();
        Debug.LogAssertion("Execution terminated");
    }

    private void declareInstruction(){
        if(commandQueue.Peek().Contains("Int")){
            Debug.Log("Executing " + commandQueue.Peek() + " with args: " + intArgQueue.Peek());
        } else {
            Debug.Log("Executing " + commandQueue.Peek());
        }
    }

    public void commandExecution(string cmd){
        switch(cmd){
            case "movU":
                Movement.Instance.moveUp();
                break;
            case "movU_Int":
                Movement.Instance.moveUp(Instance.intArgQueue.Dequeue());
                break;
            case "movD":
                Movement.Instance.moveDown();
                break;
            case "movD_Int":
                Movement.Instance.moveDown(Instance.intArgQueue.Dequeue());
                break;
            case "movL":
                Movement.Instance.moveLeft();
                break;
            case "movL_Int":
                Movement.Instance.moveLeft(Instance.intArgQueue.Dequeue());
                break;
            case "movR":
                Movement.Instance.moveRight();
                break;
            case "movR_Int":
                Movement.Instance.moveRight(Instance.intArgQueue.Dequeue());
                break;
            case "stand":
                Movement.Instance.stand();
                break;
            default:
                Debug.Log("void CommandExecution: No such instruction");
                break;
        }
    }
}

}