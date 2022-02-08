using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGeneration : MonoBehaviour
{
    [SerializeField]private Transform LastGenerationpoint;
    private int createdroadnumber,createdroadnumber2,amntilesonscreen = 7;
    private float Safezone = 65.5f,spawnz = -62.5f,tilelenght = 62.5f;
    private List<GameObject> Activeroads;
    [SerializeField]private GameObject[] Road;
    [SerializeField]private Transform player;
    [SerializeField]private Transform[] playeropponents;
    [SerializeField]private Vector3 lastendposition;
    private bool RandomNumberAttached = false;
    private int RandomRoad;
    private void Start()
    {
        Activeroads = new List<GameObject>();
        for(int i = 0; i < amntilesonscreen; i++) 
        {
           CreateRoads();
        }
    }
    private void Update()
    {
        if(player.position.z - Safezone > (spawnz - amntilesonscreen * tilelenght)) 
        {
            CreateRoads();
            DestroyRoads();
        }
        if(!RandomNumberAttached) 
        {
            RandomNumberCreator(RandomRoad);
        }
    }
    private void CreateRoads(int prefabindex = -1)
    {
        if(createdroadnumber < RandomRoad) 
        {
            createdroadnumber++;
            GameObject go;
            go = Instantiate(Road[0]) as GameObject;
            go.transform.SetParent(transform);
            go.transform.position = Vector3.forward * spawnz;
            spawnz += tilelenght;
            Activeroads.Add(go);
        }
        else 
        {
            createdroadnumber2++;
            GameObject go;
            go = Instantiate(Road[1]) as GameObject;
            go.transform.SetParent(transform);
            go.transform.position = Vector3.forward * spawnz;
            spawnz += tilelenght;
            Activeroads.Add(go);
        }
        if(createdroadnumber2 >= RandomRoad) 
        {
            createdroadnumber2 = 0;
            createdroadnumber = 0;
            RandomNumberAttached = false;
        }
         if(createdroadnumber >= RandomRoad) 
        {
            createdroadnumber2 = 0;
            RandomNumberAttached = false;
        }
    }
    private void DestroyRoads() 
    {
       Destroy(Activeroads[0]);
       Activeroads.RemoveAt(0);
    }
    private int RandomNumberCreator(int RandomNumber) 
    {
        RandomNumberAttached = true;
        RandomNumber = Random.Range(5,16);
        return RandomNumber;
    }
}
