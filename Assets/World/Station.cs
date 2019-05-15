
// Almost working multi-dictionary code

using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class Station : VoxelObject
{
	public static GameObject stationObject;
	public static Station station;

	// These values are manipulateable within Unity to create different results. Values below are the recommended values.
	// These values are set on the server and synced to all clients.
	[SyncVar] public int length = 20;
	[SyncVar] public int circumference = 500;
	[SyncVar] public float targetGravity = 0;
	[SyncVar] public float currentGravity = 10;
	[SyncVar] [HideInInspector] public float currentGravityScaler = 10;
	[SyncVar] [HideInInspector] public int radius = 50;

	private InGameMenu menu;

	// Arrays.
	private byte[][] data;
	private Dictionary<int, GameObject> meshHolders;
	public Dictionary<Vector2i, Block>[] block;
	public int[] masses;

	void Awake ()
	{
		stationObject = gameObject;
		station = this;

		stationObject.transform.parent = World.worldObject.transform;
		//stationObject.transform.localPosition = Vector3.one * 100000.0f;
		//stationObject.name = "Station";
	}

	void Start ()
	{
		menu = GameObject.Find ("uiMenu").GetComponent<InGameMenu> ();
		meshHolders = new Dictionary<int, GameObject> ();
		masses = new int[circumference];

		if (isServer) {
			if (File.Exists (Application.persistentDataPath + "/saves/" + World.worldName + "/Station.dct")) {
				LoadStation ();
			} else {
				CreateStation ();
				SaveStation ();
			}
		} else {
			Commands.commands.CmdGetStation ();
		}

	}

	[TargetRpc]
	public void TargetSendStationMeta (NetworkConnection Connection, ObjMetadata Meta)
	{
		stationObject.name = Meta.name;
		stationObject.transform.localPosition = Meta.position;
		stationObject.transform.localRotation = Meta.rotation;
		station.circumference = Meta.circumference;
		station.targetGravity = Meta.targetGravity;

		data = new byte[Meta.dataSize][];
	}

	[TargetRpc]
	public void TargetSendStationPiece (NetworkConnection Connection, byte[] Data, int Index)
	{
		data [Index] = Data;
	}

	private void SaveStation ()
	{
		Serialization.SaveStation (block);
	}

	private void LoadStation ()
	{
		block = Serialization.LoadStation ();
		circumference = block.Length;
		masses = new int[block.Length];
		radius = Mathf.RoundToInt (circumference / (2 * Mathf.PI));

		dataReady = true;
	}

	private void CreateStation ()
	{
		if (circumference < 20 || circumference > 5000) {
			Menu.errorReason = "Station circumference out of bounds!\nGiven: " + circumference + " - Limits: 200 to 5000";
			menu.QuitGame ();
		}
		if (length < 10 || length > 200) {
			Menu.errorReason = "Station length out of bounds!\nGiven: " + length + " - Limits: 10 to 200";
			menu.QuitGame ();
		}
		if (circumference * length > 500000) {
			Menu.errorReason = "Station size out of bounds!\nGiven: " + circumference * length + " - Max: 500000)";
			menu.QuitGame ();
		}

		block = new Dictionary<Vector2i, Block>[circumference];

		radius = (short)Mathf.RoundToInt (circumference / (2 * Mathf.PI));

		for (int circ = 0; circ < circumference; circ++) {
			block [circ] = new Dictionary<Vector2i, Block> (); 
		}

		CreateRing (-1);

		dataReady = true;
	}

	private void CreateRing (sbyte Height)
	{
		sbyte height = Height;
		for (int circ = 0; circ < circumference; circ++) {
			for (int lengthPass = -length / 2; lengthPass < length / 2; lengthPass++) {
				Vector2i blockPos = new Vector2i (lengthPass, height); 
				block [circ].Add (blockPos, new Block ());
				mass += 100;
			}
		}
	}

	private int circumferencePass = 0;
	// This value must persist outside of Update().
	void Update ()
	{
		if (dataReady) {
			if (!firstRenderDone) {
				float tickStartTime = Time.realtimeSinceStartup;
				while (circumferencePass < circumference && Time.realtimeSinceStartup - tickStartTime < 0.01) {
					RenderStationSegment (circumferencePass);
					circumferencePass++;
				}
				if (circumferencePass < circumference) {
					float percentLoaded = Mathf.Floor (((float)circumferencePass) / ((float)circumference) * 100);
					print ("Rendering Station: " + percentLoaded + "%... ");
				} else {
					print ("Rendering Complete! ");
					circumferencePass = 0;
					firstRenderDone = true;
				}
			} 
		} else {
			// If dataReady is false, we'll check if we have all the pieces of data, and if so, reassemble the data.
			if (data != null && data [0] != null) {
				dataReady = true;
				foreach (byte[] piece in data) {
					if (piece == null) {
						dataReady = false;
					}
				}
			}
			if (dataReady) {
				byte[] allData = new byte[0];
				foreach (byte[] piece in data) {
					byte[] newAllData = new byte[allData.Length + piece.Length];
					allData.CopyTo (newAllData, 0);
					piece.CopyTo (newAllData, allData.Length);
					allData = newAllData;
				}
				block = Serialization.Byte2Dictionary (allData);
				print ("We've just received the last of the Station data! Rendering... ");
			} else {
				print ("Data is not ready, we're not rendering yet! ");

			}
		}

	}

	private Vector3[] GetCubeVertices (int CircumferencePos, int LengthPos, int RadiusPos)
	{

		int circumferencePos = CircumferencePos, lengthPos = LengthPos, radiusPos = RadiusPos;

		float PosAngle = GetAngle (circumferencePos + 0.5f); 
		float NegAngle = GetAngle (circumferencePos - 0.5f); 

		float CosPosAngle = Mathf.Cos (PosAngle);
		float CosNegAngle = Mathf.Cos (NegAngle);
		float SinPosAngle = Mathf.Sin (PosAngle);
		float SinNegAngle = Mathf.Sin (NegAngle);

		Vector3[] cubeVertices = new Vector3[] {
			new Vector3 ((radius - radiusPos - 0.5f) * CosNegAngle, lengthPos - 0.5f, (radius - radiusPos - 0.5f) * SinNegAngle),
			new Vector3 ((radius - radiusPos - 0.5f) * CosNegAngle, lengthPos + 0.5f, (radius - radiusPos - 0.5f) * SinNegAngle),
			new Vector3 ((radius - radiusPos - 0.5f) * CosPosAngle, lengthPos - 0.5f, (radius - radiusPos - 0.5f) * SinPosAngle),
			new Vector3 ((radius - radiusPos - 0.5f) * CosPosAngle, lengthPos + 0.5f, (radius - radiusPos - 0.5f) * SinPosAngle),
			new Vector3 ((radius - radiusPos + 0.5f) * CosNegAngle, lengthPos - 0.5f, (radius - radiusPos + 0.5f) * SinNegAngle),
			new Vector3 ((radius - radiusPos + 0.5f) * CosNegAngle, lengthPos + 0.5f, (radius - radiusPos + 0.5f) * SinNegAngle),
			new Vector3 ((radius - radiusPos + 0.5f) * CosPosAngle, lengthPos - 0.5f, (radius - radiusPos + 0.5f) * SinPosAngle),
			new Vector3 ((radius - radiusPos + 0.5f) * CosPosAngle, lengthPos + 0.5f, (radius - radiusPos + 0.5f) * SinPosAngle),
		};

		return cubeVertices;
	}

	private float GetAngle (float CircumferencePos)
	{  
		float angle = 2 * Mathf.PI * (CircumferencePos / circumference); // Finds angle in radians (ratio of 2pi)
		return angle;
	}

	// Called externally to change a block.
	[ClientRpc]
	public void RpcSetBlock (Vector3i Position, byte Type, byte Material)
	{
		int circumferencePos = Position.x, lengthPos = Position.y, invRadiusPos = Position.z;
		byte type = Type, material = Material;

		if (type > 0 && invRadiusPos < radius / 2) {
			return;
		}

		Vector2i blockPos = new Vector2i (lengthPos, radius - invRadiusPos);

		if (circumferencePos == circumference) {
			circumferencePos = 0;
		} else if (circumferencePos < 0) {
			circumferencePos = circumference - 1;
		}

		block [circumferencePos].Remove (blockPos);
		block [circumferencePos].Add (blockPos, new Block (type, material));

		GameObject thisMeshHolder;
		meshHolders.TryGetValue (circumferencePos, out thisMeshHolder);
		Destroy (thisMeshHolder);
		meshHolders.Remove (circumferencePos);
		RenderStationSegment (circumferencePos);

		int negCircumferencePos = (circumferencePos - 1) % circumference;
		if (negCircumferencePos < 0) {
			negCircumferencePos += circumference;
		}
		meshHolders.TryGetValue (negCircumferencePos, out thisMeshHolder);
		Destroy (thisMeshHolder);
		meshHolders.Remove (negCircumferencePos);
		RenderStationSegment (negCircumferencePos);

		int posCircumferencePos = (circumferencePos + 1) % circumference;
		meshHolders.TryGetValue (posCircumferencePos, out thisMeshHolder);
		Destroy (thisMeshHolder);
		meshHolders.Remove (posCircumferencePos);
		RenderStationSegment (posCircumferencePos);

	}

	protected void SetMesh (List<Mesh> Meshes, int XPos)
	{
		int xPos = XPos;

		// Take the meshes added to the list and then place them in a CombineInstance. 
		Mesh[] meshFilters = Meshes.ToArray ();
		CombineInstance[] combine = new CombineInstance[meshFilters.Length];
		for (int i = 0; i < meshFilters.Length; i++) {
			combine [i].mesh = meshFilters [i];
			combine [i].transform = transform.localToWorldMatrix;
		}

		if (combine.Length == 0) {
			return;
		}

		// Apply the mesh array to a new instance of the MeshHolder object, which can be referenced later in the stationBlock[] array. 
		GameObject newMeshHolder = GameObject.Instantiate (meshHolderTemplate);
		newMeshHolder.GetComponent<MeshFilter> ().mesh.CombineMeshes (combine);
		newMeshHolder.GetComponent<MeshCollider> ().sharedMesh = newMeshHolder.GetComponent<MeshFilter> ().sharedMesh;
		newMeshHolder.transform.parent = BlockHolder.transform;
		newMeshHolder.name = "MeshHolder [" + xPos + "]";
		newMeshHolder.GetComponent<SetTextures> ().circumferencePos = xPos;
		meshHolders.Remove (xPos);
		meshHolders.Add (xPos, newMeshHolder);
	}

	// This function renders one radial segment of the station, and tries to be called multiple times per tick.
	private void RenderStationSegment (int CircumferencePos)
	{
		int circumferencePos = CircumferencePos;

		List<Mesh> meshes = new List<Mesh> ();

		masses [circumferencePos] = 0;

		foreach (Vector2i pos in block[circumferencePos].Keys) {
			Block targetBlock;
			if (block [circumferencePos].TryGetValue (pos, out targetBlock)) {
				
				if (targetBlock.type > 0) {
					List<Mesh> newMeshes = DrawBlock (circumferencePos, Mathf.RoundToInt (pos.x), Mathf.RoundToInt (pos.y)); // This function renders one block's worth of faces.
					meshes.AddRange (newMeshes);
				}
			}
		}

		SetMesh (meshes, circumferencePos);
	}

	// This function renders one block's worth of faces.
	// Warning: Quite complicated.
	private List<Mesh> DrawBlock (int CircumferencePos, int LengthPos, int RadiusPos)
	{
		int circumferencePos = CircumferencePos, lengthPos = LengthPos, radiusPos = RadiusPos;

		List<Mesh> blockMeshes = new List<Mesh> ();

		Vector2i thisPos = new Vector2i (lengthPos, radiusPos);
		Block thisBlock;
		block [circumferencePos].TryGetValue (thisPos, out thisBlock);

		Vector3[] cubeVertices = GetCubeVertices (circumferencePos, lengthPos, radiusPos);
		Vector2[] uvs = GetUVArray (thisBlock.material);

		// Face Rendering
		if (thisBlock.selfFaces [0]) { // +C face
			Vector2i otherPos = new Vector2i (lengthPos, radiusPos);
			Block otherBlock;
			block [(circumferencePos + 1) % (circumference)].TryGetValue (otherPos, out otherBlock);

			if (otherBlock == null
			    || otherBlock.type == 0
			    || !otherBlock.otherFaces [1]) { // +C face, so check other block's -C face

				Mesh m = new Mesh ();
				m.name = "ScriptedMesh";
				m.vertices = new Vector3[] { cubeVertices [2], cubeVertices [3], cubeVertices [6], cubeVertices [7] };
				m.triangles = new int[] { 2, 1, 0, 1, 2, 3 };
				m.uv = uvs;
				m.RecalculateNormals ();

				blockMeshes.Add (m);
			}
		}

		if (thisBlock.selfFaces [1]) { // -C face
			Vector2i otherPos = new Vector2i (lengthPos, radiusPos);
			Block otherBlock;
			int negCircumferencePos = (circumferencePos - 1) % circumference;
			if (negCircumferencePos < 0) {
				negCircumferencePos += circumference;
			}
			block [negCircumferencePos].TryGetValue (otherPos, out otherBlock);

			if (otherBlock == null
			    || otherBlock.type == 0
			    || !otherBlock.otherFaces [0]) { // -C face, so check other block's +C face
			
				Mesh m = new Mesh ();
				m.name = "ScriptedMesh";
				m.vertices = new Vector3[] { cubeVertices [0], cubeVertices [1], cubeVertices [4], cubeVertices [5] };
				m.triangles = new int[] { 1, 2, 0, 2, 1, 3 };
				m.uv = uvs;
				m.RecalculateNormals ();

				blockMeshes.Add (m);
			}
		}

		if (thisBlock.selfFaces [2]) { // +Y face
			Vector2i otherPos = new Vector2i (lengthPos + 1, radiusPos);
			Block otherBlock;
			block [circumferencePos].TryGetValue (otherPos, out otherBlock);

			if (otherBlock == null
			    || otherBlock.type == 0
			    || !otherBlock.otherFaces [3]) { // +Y face, so check other block's -Y face

				Mesh m = new Mesh ();
				m.name = "ScriptedMesh";
				m.vertices = new Vector3[] { cubeVertices [1], cubeVertices [3], cubeVertices [5], cubeVertices [7] };
				m.triangles = new int[] { 1, 2, 0, 2, 1, 3 };
				m.uv = uvs;
				m.RecalculateNormals ();

				blockMeshes.Add (m);
			}
		}

		if (thisBlock.selfFaces [3]) { // -Y face
			Vector2i otherPos = new Vector2i (lengthPos - 1, radiusPos);
			Block otherBlock;
			block [circumferencePos].TryGetValue (otherPos, out otherBlock);

			if (otherBlock == null
			    || otherBlock.type == 0
			    || !otherBlock.otherFaces [2]) { // -Y face, so check other block's +Y face

				Mesh m = new Mesh ();
				m.name = "ScriptedMesh";
				m.vertices = new Vector3[] { cubeVertices [0], cubeVertices [2], cubeVertices [4], cubeVertices [6] };
				m.triangles = new int[] { 2, 1, 0, 1, 2, 3 };
				m.uv = uvs;
				m.RecalculateNormals ();

				blockMeshes.Add (m);
			}
		}
		if (thisBlock.selfFaces [4]) { // +R face
			Vector2i otherPos = new Vector2i (lengthPos, radiusPos + 1);
			Block otherBlock;
			block [circumferencePos].TryGetValue (otherPos, out otherBlock);

			if (otherBlock == null
			    || otherBlock.type == 0
			    || !otherBlock.otherFaces [5]) { // +R face, so check other block's -R face

				Mesh m = new Mesh ();
				m.name = "ScriptedMesh";
				m.vertices = new Vector3[] { cubeVertices [0], cubeVertices [1], cubeVertices [2], cubeVertices [3] };
				m.uv = uvs;
				m.triangles = new int[] { 2, 1, 0, 1, 2, 3 };
				m.RecalculateNormals ();

				blockMeshes.Add (m);
			}
		}

		if (thisBlock.selfFaces [5]) { // -R face
			Vector2i otherPos = new Vector2i (lengthPos, radiusPos - 1);
			Block otherBlock;
			block [circumferencePos].TryGetValue (otherPos, out otherBlock);

			if (otherBlock == null
			    || otherBlock.type == 0
			    || !otherBlock.otherFaces [4]) { // -R face, so check other block's +R face
				// && otherBlock.material != thisBlock.material
				Mesh m = new Mesh ();
				m.name = "ScriptedMesh";
				m.vertices = new Vector3[] { cubeVertices [4], cubeVertices [5], cubeVertices [6], cubeVertices [7] };
				m.uv = uvs;
				m.triangles = new int[] { 0, 1, 2, 3, 2, 1 };
				m.RecalculateNormals ();

				blockMeshes.Add (m);
			}
		}
		// End Face Rendering

		// Model Rendering




		// End Model Rendering

		return blockMeshes;
	}

}
