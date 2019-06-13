
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class SendData : NetworkBehaviour
{
    [TargetRpc]
    public void TargetSendStuff(NetworkConnection connection)
    {
        print("I've just received a target RPC call!");
    }

    /*

				// Send the data for the station. 
				ObjMetadata stationMeta = new ObjMetadata (stationObject.transform.localPosition, Vector3.zero, 
					stationObject.transform.localRotation, station.circumference, station.targetGravity);
				byte[] stationData = Serialization.Dictionary2Byte (station.block);

				TargetSendStation (connection, stationMeta, stationData);

					GameObject shipObject = Ship;
					Ship ship = shipObject.GetComponent<Ship> ();
					Rigidbody shipRigidbody = shipObject.GetComponent<Rigidbody> ();

					ObjMetadata shipMeta = new ObjMetadata (shipObject.transform.localPosition, 
						shipRigidbody.velocity, shipObject.transform.localRotation, shipObject.name);
					byte[] shipData = Serialization.Dictionary3Byte (ship.block);

					TargetSendShip (connection, shipMeta, shipData);




	[TargetRpc]
	public void TargetSendStation (NetworkConnection connection, ObjMetadata meta, byte[] data) {
		print ("HOLY SHIT IVE JUST BEEN SENT A STATION!");
		GameObject stationObject = GameObject.Find ("Station");
		Station station = stationObject.GetComponent<Station>();

		stationObject.name = meta.name;
		stationObject.transform.localPosition = meta.position;
		stationObject.transform.localRotation = meta.rotation;
		station.circumference = meta.circumference;
		station.targetGravity = meta.targetGravity;

		station.block = Serialization.Byte2Dictionary (data);

		station.dataReady = true;
	}*/

}


















