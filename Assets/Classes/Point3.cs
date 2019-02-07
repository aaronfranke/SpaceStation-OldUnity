
// Point3.cs - Represent 3D positions or points with 16-bit shorts. By Aaron Franke. 
// Inspired by Unity's built-in Vector3 struct. I've tried to mimic it somewhat.
// Supports creating a Point3 via numbers or with an existing Vector3.
// Supports adding together Point3 and Vector3, with each other and together.
// Includes readonly pre-built Point3 objects for directions and multipliers.

using UnityEngine;
using System;

[Serializable]
public struct Point3
{
	// Variables.
	public short x, y, z;

	// Constructors.
	public Point3 (int X, int Y, int Z)
	{
		x = (short)X;
		y = (short)Y;
		z = (short)Z;
	}

	public Point3 (Point3 Point)
	{
		x = Point.x;
		y = Point.y;
		z = Point.z;
	}

	public Point3 (Vector3 V)
	{
		x = (short)Mathf.RoundToInt (V.x);
		y = (short)Mathf.RoundToInt (V.y);
		z = (short)Mathf.RoundToInt (V.z);
	}

	// Constants.
	public static readonly Point3 zero = new Point3 (0, 0, 0);
	public static readonly Point3 one = new Point3 (1, 1, 1);
	public static readonly Point3 negOne = new Point3 (-1, -1, -1);

	public static readonly Point3 up = new Point3 (0, 1, 0);
	public static readonly Point3 down = new Point3 (0, -1, 0);
	public static readonly Point3 right = new Point3 (1, 0, 0);
	public static readonly Point3 left = new Point3 (-1, 0, 0);
	public static readonly Point3 forward = new Point3 (0, 0, 1);
	public static readonly Point3 back = new Point3 (0, 0, -1);

	// Output methods.
	public Vector3 ToVector ()
	{
		Vector3 newVector = new Vector3 (x, y, z);
		return newVector;
	}

	public static Vector3 ToVector (Point3 Point)
	{
		Vector3 newVector = new Vector3 (Point.x, Point.y, Point.z);
		return newVector;
	}

	override
	public String ToString ()
	{
		return "(" + x + ", " + y + ", " + z + ")";
	}

	public static String ToString (Point3 Point)
	{
		return "(" + Point.x + ", " + Point.y + ", " + Point.z + ")";
	}

	public bool Equals (Point3 Other)
	{
		return (Other.x == x && Other.y == y && Other.z == z);
	}

	// Math methods.
	public void MultiplySelf (int Scaler)
	{
		short scaler = (short)Scaler;
		x *= scaler;
		y *= scaler;
		z *= scaler;
	}

	public Point3 Multiply (int Scaler)
	{
		short scaler = (short)Scaler;
		Point3 newPoint = new Point3 (x * scaler, y * scaler, z * scaler);
		return newPoint;
	}

	public static Point3 Multiply (Point3 Point, int Scaler)
	{
		Point3 newPoint = Point.Multiply (Scaler);
		return newPoint;
	}

	public void Add (Point3 Other)
	{
		x += Other.x;
		y += Other.y;
		z += Other.z;
	}

	public void Add (Vector3 OtherVector)
	{
		x += (short)Mathf.RoundToInt (OtherVector.x);
		y += (short)Mathf.RoundToInt (OtherVector.y);
		z += (short)Mathf.RoundToInt (OtherVector.z);
	}

	public static Point3 Add (Point3 A, Point3 B)
	{
		Point3 newPoint = new Point3 (A.x + B.x, A.y + B.y, A.z + B.z);
		return newPoint;
	}

	public static Point3 Add (Point3 A, Vector3 B)
	{
		Point3 newPoint = new Point3 (A.x + (short)Mathf.RoundToInt (B.x), A.y + (short)Mathf.RoundToInt (B.y), A.z + (short)Mathf.RoundToInt (B.z));
		return newPoint;
	}

	public static Point3 Add (Vector3 A, Point3 B)
	{
		Point3 newPoint = new Point3 ((short)Mathf.RoundToInt (A.x) + B.x, (short)Mathf.RoundToInt (A.y) + B.y, (short)Mathf.RoundToInt (A.z) + B.z);
		return newPoint;
	}

	public static Point3 Add (Vector3 A, Vector3 B)
	{
		Point3 newPoint = new Point3 ((short)Mathf.RoundToInt (A.x + B.x), (short)Mathf.RoundToInt (A.y + B.y), (short)Mathf.RoundToInt (A.z + B.z));
		return newPoint;
	}

}




