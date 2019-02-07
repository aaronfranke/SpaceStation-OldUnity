
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class Commands : NetworkBehaviour
{

	public static Commands commands;

	// Do not attempt to serialize these. They are persistent temporary variables only.
	private GameObject shipObject;
	private Ship shipScript;

	void Awake ()
	{
		commands = this;
	}

	[Command]
	public void CmdSetBlock (Vector3 Position, byte Type, byte Material, string ObjectName)
	{
		print (Position + " " + Type + " " + Material + " " + ObjectName + " " + Station.stationObject.name);
		if (Station.stationObject.name.Contains (ObjectName)) {
			Point3 pos = new Point3 (Position);
			Station.station.RpcSetBlock (pos, Type, Material);
		} else {
			if (shipObject == null || !shipObject.name.Equals (ObjectName)) {
				shipObject = GameObject.Find (ObjectName);
				shipScript = shipObject.GetComponent<Ship> ();
			} 
			if (shipScript != null) {
				Vector3 position = shipObject.transform.InverseTransformPoint (Position);
				Point3 pos = new Point3 (position);
				shipScript.RpcSetBlock (pos, Type, Material);
			}
		}
	}

	public const ushort packetSize = 60000;

	[Command]
	public void CmdGetStation ()
	{
		byte[] data = Serialization.Dictionary2Byte (Station.station.block);

		int dataSize = data.Length / packetSize;

		ObjMetadata meta = new ObjMetadata (Station.stationObject.transform.localPosition, 
			                   Station.stationObject.transform.localRotation, Station.stationObject.name, dataSize + 1, Station.station.circumference, Station.station.targetGravity);
		
		Station.station.TargetSendStationMeta (connectionToClient, meta);

		// Unity has a limit of 64KB per packet, so we'll split it up into many smaller packets.
		for (int i = 0; i < dataSize; i++) {
			byte[] piece = new byte[packetSize];
			Array.Copy (data, i * packetSize, piece, 0, packetSize);
			Station.station.TargetSendStationPiece (connectionToClient, piece, i);
		}
		int finalLength = data.Length % packetSize;
		byte[] finalPiece = new byte[finalLength];
		Array.Copy (data, dataSize * packetSize, finalPiece, 0, finalLength);
		Station.station.TargetSendStationPiece (connectionToClient, finalPiece, dataSize);
	}

	[Command]
	public void CmdGetShip (string ShipName)
	{
		if (!shipObject || !shipObject.name.Equals (ShipName)) {
			shipObject = GameObject.Find (ShipName);
			shipScript = shipObject.GetComponent<Ship> ();
		} 
		// stub
	}

}
