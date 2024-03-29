using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderScript : MonoBehaviour
{
    private CarController carController;
    private CarAI carAI;
    [SerializeField] private GameObject RetryPanel;
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
            carController.IsChangingStrip= true;    
            Debug.Log("Change ?");
            Debug.Log(carAI.lane);
            if(carAI.lane == 1 && !carController.IsChangingStrip) 
            {
                if(transform.tag == "Opponent")
                { 
                    carAI.side = "left";
                    carController.ChangeLane(carAI.side);
                }
            }
            else if(carAI.lane == 0 && !carController.IsChangingStrip) 
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
        if(other.tag == "Finish"  && transform.tag != "CheckTrigger") 
        {
            Debug.Log("Race Succesfully finished");
        } 
    }
    private void OnCollisionEnter(Collision other) 
    {
        if(other.gameObject.tag != "Obstacle") 
        {
            carController.IsCarTouching = true;
            carController.LockPosition(carController.IsCarTouching);
        }
        if(other.gameObject.tag == "Obstacle" && transform.tag == "Player") 
        {
            RetryPanel.SetActive(true);
        }
        if(other.gameObject.tag == "Destroy" && transform.tag == "TrafficAI") 
        {
            Destroy(gameObject);
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
