
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public abstract class VoxelObject : NetworkBehaviour
{
	// This class is extended by Station and Ship. Only aspects that they share go here.

	// Constants.
	[SerializeField] public GameObject meshHolderTemplate;
	[SerializeField] public GameObject BlockHolder;

	// Variables.
	[SyncVar] public int mass = 0;
	[SyncVar] public int gyroCount = 5;
	protected bool firstRenderDone = false;
	protected bool dataReady = false;

	protected Vector2[] GetUVArray (byte materialByte)
	{
		float material = materialByte;
		float materialX = (material % 16) / 16;
		float materialY = (material - (material % 16)) / 256;
		Vector2[] uvArray = new Vector2[] { 
			new Vector2 (materialX + 0.0005f, materialY + 0.0005f), 
			new Vector2 (materialX + 0.0005f, materialY + 0.0620f), 
			new Vector2 (materialX + 0.0620f, materialY + 0.0005f), 
			new Vector2 (materialX + 0.0620f, materialY + 0.0620f) 
		};
		return uvArray;
	}

}
