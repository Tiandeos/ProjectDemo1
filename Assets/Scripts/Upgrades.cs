using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Upgrades : MonoBehaviour
{
    [SerializeField]private int NitroLevel = 0;//Nitro boost artır
    [SerializeField]private int EngineLevel = 0;//Maks hız + hızlanma
    [SerializeField]private int TireLevel = 0;//Yol tutuşu + hızlanma
    [SerializeField]private int AeroDynamic = 0;//Hızlanma + maks Hız
    [SerializeField]private int SuspansionLevel = 0;//Yol tutuşu Maks hız
    [SerializeField]private int TransMissionLevel = 0;//Maks hız + hızlanma ??
    public float boostForce;
    public float StockNitro;
    public Camera camera;
    GameData gameData;
    CameraController cameraController;
    CarController carController;
    private void Awake()
    {
        gameData = SaveSystem.Load();
        NitroLevel = gameData.NitroLevel[0];
        EngineLevel = gameData.EngineLevel[0];
        TireLevel = gameData.TireLevel[0];
        AeroDynamic = gameData.AeroDynamicLevel[0];
        SuspansionLevel = gameData.SuspansionLevel[0];
        TransMissionLevel = gameData.TransmissionLevel[0];
    }
    private void Start()
    {
        GetObjects();
        UpgradeManaging();
    }  
    private void GetObjects() 
    {
        camera = FindObjectOfType<Camera>();
        carController = GetComponent<CarController>();
        cameraController= camera.GetComponent<CameraController>();
    }
    private void UpgradeManaging() 
    {
        if(NitroLevel != 0) 
        {
            boostForce = NitroLevel * 5000;
            StockNitro = NitroLevel  * 250;
        }
        else 
        {
            boostForce = 2500;
            StockNitro = 150;
        }
        if(EngineLevel != 0) 
        {
            carController.maxspeed += 15 * EngineLevel;
        }
        if(TireLevel != 0) 
        {
            
        } 
        else 
        {

        }
        if(SuspansionLevel != 0) 
        {
            carController.maxspeed += 10 * SuspansionLevel;
        }
        else 
        {
            
        }
        if(TransMissionLevel != 0) 
        {
            carController.maxspeed += 10 * SuspansionLevel;
        }
        if(AeroDynamic != 0) 
        {
            carController.maxspeed += 10 * AeroDynamic;
        }   
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Z) && !carController.IsUsingNitro && carController.CanUseNitro) 
        {
            carController.IsUsingNitro = true;
            Debug.Log("Fov plus");
            cameraController.FAV += 20;            
        }
        else if(carController.IsFinishedUsingNitro) 
        {
            Debug.Log("Nitro Use Finished");
            cameraController.FAV = cameraController.StartFOV;
            carController.IsFinishedUsingNitro = false;
        }
    }
}
