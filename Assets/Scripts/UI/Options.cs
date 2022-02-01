using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Options : MonoBehaviour
{
    public GameData gameData;
    [SerializeField]private GameObject ControlPanel;
    [SerializeField]private GameObject DefaultOptionPanel;
    private void Start()
    {
        gameData = SaveSystem.Load();
    }
    public void OnClickEnterControls() 
    {
        ControlPanel.SetActive(true);
        DefaultOptionPanel.SetActive(false);
    }
    public void OnClickEnterSounds() 
    {

    }
    public void OnClickEnterConnectStore() 
    {

    }
    public void OnClickEnterLanguage() 
    {

    }
    #region ControlOptions
    public void OnClickEnterButtons() 
    {
        gameData.ControlType = 0;
        SaveSystem.Save(gameData);
    }
    public void OnClickEnterSteeringWheel() 
    {
        gameData.ControlType = 1;
        SaveSystem.Save(gameData);
    }
    #endregion
}
