
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using UnityEngine;

public static class Serialization {

	public static IFormatter formatter = new BinaryFormatter();

	// Converts to and from byte[] for sending over the network. 
	public static byte[] Dictionary2Byte (Dictionary<Point2, Block>[] block) {
		MemoryStream memoryStream = new MemoryStream();
		formatter.Serialize (memoryStream, block);
		byte[] byteArray = memoryStream.ToArray ();
		return byteArray;
	}

	public static byte[] Dictionary3Byte (Dictionary<Point3, Dictionary<Point3, Block>> block) {
		MemoryStream memoryStream = new MemoryStream();
		formatter.Serialize (memoryStream, block);
		byte[] byteArray = memoryStream.ToArray ();
		return byteArray;
	}

	public static Dictionary<Point3, Dictionary<Point3, Block>> Byte3Dictionary (byte[] byteArray) {
		MemoryStream memoryStream = new MemoryStream();
		memoryStream.Write (byteArray, 0, byteArray.Length);
		memoryStream.Position = 0;
		Dictionary<Point3, Dictionary<Point3, Block>> dictionary = formatter.Deserialize (memoryStream) as Dictionary<Point3, Dictionary<Point3, Block>>;
		return dictionary;
	}

	public static Dictionary<Point2, Block>[] Byte2Dictionary (byte[] byteArray) {
		MemoryStream memoryStream = new MemoryStream();
		memoryStream.Write (byteArray, 0, byteArray.Length);
		memoryStream.Position = 0;
		Dictionary<Point2, Block>[] dictionary = formatter.Deserialize (memoryStream) as Dictionary<Point2, Block>[];
		return dictionary;
	}
	// Converts to and from byte[] for sending over the network. 


	// Writes directly to a file, for saving to disk. 
	public static void SaveShip (string ShipName, Dictionary<Point3, Dictionary<Point3, Block>> Block) {
		string filename = Application.persistentDataPath + "/saves/" + World.worldName + "/ships/" + ShipName + ".dct";

		Stream stream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None);
		formatter.Serialize(stream, Block);
		stream.Close();
	}

	public static void SaveStation (Dictionary<Point2, Block>[] Block) {
		string filename = Application.persistentDataPath + "/saves/" + World.worldName + "/Station.dct";

		Stream stream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None);
		formatter.Serialize(stream, Block);
		stream.Close();
	}

	public static Dictionary<Point2, Block>[] LoadStation () {
		string filename = Application.persistentDataPath + "/saves/" + World.worldName + "/Station.dct";

		Stream stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
		Dictionary<Point2, Block>[] dictionary = formatter.Deserialize(stream) as Dictionary<Point2, Block>[];
		stream.Close();
		return dictionary;
	}

	public static Dictionary<Point3, Dictionary<Point3, Block>> LoadShip (string ShipName) {
		string filename = Application.persistentDataPath + "/saves/" + World.worldName + "/ships/" + ShipName + ".dct";

		Stream stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
		Dictionary<Point3, Dictionary<Point3, Block>> dictionary = formatter.Deserialize(stream) as Dictionary<Point3, Dictionary<Point3, Block>>;
		stream.Close();
		return dictionary;
	}
	// Writes directly to a file, for saving to disk. 



	/* 
	 * Just in case I want to send objects piece-by-piece rather than whole, I'll leave this code here.
	 * 
	public static byte[] Dictionary2Byte (Dictionary<Point2, Block> block) {
		MemoryStream memoryStream = new MemoryStream();
		formatter.Serialize (memoryStream, block);
		byte[] byteArray = memoryStream.ToArray ();
		return byteArray;
	}

	public static byte[] Dictionary3Byte (Dictionary<Point3, Block> block) {
		MemoryStream memoryStream = new MemoryStream();
		formatter.Serialize (memoryStream, block);
		byte[] byteArray = memoryStream.ToArray ();
		return byteArray;
	}

	public static Dictionary<Point3, Block> Byte3Dictionary (byte[] byteArray) {
		MemoryStream memoryStream = new MemoryStream();
		memoryStream.Write (byteArray, 0, byteArray.Length);
		memoryStream.Position = 0;
		Dictionary<Point3, Block> dictionary = formatter.Deserialize (memoryStream) as Dictionary<Point3, Block>;
		return dictionary;
	}

	public static Dictionary<Point2, Block> Byte2Dictionary (byte[] byteArray) {
		MemoryStream memoryStream = new MemoryStream();
		memoryStream.Write (byteArray, 0, byteArray.Length);
		memoryStream.Position = 0;
		Dictionary<Point2, Block> dictionary = formatter.Deserialize (memoryStream) as Dictionary<Point2, Block>;
		return dictionary;
	}*/

}