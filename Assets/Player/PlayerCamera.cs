
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{

	// As this one isn't contained in the prefab, it doesn't get a SerializeField.
	private GameObject uiMenu;

	[SerializeField] private GameObject playerBody;
	[SerializeField] private GameObject playerRoot;
	[SerializeField] private GameObject cameraHolder;
	[SerializeField] private LocalPlayer localPlayer;
	[SerializeField] private Commands commands;
	[SerializeField] private MeshRenderer playerModelMeshRenderer;

	private RaycastHit hit;
	private Mesh mesh;

	private Vector3 backAngle = new Vector3 (0, 180, 0);
	private Vector3 placeBlock;
	private Vector3 removeBlock;
	private string objectName;

	public float sensitivity = 400;
	public float keySensitivity = 150;

	public static bool paused = false;
	private bool thirdPerson = false;
	private bool secondPerson = false;

	private float xRot = 0;

	void Start ()
	{
		uiMenu = GameObject.Find ("uiMenu");
	}

	void Update ()
	{

		mesh = null;

		ThirdPerson ();

		PauseMenu ();

		if (GetBlockInView ()) {
			if (DebugUI.debugmenu) {
				DrawTestTriangle ();
			}
			if (Input.GetMouseButtonDown (0)) {
				commands.CmdSetBlock (removeBlock, 0, 0, objectName);
			}
			if (Input.GetMouseButtonDown (1)) {
				commands.CmdSetBlock (placeBlock, 1, 1, objectName);
			}
		}

	}

	private void ThirdPerson ()
	{
		// Key toggles. 
		if (Input.GetKeyDown (KeyCode.F5) || Input.GetKeyDown (KeyCode.V)) {
			thirdPerson = !thirdPerson; 
			secondPerson = false;
		}
		if (Input.GetKeyDown (KeyCode.F6)) {
			secondPerson = !secondPerson; 
			thirdPerson = false;
		}

		if (thirdPerson) { // Moves camera backwards for 3rd person view. 
			float cameraDistance = -5.0f;
			RaycastHit cameraPosFinder;
			Physics.Raycast (transform.localPosition, -transform.forward, out cameraPosFinder);
			if (cameraPosFinder.distance > 0.1 && cameraPosFinder.distance < 1 - cameraDistance) {
				cameraDistance = 1 - cameraPosFinder.distance; 
			}
			cameraHolder.transform.localPosition = new Vector3 (0, 0, cameraDistance);
			cameraHolder.transform.localEulerAngles = Vector3.zero;
			playerModelMeshRenderer.enabled = true;

		} else if (secondPerson) { // Moves camera forwards for 2nd person view. 
			float cameraDistance = 5.0f;
			RaycastHit cameraPosFinder;
			Physics.Raycast (transform.localPosition, transform.forward, out cameraPosFinder);
			if (cameraPosFinder.distance > 0.1 && cameraPosFinder.distance < 1 - cameraDistance) {
				cameraDistance = 1 - cameraPosFinder.distance; 
			}
			cameraHolder.transform.localPosition = new Vector3 (0, 0, cameraDistance);
			cameraHolder.transform.localEulerAngles = backAngle;
			playerModelMeshRenderer.enabled = true;

		} else { // Standard camera position, you know, on the player's head. 
			cameraHolder.transform.localPosition = Vector3.zero;
			cameraHolder.transform.localEulerAngles = Vector3.zero;
			playerModelMeshRenderer.enabled = false;
		}
	}

	private void VerticalLook ()
	{
		if (!localPlayer.jetpackEnabled || Input.GetKey (KeyCode.LeftAlt)) { // Vertical look axis, rotates the camera only. 
			xRot -= Input.GetAxis ("Mouse Y") * Time.deltaTime * sensitivity; 
			if (Input.GetKey (KeyCode.UpArrow) == true) { // Vertical arrow key look movement. 
				xRot -= Time.deltaTime * keySensitivity * 0.75f;
			}
			if (Input.GetKey (KeyCode.DownArrow) == true) {
				xRot += Time.deltaTime * keySensitivity * 0.75f;
			}
			// Prevents the end result rotation from going farther than fully up or down. Removing this causes seizures. 
			xRot = Mathf.Clamp (xRot, -80, 80); 
			if (thirdPerson) {
				xRot = Mathf.Clamp (xRot, -20, 50); 
			} else if (secondPerson) {
				xRot = Mathf.Clamp (xRot, -50, 20); 
			}
			transform.localRotation = Quaternion.Euler (xRot, transform.localRotation.eulerAngles.y, 0);

		} else { // Vertical look axis, rotates entire player model. 
			playerBody.transform.Rotate (-Input.GetAxis ("Mouse Y") * Time.deltaTime * sensitivity * 0.75f, 0, 0);
			if (Input.GetKey (KeyCode.UpArrow)) { // Vertical arrow key look movement. 
				playerBody.transform.Rotate (Time.deltaTime * -keySensitivity * 0.75f, 0, 0);
			}
			if (Input.GetKey (KeyCode.DownArrow)) {
				playerBody.transform.Rotate (Time.deltaTime * keySensitivity * 0.75f, 0, 0);
			}
			xRot = xRot * 0.98f;
		}
	}

	private void HorizontalLook ()
	{
		if (Input.GetKey (KeyCode.LeftAlt)) { // Horizontal look axis, rotates the camera only. 
			transform.Rotate (0, Input.GetAxis ("Mouse X") * Time.deltaTime * sensitivity, 0);
			transform.localRotation = Quaternion.Euler (new Vector3 (transform.localRotation.eulerAngles.x, transform.localRotation.eulerAngles.y, 0));
		} else { // Horizontal look axis, rotates entire player model. 
			playerBody.transform.Rotate (0, Input.GetAxis ("Mouse X") * Time.deltaTime * sensitivity, 0);
			if (Input.GetKey (KeyCode.LeftArrow) == true) {
				playerBody.transform.Rotate (new Vector3 (0, Time.deltaTime * -keySensitivity, 0)); // Horizontal arrow key look movement. 
			}
			if (Input.GetKey (KeyCode.RightArrow) == true) {
				playerBody.transform.Rotate (new Vector3 (0, Time.deltaTime * keySensitivity, 0));
			}
		}
	}

	private void ViewCentering ()
	{
		float yRot = transform.localRotation.eulerAngles.y;
		if (transform.localRotation.eulerAngles.y > 180) {
			yRot = yRot - 360;
		}
		transform.localRotation = Quaternion.Euler (xRot, yRot * 0.98f, 0);
	}

	private void PauseMenu ()
	{
		if (Input.GetKeyDown (KeyCode.Escape)) {
			paused = !paused;
		} // Causes Esc to act as a toggle for the game menu. 

		if (!paused && Time.time > 0.1f) {
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
			uiMenu.transform.localPosition = new Vector3 (100000, 0, 0);

			VerticalLook ();
			HorizontalLook ();

			if (!Input.GetKey (KeyCode.LeftAlt)) {
				ViewCentering ();
			}

		} else {
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
			uiMenu.transform.localPosition = new Vector3 (0, uiMenu.transform.localPosition.y, 0);
		}
	}

	private bool MouseOverButton ()
	{
		List<RaycastResult> menuHit = new List<RaycastResult> ();
		PointerEventData pointerEventData = new PointerEventData (EventSystem.current);
		pointerEventData.position = Input.mousePosition;
		EventSystem.current.RaycastAll (pointerEventData, menuHit);
		return menuHit.Count > 0;
	}

	private bool GetBlockInView ()
	{
		if (!Physics.Raycast (Camera.main.ScreenPointToRay (new Vector3 (Screen.width / 2, Screen.height / 2, 0)), out hit)) {
			return false;
		}

		if (hit.distance > 8) {
			return false;
		}

		if (paused) {
			if (MouseOverButton ()) {
				return false;
			}
		}
		//print (hit.point + " " + hit.transform);

		MeshCollider meshCollider = hit.collider as MeshCollider;
		if (meshCollider == null || meshCollider.sharedMesh == null) {
			//return false;
		} else {
			mesh = meshCollider.sharedMesh;
		}

		Vector3 internalPos = hit.point - (hit.normal / 2);
		Vector3 externalPos = hit.point + (hit.normal / 2);

		// Safe method for checking whether or not the raycast has hit a block of the main Station grid, and if so, set the appropriate variables. 
		if (hit.collider.transform.gameObject.name.Contains ("MeshHolder") && hit.collider.transform.parent && hit.collider.transform.parent.parent && hit.collider.transform.parent.parent.name.Contains ("Station")) {
			objectName = "Station";

			internalPos = hit.collider.transform.parent.parent.transform.InverseTransformPoint (internalPos);
			externalPos = hit.collider.transform.parent.parent.transform.InverseTransformPoint (externalPos);

			int circumferencePos = hit.collider.transform.gameObject.GetComponent<SetTextures> ().circumferencePos;

			placeBlock = new Vector3 (
				circumferencePos + GetCircumferenceDirection (hit.point, hit.normal),
				Mathf.Round (externalPos.y),
				Mathf.Round (new Vector2 (externalPos.x, externalPos.z).magnitude)
			);
			removeBlock = new Vector3 (
				circumferencePos,
				Mathf.Round (internalPos.y),
				Mathf.Round (new Vector2 (internalPos.x, internalPos.z).magnitude)
			);

		} else {
			objectName = hit.transform.name;
			placeBlock = externalPos;
			removeBlock = internalPos;
		}

		return true;
	}

	private sbyte GetCircumferenceDirection (Vector3 pos, Vector3 dir)
	{
		float angleHit = Mathf.Atan2 (pos.z - Station.stationObject.transform.localPosition.z, pos.x - Station.stationObject.transform.localPosition.x);
		float angleNormal = Mathf.Atan2 (pos.z + dir.z - Station.stationObject.transform.localPosition.z, pos.x + dir.x - Station.stationObject.transform.localPosition.x);
		float angleDiff = angleNormal - angleHit;
		if (angleDiff > 1) {
			return -1;
		}
		if (angleDiff < -1) {
			return 1;
		}
		if (angleDiff > (1.0f / Station.station.circumference)) {
			return 1;
		}
		if (angleDiff < (-1.0f / Station.station.circumference)) {
			return -1;
		}
		return 0;
	}

	private bool DrawTestTriangle ()
	{
		if (mesh) {
			Vector3[] vertices = mesh.vertices;
			int[] triangles = mesh.triangles;
			Vector3 p0 = vertices [triangles [hit.triangleIndex * 3 + 0]];
			Vector3 p1 = vertices [triangles [hit.triangleIndex * 3 + 1]];
			Vector3 p2 = vertices [triangles [hit.triangleIndex * 3 + 2]];
			Transform hitTransform = hit.collider.transform;
			p0 = hitTransform.TransformPoint (p0);
			p1 = hitTransform.TransformPoint (p1);
			p2 = hitTransform.TransformPoint (p2);

			Debug.DrawLine (p0, p1);
			Debug.DrawLine (p1, p2);
			Debug.DrawLine (p2, p0);
		}
		Debug.DrawRay (hit.point, -hit.normal / 2);
		Debug.DrawRay (hit.point, hit.normal / 2);

		return true;
	}




}
