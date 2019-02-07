using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class RenderQuality : MonoBehaviour
{
	private Dropdown dropdown;
	private byte renderQuality;

	void Start ()
	{
		dropdown = GetComponent<Dropdown> ();

		if (!PlayerPrefs.HasKey ("RenderQuality")) {
			PlayerPrefs.SetInt ("RenderQuality", 4);
		}
		renderQuality = (byte)PlayerPrefs.GetInt ("RenderQuality");
		dropdown.value = 5 - renderQuality;
	}

	public void SetRenderQuality (int unusedValue)
	{ 
		renderQuality = (byte)(5 - dropdown.value);
		QualitySettings.SetQualityLevel (renderQuality, gameObject.name.Equals ("RenderQualityDropdownMainMenu"));
		PlayerPrefs.SetInt ("RenderQuality", renderQuality);
		PlayerPrefs.Save ();
	}

}
