using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGenerator : ObjectBase{
    private ObjectEnvironment envirScript;
    [SerializeField] private GameObject spawnPoint;
    public List<GameObject> itemCube; //0 - blue, 1 - red, 2 - green
    public bool randomize;
    public List<int> cubeCounts; //0 - blue, 1 - red, 2 - green
    List<int> storedCubes;

    private new void Start(){
        if(randomize){
            storedCubes = new List<int>();
            //get blue count
            for(int i = 0; i < cubeCounts[0]; i++){
                storedCubes.Add(0);
            }
            //get red count
            for(int i = 0; i < cubeCounts[1]; i++){
                storedCubes.Add(1);
            }
            //get green count
            for(int i = 0; i < cubeCounts[2]; i++){
                storedCubes.Add(2);
            }
            storedCubes.Shuffle();
        }
        
        isMovable = false;
        envirScript = gameObject.GetComponent<ObjectEnvironment>();
        //envirScript.tileUnder.isOccupied = true;
        if(!randomize){
            StartCoroutine(CreateCube());
        } else {
            System.Random rnd = new System.Random();
            int randNum = rnd.Next(storedCubes.Count);
            StartCoroutine(CreateCube(storedCubes[randNum]));
            storedCubes.RemoveAt(randNum);
        }
    }

    //create object
    void OnTriggerExit(Collider col){
        if(randomize && storedCubes.Count > 0){
            System.Random rnd = new System.Random();
            int randNum = rnd.Next(storedCubes.Count);
            StartCoroutine(CreateCube(storedCubes[randNum]));
            storedCubes.RemoveAt(randNum);
        } else {
            StartCoroutine(CreateCube());
        }
    }

    IEnumerator CreateCube(){
        yield return new WaitForSeconds(Globals.Instance.timePerStep);       
        GameObject cube = Instantiate(itemCube[0], spawnPoint.transform.position, spawnPoint.transform.rotation);
        cube.name = "CubeKey";
    }

    IEnumerator CreateCube(int color){
        yield return new WaitForSeconds(Globals.Instance.timePerStep);       
        GameObject cube = Instantiate(itemCube[color], spawnPoint.transform.position, spawnPoint.transform.rotation);
        cube.name = "CubeKey";
    }
}