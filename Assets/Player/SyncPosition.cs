
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class SyncPosition : NetworkBehaviour
{

	[SerializeField] private GameObject playerBody = null;
	[SerializeField] private Rigidbody physicsRoot = null;
	[SerializeField] private Player player = null;

	void Update ()
	{
		CmdSyncPos (transform.localPosition, transform.localRotation, playerBody.transform.localRotation, physicsRoot.velocity, transform.parent.name); 
	}

	// Send stats to the server and run the RPC for everyone, including the server.
	[Command]
	public void CmdSyncStats (bool Dampening, bool Jetpack, bool OnStation)
	{
		RpcSyncStats (Dampening, Jetpack, OnStation);
	}

	// For each player, transfer the stats from the server to the client, and set them.
	[ClientRpc]
	void RpcSyncStats (bool Dampening, bool Jetpack, bool OnStation)
	{
		if (playerBody == null) {
			return;
		}
		player.velocityDampening = Dampening;
		player.jetpackEnabled = Jetpack;
		player.onStation = OnStation;
	}

	// Send position to the server and run the RPC for everyone, including the server.
	[Command]
	protected void CmdSyncPos (Vector3 localPosition, Quaternion localRotation, Quaternion bodyRotation, Vector3 velocity, string parentName)
	{
		RpcSyncPos (localPosition, localRotation, bodyRotation, velocity, parentName);
	}

	// For each player, transfer the position from the server to the client, and set it as long as it's not the local player.
	[ClientRpc]
	void RpcSyncPos (Vector3 localPosition, Quaternion localRotation, Quaternion bodyRotation, Vector3 velocity, string parentName)
	{
		if (playerBody == null) {
			return;
		}
		if (!isLocalPlayer) {
			transform.localPosition = localPosition;
			transform.localRotation = localRotation;
			playerBody.transform.localRotation = bodyRotation;
			physicsRoot.velocity = velocity;

			if (!transform.parent || !transform.parent.name.Equals (parentName)) {
				print ("parent: " + parentName);
				transform.parent = GameObject.Find (parentName).transform;
			}
		}
	}
}
















