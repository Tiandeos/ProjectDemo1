using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedoMeter : MonoBehaviour
{
    [SerializeField]private CarController carController;
    [SerializeField]private Transform needleTransform;
    private float endPosition = -225;
    private float startPosition = -40;
    private float desiredPosition;
    private float speed;
    private float maxspeed;
    private void Awake()
    {

    }
    // Update is called once per frame
    void FixedUpdate()
    {
        GetSpeed();
        UpdateNeedle();
    }
    private void GetSpeed() 
    {
        speed = carController.speed;
        maxspeed =  carController.topspeed;
    }
    private void UpdateNeedle() 
    {
        desiredPosition = startPosition - endPosition;
        float temp = speed / 180;
        needleTransform.eulerAngles = new Vector3(0,0,startPosition - temp * desiredPosition);
    }
}
