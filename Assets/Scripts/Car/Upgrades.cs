using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Upgrades : MonoBehaviour
{
    //Yol tutuşu = maks hızı etkiliyor(azsa maks hız düşer fazlaysa artar). eğer yol tutuşu az kalırsa hızlanma artmıyor. 
    //Yol tutuşu =  +5toph +5hız sonseviyede = +15toph + 10hız
    //Son seviyede hızlanma ve maks hız artıyor.
    //Ufak formül hızlanmayı 20 ile çarp maks hızı 2 ile böl. yol tutuşu için eksilt 15 sonra bunu 10000'ne böl 0.15 çıkart mevcut gripten bitti gitti tamam
    //Upgrade sırası 
    //Maksimum seviye 10
    [SerializeField]private int NitroLevel = 0;//Nitro boost artır, 2 maks
    [SerializeField]private int EngineLevel = 0;//Maks hız 15 + hızlanma 10
    [SerializeField]private int TireLevel = 0;//Yol tutuşu 15 + hızlanma 5
    [SerializeField]private int AeroDynamic = 0;//Hızlanma 5 + maks Hız 5 + Yol tutuşu 10
    [SerializeField]private int EcuLevel = 0; //Top hız 15 + Hızlanma 10 
    [SerializeField]private int SuspansionLevel = 0;//Yol tutuşu 15, Maks hız 5
    [SerializeField]private int TransMissionLevel = 0;//Maks hız 10 + hızlanma 5
    public float boostForce;
    public float StockNitro;
    public int PlayerCarID;
    public Camera camera;
    GameData gameData;
    CameraController cameraController;
    CarController carController;
    private void Awake()
    {
        gameData = SaveSystem.Load();
        NitroLevel = gameData.NitroLevel[PlayerCarID];
        EngineLevel = gameData.EngineLevel[PlayerCarID];
        TireLevel = gameData.TireLevel[PlayerCarID];
        AeroDynamic = gameData.AeroDynamicLevel[PlayerCarID];
        SuspansionLevel = gameData.SuspansionLevel[PlayerCarID];
        TransMissionLevel = gameData.TransmissionLevel[PlayerCarID];
        EcuLevel = gameData.EcuLevel[PlayerCarID];
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
            boostForce = NitroLevel * 2500;
            StockNitro = NitroLevel  * 250;
        }
        else 
        {
            boostForce = 1250;
            StockNitro = 200;
        }
        if(EngineLevel != 0) 
        {
            carController.topspeed += 7.5f * EngineLevel;
            carController.StartMotorForce += 200 * EngineLevel;
        }
        if(TireLevel != 0) 
        {
            carController.StartMotorForce += 100 * TireLevel;
        } 
        if(SuspansionLevel != 0) 
        {
            carController.topspeed += 2.5f * SuspansionLevel;
        }
        if(EcuLevel != 0) 
        {
            carController.StartMotorForce += 200 * EcuLevel;
            carController.topspeed += 7.5f;
        }
        if(TransMissionLevel != 0) 
        {
            carController.topspeed += 10 * TransMissionLevel;
        }
        if(AeroDynamic != 0) 
        {
            carController.topspeed += 2.5f * AeroDynamic;
            carController.StartMotorForce += 100 * AeroDynamic;
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
