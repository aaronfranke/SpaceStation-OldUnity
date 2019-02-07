using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetTextures : MonoBehaviour
{
	private MeshRenderer meshRenderer;

	private Dropdown dropdown;

	public Material[] materialArray;

	public static byte quality = 1;

	void Start ()
	{
		if (!PlayerPrefs.HasKey ("TextureQuality")) {
			PlayerPrefs.SetInt ("TextureQuality", 1);
		}
		quality = (byte)PlayerPrefs.GetInt ("TextureQuality"); 

		if (gameObject.name.Equals ("MeshHolderTemplate")) {
			meshRenderer = GetComponent<MeshRenderer> ();
			meshRenderer.material = materialArray [quality];
		} else if (gameObject.name.Equals ("TextureQualityDropdown")) {
			dropdown = GetComponent<Dropdown> ();
			dropdown.value = quality;
		}
	}

	public void SetTextureQuality (int unusedValue)
	{
		quality = (byte)(dropdown.value);
		PlayerPrefs.SetInt ("TextureQuality", quality);
		PlayerPrefs.Save ();
	}

	// For use when a child of MeshHolder.
	public int circumferencePos;
}
