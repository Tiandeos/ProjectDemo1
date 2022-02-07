using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGeneration : MonoBehaviour
{
    [SerializeField]private Transform LastGenerationpoint;
    private int createdroadnumber,amntilesonscreen = 7;
    private float Safezone = 65.5f,spawnz = -62.5f,tilelenght = 62.5f;
    private List<GameObject> Activeroads;
    [SerializeField]private GameObject[] Road;
    [SerializeField]private Transform player;
    [SerializeField]private Transform[] playeropponents;
    [SerializeField]private Vector3 lastendposition;
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
    }
    private void CreateRoads(int prefabindex = -1)
    {
        createdroadnumber++;
        if(createdroadnumber < 15) 
        {
            GameObject go;
            go = Instantiate(Road[1]) as GameObject;
            go.transform.SetParent(transform);
            go.transform.position = Vector3.forward * spawnz;
            spawnz += tilelenght;
            Activeroads.Add(go);
        }
        else 
        {
            GameObject go;
            go = Instantiate(Road[0]) as GameObject;
            go.transform.SetParent(transform);
            go.transform.position = Vector3.forward * spawnz;
            spawnz += tilelenght;
            Activeroads.Add(go);
        }
    }
    private void DestroyRoads() 
    {
       Destroy(Activeroads[0]);
       Activeroads.RemoveAt(0);
    }

}
