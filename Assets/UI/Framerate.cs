using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Framerate : MonoBehaviour
{
	public Text text;
	// Use this for initialization
	void Start ()
	{
		text = GetComponent<Text> ();
		InvokeRepeating ("UpdateText", 0.0f, 1.0f);
	}
	
	// Update is called once per frame
	void UpdateText ()
	{
		short framerateToDisplay = (short)(Mathf.RoundToInt (1 / Time.deltaTime));
		text.text = ("Framerate: " + framerateToDisplay + " FPS");
	}
}
