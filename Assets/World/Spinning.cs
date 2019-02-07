using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class Spinning : NetworkBehaviour
{
	public static readonly int gyroStrength = 100000;

	protected GameObject world;
	protected GameObject playerBody;
	protected GameObject playerRoot;
	protected Station station;
	protected LocalPlayer localPlayer;

	[SyncVar] protected Vector3 RotationVector = new Vector3 (0.0f, 6.0f, 0.0f);
	[SyncVar] protected Vector3 RotationTarget = new Vector3 (0.0f, 6.0f, 0.0f);

	void Start ()
	{
		station = GetComponent<Station> ();
		station.radius = (short)Mathf.RoundToInt (station.circumference / (2 * Mathf.PI));
		RotationVector = CalculateSpin (station.currentGravity, station.radius);
	}

	void Update ()
	{
		AdjustRotationSpeed ();
		transform.Rotate (RotationVector * Time.deltaTime);
	}

	void AdjustRotationSpeed ()
	{
		if (station.targetGravity < 0) {
			station.targetGravity = Mathf.Abs (station.targetGravity);
		}
		RotationTarget = CalculateSpin (station.targetGravity, station.radius);
		station.currentGravity = CalculateGravity (RotationVector, station.radius);
		station.currentGravityScaler = CalculateGravity (RotationVector);

		float changer = Time.deltaTime * station.gyroCount * gyroStrength / station.mass;
		if (RotationVector.y + changer < RotationTarget.y) {
			RotationVector.y += changer;
		} else if (RotationVector.y - changer > RotationTarget.y) {
			RotationVector.y -= changer;
		} else {
			RotationVector.y = RotationTarget.y;
		}
	}

	public static Vector3 CalculateSpin (float grav, int rad)
	{
		float radiansPerSecond = Mathf.Sqrt (grav / rad);
		float degreesPerSecond = radiansPerSecond * 57.2958f;
		Vector3 rotation = new Vector3 (0, degreesPerSecond, 0);
		return rotation;
	}

	public static float CalculateGravity (Vector3 rotation, int dist)
	{
		float radiansPerSecond = rotation.y / 57.2958f;
		float gravity = radiansPerSecond * radiansPerSecond * dist;
		return gravity;
	}

	public static float CalculateGravity (Vector3 rotation)
	{
		float radiansPerSecond = rotation.y / 57.2958f;
		float gravity = radiansPerSecond * radiansPerSecond;
		return gravity;
	}
}
