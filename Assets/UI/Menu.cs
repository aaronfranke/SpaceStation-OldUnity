
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Menu : MonoBehaviour
{
    public GameObject MainMenu;
    public GameObject OptionsMenu;
    protected Vector3 mainPos, optionsPos;

    public static GameObject lastButtonObject;
    public GameObject defaultButtonObject;
    public GameObject optionsButtonObject;

    public static string errorReason = "No Reason Given";
    protected GameObject managerObject;
    protected NetworkManagerScript managerScript;

    public void AllMenuStart()
    {
        managerObject = GameObject.Find("NetworkManager");
        if (managerObject == null)
        {
            SceneManager.LoadScene(0); // If the world scene starts directly without the network manager, load the main menu scene. 
        }
        else
        {
            managerScript = managerObject.GetComponent<NetworkManagerScript>();
        }

        mainPos = Vector3.zero;
        optionsPos = -OptionsMenu.transform.localPosition;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            GoToMain();
        }
        if (Input.GetKeyDown(KeyCode.F11))
        {
            Screen.fullScreen = !Screen.fullScreen;
        }
    }

    // Action Functions
    public void ResetHue()
    {
        ButtonColor.ResetHue();
    }

    // Menu Navigation Functions
    public void GoToMain()
    {
        transform.localPosition = mainPos;
        EventSystem.current.SetSelectedGameObject(defaultButtonObject);
    }

    public void GoToOptions()
    {
        transform.localPosition = optionsPos;
        EventSystem.current.SetSelectedGameObject(optionsButtonObject);
    }

    // Game Session Management Functions
    public void ExitGame()
    {

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif

        Application.Quit();
    }

}
