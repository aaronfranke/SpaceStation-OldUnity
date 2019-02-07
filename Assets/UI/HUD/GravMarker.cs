using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class GravMarker : MonoBehaviour
{
	public static GameObject playerRoot;
	[SerializeField] private GameObject gravTextObject;
	[SerializeField] private Text gravText;

	void Update ()
	{
		if (Station.station != null && playerRoot != null && playerRoot.transform.parent.name.StartsWith ("Station")) {
			
			Vector2 flatPos = new Vector2 (playerRoot.transform.localPosition.x, playerRoot.transform.localPosition.z);

			float gravity = Station.station.currentGravityScaler * flatPos.magnitude;

			SetMarkerPos (gravity);

			string gravityString = "";
			if (gravity > 10) {
				gravityString = Mathf.RoundToInt (gravity).ToString ();
			} else {
				gravityString = gravity.ToString ("F1");
			}

			gravText.text = "Gravity: " + gravityString + " m/s²";
		} else {
			gravText.text = "No Gravity Here!";
			transform.localPosition = new Vector3 (-300, transform.localPosition.y, 0);
		}
	}

	private void SetMarkerPos (float gravity)
	{
		float position = 300;

		if (gravity < 4) {
			position = (200 * (gravity / 4)) - 300;
		} else if (gravity < 64) {
			position = (Mathf.Log (gravity, 2) * 100) - 300;
		} 

		transform.localPosition = new Vector3 (position, transform.localPosition.y, 0);
	}
}
