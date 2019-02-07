
using System;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

// Stores information about the world.

[Serializable]
public class World : MonoBehaviour
{
	public static string worldName = "World";

	public static GameObject worldObject;

	void Awake ()
	{
		worldObject = gameObject;
	}

}




