using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]private Vector3 offset;
    public float temp = 10;
    [SerializeField]private Transform target;
    [SerializeField]private float translateSpeed,rotationspeed;
    [HideInInspector]public float FAV;
    [HideInInspector]public float StartFOV;
    private Camera camera;
    private void Start()
    {
        camera = GetComponent<Camera>();
        FAV = camera.fieldOfView;
        StartFOV = FAV;
    }
    private void FixedUpdate()
    {
        HandleTranslation();
    }
    private void Update()
    {
        Debug.Log(FAV);
        camera.fieldOfView = Mathf.Lerp(camera.fieldOfView , FAV,temp * Time.deltaTime);    
    }   
    private void HandleTranslation() 
    {
        var targetposition = target.TransformPoint(offset);
        transform.position = Vector3.Lerp(transform.position,targetposition,translateSpeed * Time.deltaTime);
    }
    /*private void HandleRotation() 
    {
        var direction = target.position - transform.position;
        var rotation = Quaternion.LookRotation(direction,Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation,rotation,rotationspeed * Time.deltaTime);
    }*/
}
