
using System;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

// Stores information about the world.

[Serializable]
public class ServerSetup : NetworkBehaviour
{
    public GameObject worldPrefab;
    public GameObject stationPrefab;
    public GameObject shipPrefab;

    void Awake()
    {
        if (!NetworkServer.active)
        {
            return;
        }

        GameObject worldObject = Instantiate(worldPrefab) as GameObject;
        NetworkServer.Spawn(worldObject);
        GameObject stationObject = Instantiate(stationPrefab) as GameObject;
        NetworkServer.Spawn(stationObject);

        GameObject shipObject = Instantiate(shipPrefab) as GameObject;
        NetworkServer.Spawn(shipObject);
    }

}
