using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ButtonClick : MonoBehaviour
{
    public float alphaThresHold = 0.1f;
    private void Start()
    {
       this.GetComponent<Image>().alphaHitTestMinimumThreshold = alphaThresHold; 
    }
}
