using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemPickup : ObjectBase{
    [SerializeField] private float degPerSec = 30f;
    public float yPos = 30f;

    private new void Start(){
        base.Start();
        isMovable = false;
    }

    public void TriggerPickup(){
        var player = GameObject.Find("PlayerCharacter");
        OnTriggerEnter(player.GetComponent<Collider>());
    }

    private void OnTriggerEnter(Collider col) {
        if(col.gameObject.tag == "Player"){
            Debug.Log("Gem picked up!");
            var player = GameObject.Find("PlayerCharacter").transform.position;
            transform.position = new Vector3(player.x, Globals.Instance.pickupYFixedPos, player.z);
            StartCoroutine(FadePickup());
        }
    }

    private void StartFade(){
        StartCoroutine(FadePickup());
    }

    private IEnumerator FadePickup(){
        var animator = gameObject.GetComponent<Animator>();
        yield return StartCoroutine(MoveUp());
        Destroy(gameObject);
    }

    private void Update(){
        transform.Rotate(new Vector3(0f, Time.deltaTime * degPerSec, 0f), Space.World);
    }

    public IEnumerator MoveUp(){
        float timeElapsed = 0;
        var originPos = transform.position;
        
        var moveDir = new Vector3(transform.position.x, Globals.Instance.pickupYFixedPos + yPos, transform.position.z);
        var targetPos = moveDir;

        while(timeElapsed < Globals.Instance.timeToMove){
            transform.position = Vector3.Lerp(originPos, targetPos,
            (timeElapsed / Globals.Instance.timeToMove));
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPos;
    }
}
