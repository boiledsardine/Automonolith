using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialKeys : MonoBehaviour{
    [SerializeField] private float repeatSpeed = 0.2f;
    [SerializeField] private float keyDownDelay = 0.5f;
    List<KeyCode> registeredKeys = new List<KeyCode>();
    Dictionary<KeyCode, KeyState> keys = new Dictionary<KeyCode, KeyState>();

    public bool getKeyPress(KeyCode key){

        if(Input.GetKey(key)){
            KeyState state = keys[key];
            
            if(Input.GetKeyDown(key)){
                state.lastPressTime = Time.time;
                return true;
            }

            float timeSinceLastPress = Time.time - state.lastPressTime;

            if(timeSinceLastPress > keyDownDelay){
                float timeSinceLastTick = Time.time - state.lastTickTime;

                if(timeSinceLastTick > repeatSpeed){
                    state.lastTickTime = Time.time;
                    return true;
                }
            }
        }

        return false;
    }

    public void registerSpecialKey(KeyCode key){
        if(!registeredKeys.Contains(key)){
            registeredKeys.Add(key);
            KeyState state = new KeyState(key);
            keys.Add(key, state);
        }        
    }
    
    public class KeyState{
        public KeyState(KeyCode key){
            this.key = key;
        }
        public readonly KeyCode key;
        public float lastTickTime;
        public float lastPressTime;
    }
}