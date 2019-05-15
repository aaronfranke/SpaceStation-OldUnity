
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
	public static byte[] Dictionary2Byte (Dictionary<Vector2i, Block>[] block) {
		MemoryStream memoryStream = new MemoryStream();
		formatter.Serialize (memoryStream, block);
		byte[] byteArray = memoryStream.ToArray ();
		return byteArray;
	}

	public static byte[] Dictionary3Byte (Dictionary<Vector3i, Dictionary<Vector3i, Block>> block) {
		MemoryStream memoryStream = new MemoryStream();
		formatter.Serialize (memoryStream, block);
		byte[] byteArray = memoryStream.ToArray ();
		return byteArray;
	}

	public static Dictionary<Vector3i, Dictionary<Vector3i, Block>> Byte3Dictionary (byte[] byteArray) {
		MemoryStream memoryStream = new MemoryStream();
		memoryStream.Write (byteArray, 0, byteArray.Length);
		memoryStream.Position = 0;
		Dictionary<Vector3i, Dictionary<Vector3i, Block>> dictionary = formatter.Deserialize (memoryStream) as Dictionary<Vector3i, Dictionary<Vector3i, Block>>;
		return dictionary;
	}

	public static Dictionary<Vector2i, Block>[] Byte2Dictionary (byte[] byteArray) {
		MemoryStream memoryStream = new MemoryStream();
		memoryStream.Write (byteArray, 0, byteArray.Length);
		memoryStream.Position = 0;
		Dictionary<Vector2i, Block>[] dictionary = formatter.Deserialize (memoryStream) as Dictionary<Vector2i, Block>[];
		return dictionary;
	}
	// Converts to and from byte[] for sending over the network. 


	// Writes directly to a file, for saving to disk. 
	public static void SaveShip (string ShipName, Dictionary<Vector3i, Dictionary<Vector3i, Block>> Block) {
		string filename = Application.persistentDataPath + "/saves/" + World.worldName + "/ships/" + ShipName + ".dct";

		Stream stream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None);
		formatter.Serialize(stream, Block);
		stream.Close();
	}

	public static void SaveStation (Dictionary<Vector2i, Block>[] Block) {
		string filename = Application.persistentDataPath + "/saves/" + World.worldName + "/Station.dct";

		Stream stream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None);
		formatter.Serialize(stream, Block);
		stream.Close();
	}

	public static Dictionary<Vector2i, Block>[] LoadStation () {
		string filename = Application.persistentDataPath + "/saves/" + World.worldName + "/Station.dct";

		Stream stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
		Dictionary<Vector2i, Block>[] dictionary = formatter.Deserialize(stream) as Dictionary<Vector2i, Block>[];
		stream.Close();
		return dictionary;
	}

	public static Dictionary<Vector3i, Dictionary<Vector3i, Block>> LoadShip (string ShipName) {
		string filename = Application.persistentDataPath + "/saves/" + World.worldName + "/ships/" + ShipName + ".dct";

		Stream stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
		Dictionary<Vector3i, Dictionary<Vector3i, Block>> dictionary = formatter.Deserialize(stream) as Dictionary<Vector3i, Dictionary<Vector3i, Block>>;
		stream.Close();
		return dictionary;
	}
	// Writes directly to a file, for saving to disk. 



	/* 
	 * Just in case I want to send objects piece-by-piece rather than whole, I'll leave this code here.
	 * 
	public static byte[] Dictionary2Byte (Dictionary<Vector2i, Block> block) {
		MemoryStream memoryStream = new MemoryStream();
		formatter.Serialize (memoryStream, block);
		byte[] byteArray = memoryStream.ToArray ();
		return byteArray;
	}

	public static byte[] Dictionary3Byte (Dictionary<Vector3i, Block> block) {
		MemoryStream memoryStream = new MemoryStream();
		formatter.Serialize (memoryStream, block);
		byte[] byteArray = memoryStream.ToArray ();
		return byteArray;
	}

	public static Dictionary<Vector3i, Block> Byte3Dictionary (byte[] byteArray) {
		MemoryStream memoryStream = new MemoryStream();
		memoryStream.Write (byteArray, 0, byteArray.Length);
		memoryStream.Position = 0;
		Dictionary<Vector3i, Block> dictionary = formatter.Deserialize (memoryStream) as Dictionary<Vector3i, Block>;
		return dictionary;
	}

	public static Dictionary<Vector2i, Block> Byte2Dictionary (byte[] byteArray) {
		MemoryStream memoryStream = new MemoryStream();
		memoryStream.Write (byteArray, 0, byteArray.Length);
		memoryStream.Position = 0;
		Dictionary<Vector2i, Block> dictionary = formatter.Deserialize (memoryStream) as Dictionary<Vector2i, Block>;
		return dictionary;
	}*/

}