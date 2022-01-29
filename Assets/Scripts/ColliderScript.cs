using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderScript : MonoBehaviour
{
    private CarController carController;
    private CarAI carAI;
    private void Start()
    {
        GetObjects();
    }
    private void GetObjects() 
    {
        carController = GetComponent<CarController>();
        if(transform.tag == "Opponent") 
        {
            carAI = GetComponent<CarAI>();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag != "Player" && transform.tag != "Player") {
            if(carAI.side == "right" && !carController.IsChangingStrip) 
            {
                if(transform.tag == "Opponent")
                { 
                    carAI.side = "left";
                    carController.IsChangingStrip= true;
                    carController.ChangeLane(carAI.side);
                }
            }
            else if(carAI.side == "left" && !carController.IsChangingStrip) 
            {
                carAI.side = "right";
                carController.IsChangingStrip = true;
                carController.ChangeLane(carAI.side);
            }
        }
        if(transform.tag != "CheckTrigger" && other.tag == "Finish") 
        {
            Debug.Log("Race Succesfully finished");
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if(other.tag != "Player" && transform.tag != "Player") {
            if(carAI.side == "right" && !carController.IsChangingStrip) 
            {
                Debug.Log("A");
                if(transform.tag == "Opponent")
                { 
                    carAI.side = "left";
                    carController.IsChangingStrip= true;
                    carController.ChangeLane(carAI.side);
                }
            }
        }
    }
    private void OnTriggerExit(Collider other) 
    {

    }
    private void OnCollisionEnter(Collision other) 
    {
        if(other.gameObject.tag != "Obstacle") 
        {
            carController.IsCarTouching = true;
            carController.LockPosition(carController.IsCarTouching);
        }
    }
    private void OnCollisionExit(Collision other)
    {
        carController.IsCarTouching = false;
        carController.IsCarStoppedTouching = true;
        if(!carController.IsChangingStrip) 
        {
            carController.LockPosition(carController.IsCarTouching);
        }
    }
}
