using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimitedQueue<T> : Queue<T>
{
    public int Limit = 20;

    public new void Enqueue(T item){
        if (Count == Limit){
            Debug.Log("max hit");
        } else {
            base.Enqueue(item);
        }
    }
}
