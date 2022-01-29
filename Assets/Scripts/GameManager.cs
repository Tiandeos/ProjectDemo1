using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class GameManager : MonoBehaviour
{
    [SerializeField] private CarController CarController;
    [SerializeField] private Text kph,gearnum;
    [SerializeField] private TextMeshProUGUI StartGameText;
    [SerializeField] private GameObject StartGameTextObject;
    public GameObject needle;
    public int MaxLane = 1;
    private float Timer;
    private string Winner;
    private float startPosition = 220f,endPosition = -41f ,desiredPosition,minspeedang  = 190.0f, maxpeedang = -80.0f;
    [HideInInspector]public bool IsGameStarted;
    private void Start()
    {
        IsGameStarted = false;
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
        kph.text = CarController.speed.ToString("0");
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
