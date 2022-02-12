using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class GameManager : MonoBehaviour
{
    #region DebugUI
    public TextMeshProUGUI LaneText;
    public TextMeshProUGUI ChangingTimeText;
    public TextMeshProUGUI SteerAngleText;
    public TextMeshProUGUI IsChangingStripText;
    public TextMeshProUGUI OnTouchText;
    public TextMeshProUGUI HorizontalText;
    public TextMeshProUGUI NitroText;
    public TextMeshProUGUI CanUseNitroText;
    public TextMeshProUGUI GearText;
    public TextMeshProUGUI MotorForceText;
    public TextMeshProUGUI MinSpeedText;
    public TextMeshProUGUI MaxSpeedText;
    #endregion
    [SerializeField] private CarController CarController;
    [SerializeField] private Text kph,gearnum;
    [SerializeField] private TextMeshProUGUI StartGameText;
    [SerializeField] private GameObject StartGameTextObject;
    public GameObject needle;
    public int MaxLane = 1;
    private float Timer;
    private string Winner;
    public GameData gameData;
    private float startPosition = 220f,endPosition = -41f ,desiredPosition,minspeedang  = 190.0f;
    private float maxpeedang = -80.0f;
    [HideInInspector]public bool IsGameStarted;
    private void Awake()
    {
        gameData = SaveSystem.Load();
    }
    private void Start()
    {
        IsGameStarted = false;
        GetPlayerData();
    }
    private void GetPlayerData() 
    {
        if(gameData.laneChangeType == GameData.LaneChangeType.StrictLaneChange) 
        {
            CarController.laneChangeType = CarController.LaneChangeType.StrictLaneChange;
        }
        else if(gameData.laneChangeType == GameData.LaneChangeType.FreeLaneChange) 
        {
            CarController.laneChangeType = CarController.LaneChangeType.FreeLaneChange;
        }
    }

    private void Update()
    {
        if(!IsGameStarted) 
        {
            Timer += Time.deltaTime;
            if(Timer >= 3) 
            {
                IsGameStarted = true;
                StartGameTextObject.SetActive(false);
            }
            UpdateText();
        }
        //kph.text = CarController.speed.ToString("0");
        updateNeedle();
      
    }
    private void UpdateText() 
    {
        StartGameText.text = ((int)Timer).ToString();
    }

    private void updateNeedle() 
    {
        desiredPosition = startPosition - endPosition;

    }  
    public void ChangeGear() 
    {
        gearnum.text = CarController.GearLevel.ToString();   
    }
  
}
