using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    public class TrafficAI : MonoBehaviour
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
            Debug.Log("A");
            carController = GetComponent<CarController>();
            lane = 1;
            side = "right";
            rigidBody = GetComponent<Rigidbody>();
        }
        public void ActivateEnemy() 
        {
            gameObject.SetActive(true);
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
                if(timer >= 4.5f) 
                {
                    CanChangeGear = true;
                }
            }
        }
        private void HandleMotorAI() 
        {
            if(!IsBreaking) 
            {
                vertical = 1;
            }
            else if(IsBreaking)
            {
                vertical = -1;
            }
            if(carController.speed > 70) 
            {
                vertical = 0;
            }            
            if(carController.speed > carController.maxspeed) 
            {
                CanTimerStart = true;
            }
            if(carController.speed > carController.maxspeed && carController.GearLevel < carController.MaxGearLevel && CanChangeGear) 
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
