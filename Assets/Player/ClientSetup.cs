
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ClientSetup : NetworkBehaviour
{

	[SerializeField] private GameObject playerRoot = null;
	[SerializeField] private GameObject playerCameraHolder = null;

	[SerializeField] private Player playerScript = null;
	[SerializeField] private LocalPlayer localPlayerScript = null;
	[SerializeField] private PlayerCamera playerCameraScript = null;
	[SerializeField] private Commands commandsScript = null;
	[SerializeField] private SyncPosition syncPosition = null;

	[SerializeField] private Camera playerCamera = null;
	[SerializeField] private FlareLayer playerFlare = null;
	[SerializeField] private AudioListener playerAudio = null;

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
