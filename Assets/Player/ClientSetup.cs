
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ClientSetup : NetworkBehaviour
{

	[SerializeField] private GameObject playerRoot;
	[SerializeField] private GameObject playerBody;
	[SerializeField] private GameObject playerHead;
	[SerializeField] private GameObject playerCameraHolder;
	[SerializeField] private GameObject playerCameraObject;

	[SerializeField] private Player playerScript;
	[SerializeField] private LocalPlayer localPlayerScript;
	[SerializeField] private PlayerCamera playerCameraScript;
	[SerializeField] private Commands commandsScript;
	[SerializeField] private SyncPosition syncPosition;

	[SerializeField] private Camera playerCamera;
	[SerializeField] private FlareLayer playerFlare;
	[SerializeField] private AudioListener playerAudio;

	void Start ()
	{
		if (isLocalPlayer) {
			playerRoot.name = "LocalPlayer";
			GravMarker.playerRoot = gameObject;
			localPlayerScript.enabled = true;
			playerCameraScript.enabled = true;
			commandsScript.enabled = true;
			syncPosition.enabled = true;

			playerCamera.enabled = true;
			playerFlare.enabled = true;
			playerAudio.enabled = true;

		} else {
			playerScript.enabled = true;
			Destroy (playerCameraHolder);
		}
	}
}
