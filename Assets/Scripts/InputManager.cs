using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    private CarController carController;
    public bool RightPressed;
    public bool LeftPressed;
    public bool UpPressed;
    public bool DownPressed;    
    public bool GearUpgraded;
    public bool GearDowngraded;
    public bool NitroUsing;
    private void Start()
    {
        carController = GameObject.FindGameObjectWithTag("Player").GetComponent<CarController>();
    }
    #region ForEditorControls(Pc)
    private void Update()
    {
         
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
        Debug.Log("Clicked: " + Clicked);
        Debug.Log("Right pressed : "+ RightPressed);
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
        if(carController.speed > carController.maxspeed - 5 && carController.GearLevel <= 3)
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

    public void OnClickEnterNitro() 
    {
        NitroUsing = true;
    }
    #endregion

}
