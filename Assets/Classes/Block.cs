
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Block
{
	public byte type;
	public byte material;
	public byte rotation;
	//public float progress;
	//public float health;

	public static bool[] noFaces = { false, false, false, false, false, false };
	public bool[] selfFaces = { true, true, true, true, true, true };
	public bool[] otherFaces = { true, true, true, true, true, true };

	public Block ()
	{
		type = (byte)UnityEngine.Random.Range (0, 20);
		material = (byte)UnityEngine.Random.Range (0, 5);
		rotation = 0;

		if (material == 0) {
			otherFaces = noFaces;
		}
	}

	public Block (byte Material)
	{
		type = (byte)UnityEngine.Random.Range (0, 20);
		material = Material;
		rotation = 0;

		if (material == 0) {
			otherFaces = noFaces;
		}
	}

	public Block (byte Material, byte Type)
	{
		type = Type;
		material = Material;
		rotation = 0;

		if (material == 0) {
			otherFaces = noFaces;
		}
	}

	public Block (byte Material, byte Type, byte Faces)
	{
		type = Type;
		material = Material;
		rotation = 0;

		if (material == 0) {
			otherFaces = noFaces;
		}
	}

}
