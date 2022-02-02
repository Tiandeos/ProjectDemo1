using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private CarController carController;
    public bool RightPressed;
    public bool LeftPressed;
    public bool UpPressed;
    public bool DownPressed;
    public bool GearUpgraded;
    public bool NitroUsing;
    private void Start()
    {
        carController = GetComponent<CarController>();
    }
    #region ForEditorControls(Pc)
    private void Update()
    {
         
    }
    
    #endregion
    #region MobileControl
    public void OnClickEnterLeft() 
    {
        LeftPressed = true;
    }
    public void OnClickEnterRight() 
    {
        RightPressed = true;
    }
    public void OnClickEnterGas() 
    {
        UpPressed = true;
    }
    public void OnClickEnterBreak() 
    {
        DownPressed = true;
    }
    public void OnClickEnterGearUpgrade() 
    {
        if(carController.speed > carController.maxspeed - 5 && carController.GearLevel <= 3)
        {
            GearUpgraded = true;
        }
    }
    public void OnClickEnterNitro() 
    {
        NitroUsing = true;
    }
    #endregion

}
