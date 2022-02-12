using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class InputManager : MonoBehaviour
{
    private CarController carController;
    [SerializeField]private TextMeshProUGUI fps;
    public bool RightPressed;
    public bool LeftPressed;
    public bool UpPressed;
    public bool DownPressed;    
    public bool GearUpgraded;
    public bool GearDowngraded;
    public bool NitroUsing;    
    public int avgFrameRate;

    private void Start()
    {
        carController = GameObject.FindGameObjectWithTag("Player").GetComponent<CarController>();
    }
    #region ForEditorControls(Pc)
    private void Update()
    {
        float current = 0;
        current = Time.frameCount / Time.time;
        avgFrameRate = (int)current;
        fps.text = avgFrameRate.ToString() + " FPS";
    }  
    
    #endregion
    #region MobileControl
    public void OnClickEnterLeft(bool Clicked) 
    {
        LeftPressed = Clicked;
    }
    
    public void OnClickEnterRight(bool Clicked) 
    {
        RightPressed = Clicked;
    }
    public void OnClickEnterGas(bool Clicked) 
    {
        UpPressed = Clicked;
    }
    public void OnClickEnterBreak(bool Clicked) 
    {
        DownPressed = Clicked;
    }
    public void OnClickEnterGearUpgrade() 
    {
        if(carController.speed > carController.maxspeed - 5 && carController.GearLevel <= carController.MaxGearLevel)
        {
            GearUpgraded = true;
            GearDowngraded = false;
        }
    }
    public void OnClickEnterGearDown() 
    {
        if(carController.GearLevel != 0) 
        {
            GearUpgraded = false;
            GearDowngraded = true;
        }
    }
    public void OnClickEnterRetry() 
    {
        SceneManager.LoadScene("InfiniteMode");
    }

    public void OnClickEnterNitro(bool Clicked) 
    {
        NitroUsing = Clicked;
    }
    #endregion

}
