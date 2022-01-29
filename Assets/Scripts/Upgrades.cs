using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrades : MonoBehaviour
{
    public float boostForce;
    public Camera camera;
    CameraController cameraController;
    CarController carController;
    private void Start()
    {
        camera = FindObjectOfType<Camera>();
        carController = GetComponent<CarController>();
        cameraController= camera.GetComponent<CameraController>();
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Z) && !carController.IsUsingNitro && carController.CanUseNitro) 
        {
            carController.IsUsingNitro = true;
            Debug.Log("Fov plus");
            cameraController.FAV += 20;            
        }
        else if(carController.IsFinishedUsingNitro) 
        {
            Debug.Log("Nitro Use Finished");
            cameraController.FAV = cameraController.StartFOV;
            carController.IsFinishedUsingNitro = false;
        }
    }
}
