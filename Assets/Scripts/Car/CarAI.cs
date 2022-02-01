using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAI : MonoBehaviour
{
    [HideInInspector]public string side = "right";
    private bool IsBreaking;
    private bool CanChangeGear;
    private bool CanTimerStart;
    private float vertical;
    private float timer;
    [HideInInspector]public bool IsChangingStrip;
    [HideInInspector]public int lane;
    [HideInInspector]public float zCoordinate;
    [HideInInspector]public Rigidbody rigidBody;
    [SerializeField]private Vector3 Target;
    private float distancetotarget;
    private CarController carController;
    void Start()
    {
        GetObjects();
    } 
    private void GetObjects() 
    {
        carController = GetComponent<CarController>();
        lane = 1;
        side = "right";
        rigidBody = GetComponent<Rigidbody>();
    }
    void Update()
    {
        if(carController.gameManager.IsGameStarted) 
        {
            FixVariables();
            HandleMotorAI();
        }
        if(CanTimerStart)
        {
            timer += Time.deltaTime;
            if(timer >= 2.5f) 
            {
                CanChangeGear = true;
            }
        }
    }
    private void HandleMotorAI() 
    {
        #region Break
        if(!IsBreaking) 
        {
            vertical = 0.5f;
        }
        else if(IsBreaking)
        {
            vertical = -0.5f;
        }
        #endregion
        
        if(carController.speed > carController.maxspeed) 
        {
            CanTimerStart = true;
        }
        if(carController.speed > carController.maxspeed && carController.GearLevel < 4 && CanChangeGear) 
        {
            CanChangeGear = false;
            CanTimerStart = false;
            timer = 0;
            carController.ChangeGearLevel(true);
        }
        carController.GetInput(vertical,carController.horizontalright);
    }
    public void FixVariables() 
    {
        zCoordinate = carController.zCoordinate;
        lane = carController.lane;
        Debug.Log(lane);
        IsChangingStrip = carController.IsChangingStrip;
        carController.rigidBody.drag = rigidBody.drag;
    }
}
