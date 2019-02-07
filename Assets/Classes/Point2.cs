
// Point2.cs - Represent 2D positions or points with 16-bit shorts. By Aaron Franke. 
// Inspired by Unity's built-in Vector2 struct. I've tried to mimic it somewhat.
// Supports creating a Point2 via numbers or with an existing Vector2.
// Supports adding together Point2 and Vector2, with each other and together.
// Includes readonly pre-built Point2 objects for directions and multipliers.

using UnityEngine;
using System;

[Serializable]
public struct Point2
{
	// Variables.
	public short x, y;

	// Constructors.
	public Point2 (int X, int Y)
	{
		x = (short)X;
		y = (short)Y;
	}

	public Point2 (Point2 Point)
	{
		x = Point.x;
		y = Point.y;
	}

	public Point2 (Vector2 V)
	{
		x = (short)Mathf.RoundToInt (V.x);
		y = (short)Mathf.RoundToInt (V.y);
	}

	public Point2 (int Scale)
	{
		this = one.Multiply (Scale);
	}

	// Constants.
	public static readonly Point2 zero = new Point2 (0, 0);
	public static readonly Point2 one = new Point2 (1, 1);
	public static readonly Point2 negOne = new Point2 (-1, -1);

	public static readonly Point2 up = new Point2 (0, 1);
	public static readonly Point2 down = new Point2 (0, -1);
	public static readonly Point2 right = new Point2 (1, 0);
	public static readonly Point2 left = new Point2 (-1, 0);

	// Output methods.
	public Vector2 ToVector ()
	{
		Vector2 newVector = new Vector2 (x, y);
		return newVector;
	}

	public static Vector2 ToVector (Point2 Point)
	{
		Vector2 newVector = new Vector2 (Point.x, Point.y);
		return newVector;
	}

	override
	public String ToString ()
	{
		return "(" + x + ", " + y + ")"; 
	}

	public static String ToString (Point2 Point)
	{
		return "(" + Point.x + ", " + Point.y + ")"; 
	}

	public bool Equals (Point2 Other)
	{
		return (Other.x == x && Other.y == y);
	}

	// Math methods.
	public void MultiplySelf (int Scaler)
	{
		short scaler = (short)Scaler;
		x *= scaler;
		y *= scaler;
	}

	public Point2 Multiply (int Scaler)
	{
		short scaler = (short)Scaler;
		Point2 newPoint = new Point2 (x * scaler, y * scaler);
		return newPoint;
	}

	public static Point2 Multiply (Point2 Point, int Scaler)
	{
		Point2 newPoint = Point.Multiply (Scaler);
		return newPoint;
	}

	public void Add (Point2 Other)
	{
		x += Other.x;
		y += Other.y;
	}

	public void Add (Vector2 OtherVector)
	{
		x += (short)Mathf.RoundToInt (OtherVector.x);
		y += (short)Mathf.RoundToInt (OtherVector.y);
	}

	public static Point2 Add (Point2 A, Point2 B)
	{
		Point2 newPoint = new Point2 (A.x + B.x, A.y + B.y);
		return newPoint;
	}

	public static Point2 Add (Point2 A, Vector2 B)
	{
		Point2 newPoint = new Point2 (A.x + (short)Mathf.RoundToInt (B.x), A.y + (short)Mathf.RoundToInt (B.y));
		return newPoint;
	}

	public static Point2 Add (Vector2 A, Point2 B)
	{
		Point2 newPoint = new Point2 ((short)Mathf.RoundToInt (A.x) + B.x, (short)Mathf.RoundToInt (A.y) + B.y);
		return newPoint;
	}

	public static Point2 Add (Vector2 A, Vector2 B)
	{
		Point2 newPoint = new Point2 ((short)Mathf.RoundToInt (A.x + B.x), (short)Mathf.RoundToInt (A.y + B.y));
		return newPoint;
	}

}




