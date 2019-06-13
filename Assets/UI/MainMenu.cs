
using System;
using System.IO;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

//using UnityEngine.Networking;

public class MainMenu : Menu
{
    public GameObject ConnectMenu;
    protected Vector3 connectPos;
    public GameObject errorDialogTemplate;

    public GameObject connectButtonObject;

    public InputField usernameInput;
    public InputField networkAddressInput;

    private void Start()
    {
        VerifyFolders();

        if (!errorReason.Equals("No Reason Given"))
        {
            CreateErrorDialog(errorReason);
            errorReason = "No Reason Given";
        }

        connectPos = -ConnectMenu.transform.localPosition;
        AllMenuStart();
        NewGame();
    }

    public void CreateErrorDialog()
    {
        CreateErrorDialog(errorReason);
    }

    public void CreateErrorDialog(string Reason)
    {
        GameObject newErrorDialog = GameObject.Instantiate(errorDialogTemplate);
        newErrorDialog.transform.SetParent(transform.parent, false);
        newErrorDialog.name = "ErrorDialog " + Reason;
        GameObject errorTextObject = newErrorDialog.transform.GetChild(0).gameObject;
        Text errorText = errorTextObject.GetComponent<Text>();
        errorText.text = "Error: " + Reason;
        GameObject dismissButtonObject = newErrorDialog.transform.GetChild(1).gameObject;
        lastButtonObject = EventSystem.current.currentSelectedGameObject;
        EventSystem.current.SetSelectedGameObject(dismissButtonObject);
    }

    // Game session management functions.
    public void ResumeGame()
    {
        managerScript.StartHost();
    }

    public void LoadGame()
    {
        managerScript.StartHost();
    }

    public void NewGame()
    {
        Directory.Delete(Application.persistentDataPath + "/saves/World", true);
        VerifyFolders();
        managerScript.StartHost();
    }

    public void Connect()
    {
        // Don't connect if the IP address is invalid. Instead, show an error. 
        string s = managerScript.networkAddress;
        char[] c = { '.' };
        IPAddress result = null;
        if (s.Length > 6 && (
                IPAddress.TryParse(s, out result) ||
                s.Split(c, 5).Length == 3 ||
                s.Equals("localhost")
            ))
        {
            managerScript.StartClient();
        }
        else
        {
            CreateErrorDialog("Invalid IP Address!");
        }
    }

    public void LoadNetworkInfo()
    {
        if (PlayerPrefs.HasKey("username"))
        {
            managerScript.username = PlayerPrefs.GetString("username");
            usernameInput.text = managerScript.username;
        }
        if (PlayerPrefs.HasKey("networkAddress"))
        {
            managerScript.networkAddress = PlayerPrefs.GetString("networkAddress");
            networkAddressInput.text = managerScript.networkAddress;
        }
        if (PlayerPrefs.HasKey("networkPort"))
        {
            managerScript.networkPort = PlayerPrefs.GetInt("networkPort");
        }
    }

    // Menu navigation functions.
    public void GoToConnect()
    {
        transform.localPosition = connectPos;
        EventSystem.current.SetSelectedGameObject(connectButtonObject);
    }

    // Check if these exist, if not, create them.
    private void VerifyFolders()
    {
        if (!Directory.Exists(Application.persistentDataPath))
        {
            Directory.CreateDirectory(Application.persistentDataPath);
        }
        if (!Directory.Exists(Application.persistentDataPath + "/saves"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/saves");
        }
        if (!Directory.Exists(Application.persistentDataPath + "/saves/World"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/saves/World");
        }
        if (!Directory.Exists(Application.persistentDataPath + "/saves/World/ships"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/saves/World/ships");
        }
    }

}
