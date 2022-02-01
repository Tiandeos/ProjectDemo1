using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameData gameData;
    void Awake()
    {
        gameData = SaveSystem.Load();
    }
    public void OnClickEnterOptions() 
    {
        SceneManager.LoadScene("Options",LoadSceneMode.Additive);
    } 
}
