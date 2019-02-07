
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class Player : MonoBehaviour
{

	public bool onStation = true;
	public bool jetpackEnabled = false;
	public bool velocityDampening = true;
	protected bool onGround = false;

	[SerializeField] protected GameObject playerBody;
	[SerializeField] protected Rigidbody physicsRoot;
	[SerializeField] protected SyncPosition syncPosition;

	public float health = 100;
	public float energy = 100;
	protected byte movementSpeed = 4;
	// Changing this does nothing, change in the method instead.
	protected const float airResistanceMultiplier = 0.999f;
	// Constant, can be changed here.
	protected const float jumpStrength = 6.0f;
	// Constant, can be changed here.
	protected const float jetpackSpeed = 200.0f;
	// Constant, can be changed here.

	void Start ()
	{
		transform.parent = Station.stationObject.transform;
	}

	void Update ()
	{
		PlayerTasks ();
	}

	protected void PlayerTasks ()
	{
		SetParent ();

		if (onStation) {
			ApplyGravity ();
			if (!jetpackEnabled) {
				RotateToStation ();
			}
		}

	}

	protected void SetParent ()
	{
		Vector2 playerPos2D = new Vector2 (transform.position.x, transform.position.z);
		Vector2 stationPos2D = new Vector2 (Station.stationObject.transform.position.x, Station.stationObject.transform.position.z);

		if (Vector2.Distance (playerPos2D, stationPos2D) > 2 * Station.station.radius) {
			onStation = false;
		} else if (!jetpackEnabled) {
			onStation = true;
		} 
		if (onStation) {
			transform.SetParent (Station.stationObject.transform);
		} else {
			transform.SetParent (World.worldObject.transform);
		}
	}

	protected void RotateToStation ()
	{
		Quaternion look = Quaternion.LookRotation (new Vector3 (transform.localPosition.x, 0, transform.localPosition.z));
		Vector3 euler = new Vector3 (-90, look.eulerAngles.y, 0);
		transform.localRotation = Quaternion.Euler (euler);
		playerBody.transform.localRotation = Quaternion.Euler (new Vector3 (0, playerBody.transform.localRotation.eulerAngles.y, 0));
	}

	protected void ApplyGravity ()
	{
		Vector3 forceToApply = new Vector3 (transform.localPosition.x, 0, transform.localPosition.z) * Station.station.currentGravityScaler;
		forceToApply = Station.station.transform.TransformDirection (forceToApply);
		physicsRoot.AddForce (forceToApply * (60 * Time.deltaTime), ForceMode.Acceleration);
	}
		
}
