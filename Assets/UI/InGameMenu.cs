
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class InGameMenu : Menu
{
    public static Text healthText;
    public static Text energyText;
    public static Text speedText;
    public static Text referenceFrameText;
    public static Text jetpackText;
    public static Text velocityDampeningText;

    void Start()
    {
        AllMenuStart();

        healthText = GameObject.Find("HealthText").GetComponent<Text>();
        energyText = GameObject.Find("EnergyText").GetComponent<Text>();
        speedText = GameObject.Find("SpeedText").GetComponent<Text>();
        referenceFrameText = GameObject.Find("ReferenceFrameText").GetComponent<Text>();
        jetpackText = GameObject.Find("JetpackText").GetComponent<Text>();
        velocityDampeningText = GameObject.Find("VelocityDampeningText").GetComponent<Text>();
    }

    // Action Functions
    public void SaveMap()
    {
        Serialization.SaveStation(Station.station.block);
    }

    // Menu Navigation Functions
    public void CloseMenu()
    {
        PlayerCamera.paused = false;
    }

    // Game Session Management Functions
    public void QuitGame()
    {

        if (NetworkServer.active)
        {
            SaveMap();
            managerScript.StopHost();
        }
        else
        {
            managerScript.StopClient();
        }
    }

    public new void ExitGame()
    {
        QuitGame();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif

        Application.Quit();
    }

}
