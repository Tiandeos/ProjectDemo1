using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGeneration : MonoBehaviour
{
    [SerializeField]private Transform LastGenerationpoint;
    private int createdroadnumber,createdroadnumber2,amntilesonscreen = 6;
    private float Safezone = 65.5f,spawnz = -62.5f,tilelenght = 62.5f;
    private List<GameObject> Activeroads;
    private List<GameObject> ActiveCars;
    private List<int> RandomNumbers;
    [SerializeField]private GameObject[] Road;
    [SerializeField]private GameObject[] TrafficCars;
    [SerializeField]private Transform player;
    [SerializeField]private Vector3 nextroadposition;
    int createdroads;
    private bool RandomNumberAttached = false;
    bool secondroad;
    private int RandomRoad;
    private void Start()
    {
        if(!RandomNumberAttached) 
        {
            RandomRoad = RandomNumberCreator();
        }
        Activeroads = new List<GameObject>();
        ActiveCars = new List<GameObject>();
        RandomNumbers = new List<int>(new int[3]);
        for(int i = 0; i < amntilesonscreen; i++) 
        {
            CreateRoads();
            
        }
    }
    private void Update()
    {
        if(!RandomNumberAttached) 
        {
            RandomRoad = RandomNumberCreator();
        }
        if(player.position.z - Safezone > (spawnz - amntilesonscreen * tilelenght)) 
        {
            CreateRoads();
            DestroyRoads();
            if(createdroads > 5) 
            {
                CreateTrafficCars();
            }
        }
    }
    private void CreateRoads(int prefabindex = -1)
    {
        createdroads++;            
        if(!secondroad) 
        {
            createdroadnumber++;
            GameObject go;
            go = Instantiate(Road[1]) as GameObject;
            go.transform.SetParent(transform);
            go.transform.position = Vector3.forward * spawnz;
            spawnz += tilelenght;
            nextroadposition = new Vector3(0,0,spawnz);
            Activeroads.Add(go);
        }
        else        
        {
            createdroadnumber2++;
            GameObject go;
            go = Instantiate(Road[0]) as GameObject;
            go.transform.SetParent(transform);
            go.transform.position = new Vector3(0,-0.45f,spawnz );
            spawnz += tilelenght;
            nextroadposition = new Vector3(0,0,spawnz);
            Activeroads.Add(go);
        }
        if(createdroadnumber2 >= RandomRoad) 
        {
            createdroadnumber2 = 0;
            createdroadnumber = 0;
            RandomNumberAttached = false;
            secondroad = false;
        }
        if(createdroadnumber >= RandomRoad) 
        {
            createdroadnumber2 = 0;
            createdroadnumber = 0;
            RandomNumberAttached = false;
            secondroad = true;
        }
    }
    private void DestroyRoads() 
    {
        Destroy(Activeroads[0]);
        Activeroads.RemoveAt(0);
    }
    private void DestroyCars() 
    {
        Destroy(ActiveCars[0]);
        ActiveCars.RemoveAt(0);
    }
    private void CreateTrafficCars() 
    {
        Debug.Log("Spawnz:" + spawnz);
        //-16.25 = 0 -6.25 = 1 6.25 = 2 16.25 = 3
        createdroads = 0;
        GameObject go1;
        float x = 0;
        for(int i = 0;i < 3;i++) 
        {
            int a = Random.Range(0,4);
            while(RandomNumbers.Contains(a)) 
            {
                a = Random.Range(0,4);
            }          
            RandomNumbers.Add(a);  
            switch(a) 
            {
                case 0:
                x = -15.25f;
                break;
                case 1:
                x = -5.25f;
                break;
                case 2:
                x = 5.25f;
                break;
                case 3:
                x = 15.25f;
                break;
            }
            go1 = Instantiate(TrafficCars[0]) as GameObject;
            go1.transform.position = new Vector3(x,0.8f,spawnz -62.5f);
            go1.AddComponent<TrafficAI>();
            ActiveCars.Add(go1);
        }
        RandomNumbers.Clear();
    }
    private int RandomNumberCreator() 
    {
        int RandomNumber;
        RandomNumberAttached = true;
        RandomNumber = Random.Range(5,16);
        return RandomNumber;
    }
}
