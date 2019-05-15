
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class Ship : VoxelObject
{
	// Arrays.
	private Dictionary<Vector3i, GameObject> meshHolders;
	public Dictionary<Vector3i, Dictionary<Vector3i, Block>> block;
	public Dictionary<Vector3i, int> masses;

	void Awake ()
	{
		transform.parent = World.worldObject.transform;
	}

	void Start ()
	{
		meshHolders = new Dictionary<Vector3i, GameObject> (); 
		if (isServer) {
			if (File.Exists (Application.persistentDataPath + "/saves/" + World.worldName + "/" + gameObject.name + ".dct")) {
				LoadShip ();
			} else {
				block = GetPreset ();
				SaveShip ();
			}
		}
	}

	private void SaveShip ()
	{
		Serialization.SaveShip (gameObject.name, block);
	}

	private void LoadShip ()
	{
		block = Serialization.LoadShip (gameObject.name);

		dataReady = true;
	}

	void Update ()
	{
		if (!firstRenderDone && dataReady) {
			float tickStartTime = Time.realtimeSinceStartup; 
			foreach (Vector3i chunkPos in block.Keys) {
				if (Time.realtimeSinceStartup - tickStartTime < 0.01) {
					GameObject thisMeshHolder;
					meshHolders.TryGetValue (chunkPos, out thisMeshHolder);
					if (thisMeshHolder == null) { 
						RenderShipSegment (chunkPos);
					}
				}
			}
		}
	}

	// Called externally to change a block at a position and also re-render that chunk.
	[ClientRpc]
	public void RpcSetBlock (Vector3i Position, byte Type, byte Material)
	{
		Vector3i realPos = Position;

		Vector3i blockPos = new Vector3i (realPos.x % 16, realPos.y % 16, realPos.z % 16);
		Vector3i chunkPos = new Vector3i (realPos.x / 16, realPos.y / 16, realPos.z / 16);
		byte type = Type, material = Material;

		Dictionary<Vector3i, Block> thisDict;
		block.TryGetValue (chunkPos, out thisDict);

		if (thisDict == null) {
			thisDict = new Dictionary<Vector3i, Block> ();
			block.Add (chunkPos, thisDict);
		}
		thisDict.Remove (blockPos);
		thisDict.Add (blockPos, new Block (type, material));

		GameObject newMeshHolder;
		meshHolders.TryGetValue (chunkPos, out newMeshHolder);
		Destroy (newMeshHolder); 
		meshHolders.Remove (chunkPos);

		RenderShipSegment (chunkPos);

		// Re-render the neighboring chunks, too.
		if (blockPos.x == 0) {
			GameObject nextMeshHolder;
			Vector3i nextPos = new Vector3i (chunkPos.x - 1, chunkPos.y, chunkPos.z);
			meshHolders.TryGetValue (nextPos, out nextMeshHolder);
			Destroy (nextMeshHolder);
			meshHolders.Remove (nextPos);
			RenderShipSegment (nextPos);
		}

		if (blockPos.x == 15) {
			GameObject nextMeshHolder;
			Vector3i nextPos = new Vector3i (chunkPos.x + 1, chunkPos.y, chunkPos.z);
			meshHolders.TryGetValue (nextPos, out nextMeshHolder);
			Destroy (nextMeshHolder);
			meshHolders.Remove (nextPos);
			RenderShipSegment (nextPos);
		}

		if (blockPos.y == 0) {
			GameObject nextMeshHolder;
			Vector3i nextPos = new Vector3i (chunkPos.x, chunkPos.y - 1, chunkPos.z);
			meshHolders.TryGetValue (nextPos, out nextMeshHolder);
			Destroy (nextMeshHolder);
			meshHolders.Remove (nextPos);
			RenderShipSegment (nextPos);
		}

		if (blockPos.y == 15) {
			GameObject nextMeshHolder;
			Vector3i nextPos = new Vector3i (chunkPos.x, chunkPos.y + 1, chunkPos.z);
			meshHolders.TryGetValue (nextPos, out nextMeshHolder);
			Destroy (nextMeshHolder);
			meshHolders.Remove (nextPos);
			RenderShipSegment (nextPos);
		}

		if (blockPos.z == 0) {
			GameObject nextMeshHolder;
			Vector3i nextPos = new Vector3i (chunkPos.x, chunkPos.y, chunkPos.z - 1);
			meshHolders.TryGetValue (nextPos, out nextMeshHolder);
			Destroy (nextMeshHolder);
			meshHolders.Remove (nextPos);
			RenderShipSegment (nextPos);
		}

		if (blockPos.z == 15) {
			GameObject nextMeshHolder;
			Vector3i nextPos = new Vector3i (chunkPos.x, chunkPos.y, chunkPos.z + 1);
			meshHolders.TryGetValue (nextPos, out nextMeshHolder);
			Destroy (nextMeshHolder);
			meshHolders.Remove (nextPos);
			RenderShipSegment (nextPos);
		}
	}



	public Dictionary<Vector3i, Dictionary<Vector3i, Block>> GetPreset ()
	{
		Dictionary<Vector3i, Dictionary<Vector3i, Block>> blawk = new Dictionary<Vector3i, Dictionary<Vector3i, Block>> ();

		if (gameObject.name.Equals ("StationCrossbeam")) { 
			Station station = GetComponentInParent<Station> ();
			for (int y = -station.length / 4; y < station.length / 4; y++) {
				for (int x = -station.radius; x < station.radius + 1; x++) { 
					Vector3i blockPos = new Vector3i (x % 16, y % 16, 0);
					Vector3i chunkPos = new Vector3i (x / 16, y / 16, 0);

					Dictionary<Vector3i, Block> thisDict;
					blawk.TryGetValue (chunkPos, out thisDict);

					if (thisDict == null) {
						thisDict = new Dictionary<Vector3i, Block> ();
						blawk.Add (chunkPos, thisDict);
					}
					thisDict.Remove (blockPos);
					thisDict.Add (blockPos, new Block (1, 1));
				}
				for (int z = -station.radius; z < station.radius + 1; z++) {
					Vector3i blockPos = new Vector3i (0, y % 16, z % 16);
					Vector3i chunkPos = new Vector3i (0, y / 16, z / 16);

					Dictionary<Vector3i, Block> thisDict;
					blawk.TryGetValue (chunkPos, out thisDict);

					if (thisDict == null) {
						thisDict = new Dictionary<Vector3i, Block> ();
						blawk.Add (chunkPos, thisDict);
					}
					thisDict.Remove (blockPos);
					thisDict.Add (blockPos, new Block (1, 1));
				}
			}
		} else {
			Dictionary<Vector3i, Block> thisDictionary = new Dictionary<Vector3i, Block> ();
			thisDictionary.Add (Vector3i.Zero, new Block (1, 1));
			//thisDictionary.Add (new Vector3i (20, 0, 0), new Block (1, 1));
			blawk.Add (Vector3i.Zero, thisDictionary);
		}

		dataReady = true;
		return blawk;
	}

	private Vector3[] GetCubeVertices (Vector3i Pos)
	{
		int xPos = Pos.x, yPos = Pos.y, zPos = Pos.z;

		Vector3[] cubeVertices = new Vector3[] {
			transform.InverseTransformPoint (new Vector3 ((xPos - 0.5f), yPos - 0.5f, (zPos - 0.5f))),
			transform.InverseTransformPoint (new Vector3 ((xPos - 0.5f), yPos - 0.5f, (zPos + 0.5f))),
			transform.InverseTransformPoint (new Vector3 ((xPos - 0.5f), yPos + 0.5f, (zPos - 0.5f))),
			transform.InverseTransformPoint (new Vector3 ((xPos - 0.5f), yPos + 0.5f, (zPos + 0.5f))),
			transform.InverseTransformPoint (new Vector3 ((xPos + 0.5f), yPos - 0.5f, (zPos - 0.5f))),
			transform.InverseTransformPoint (new Vector3 ((xPos + 0.5f), yPos - 0.5f, (zPos + 0.5f))),
			transform.InverseTransformPoint (new Vector3 ((xPos + 0.5f), yPos + 0.5f, (zPos - 0.5f))),
			transform.InverseTransformPoint (new Vector3 ((xPos + 0.5f), yPos + 0.5f, (zPos + 0.5f))),
		};
		return cubeVertices;
	}

	private void SetMesh (List<Mesh> Meshes, Vector3i Pos, List<Vector3i> colliders)
	{
		List<Mesh> meshes = Meshes;
		Vector3i pos = Pos; 

		// Take the meshes added to the list and then place them in a CombineInstance. 
		Mesh[] meshFilters = meshes.ToArray ();
		CombineInstance[] combine = new CombineInstance[meshFilters.Length];
		for (int i = 0; i < meshFilters.Length; i++) {
			combine [i].mesh = meshFilters [i];
			combine [i].transform = transform.localToWorldMatrix;
		}

		if (combine.Length == 0) {
			return;
		}

		// Apply the mesh array to a new instance of the MeshHolder object, and store it in the meshHolders List.
		GameObject newMeshHolder = GameObject.Instantiate (meshHolderTemplate);
		newMeshHolder.GetComponent<MeshFilter> ().mesh.CombineMeshes (combine);
		newMeshHolder.transform.parent = BlockHolder.transform;
		newMeshHolder.name = "MeshHolder [" + pos.x + ", " + pos.y + ", " + pos.z + "]";
		foreach (Vector3i c in colliders) {
			BoxCollider col = newMeshHolder.AddComponent<BoxCollider> ();
			col.center = c;
			col.size = Vector3.one;
		}
		newMeshHolder.transform.localPosition = Vector3.zero;
		newMeshHolder.transform.localRotation = Quaternion.identity;
		meshHolders.Remove (pos);
		meshHolders.Add (pos, newMeshHolder);
	}

	// This function renders one radial chunk of the ship, and tries to be called multiple times per tick.
	private void RenderShipSegment (Vector3i ChunkPos)
	{
		Vector3i chunkPos = ChunkPos;

		Dictionary<Vector3i, Block> thisChunk;
		block.TryGetValue (chunkPos, out thisChunk);

		if (thisChunk == null) {
			thisChunk = new Dictionary<Vector3i, Block> ();
			block.Add (chunkPos, thisChunk);
		} else {
			List<Mesh> meshes = new List<Mesh> ();
			List<Vector3i> colliders = new List<Vector3i> ();
			foreach (Vector3i blockPos in thisChunk.Keys) {
				Block targetBlock;
				if (thisChunk.TryGetValue (blockPos, out targetBlock)) {
					if (targetBlock.type > 0) {
						Vector3i realPos = new Vector3i (blockPos.x + chunkPos.x * 16, blockPos.y + chunkPos.y * 16, blockPos.z + chunkPos.z * 16);
						List<Mesh> newMeshes = DrawBlock (blockPos, chunkPos, realPos); // This function renders one block's worth of faces.
						colliders.Add (realPos);
						meshes.AddRange (newMeshes);
					}
				}
			}
			SetMesh (meshes, chunkPos, colliders);
		}
	}

	private List<Mesh> DrawBlock (Vector3i BlockPos, Vector3i ChunkPos, Vector3i RealPos)
	{
		Vector3i blockPos = BlockPos, chunkPos = ChunkPos, realPos = RealPos;

		List<Mesh> blockMeshes = new List<Mesh> ();

		Dictionary<Vector3i, Block> thisDict;
		block.TryGetValue (chunkPos, out thisDict);

		Block thisBlock;
		thisDict.TryGetValue (blockPos, out thisBlock);
		if (thisBlock == null) {
			return blockMeshes;
		}

		Vector3[] cubeVertices = GetCubeVertices (realPos);
		Vector2[] uvs = GetUVArray (thisBlock.material);

		// Face Rendering
		if (thisBlock.selfFaces [0]) { // +X face

			Dictionary<Vector3i, Block> otherDict = null;
			Block otherBlock = null;
			thisDict.TryGetValue (new Vector3i (blockPos.x + 1, blockPos.y, blockPos.z), out otherBlock);

			if ((realPos.x > 0 && blockPos.x == 15) || (realPos.x < 0 && blockPos.x == 0)) {
				block.TryGetValue (new Vector3i (chunkPos.x + 1, chunkPos.y, chunkPos.z), out otherDict);
				if (otherDict != null) {
					otherDict.TryGetValue (new Vector3i (0, blockPos.y, blockPos.z), out otherBlock);
					otherDict.TryGetValue (new Vector3i (-15, blockPos.y, blockPos.z), out otherBlock);
				}
			} 

			if (otherBlock == null
			    || otherBlock.type == 0
			    || !otherBlock.otherFaces [1]) { // +X face, so check other block's -X face

				Mesh m = new Mesh ();
				m.name = "ScriptedMesh";
				m.vertices = new Vector3[] { cubeVertices [4], cubeVertices [5], cubeVertices [6], cubeVertices [7] };
				m.triangles = new int[] { 2, 1, 0, 1, 2, 3 };
				m.uv = uvs;
				m.RecalculateNormals ();

				blockMeshes.Add (m);
			}
		}

		if (thisBlock.selfFaces [1]) { // -X face

			Dictionary<Vector3i, Block> otherDict = null;
			Block otherBlock = null;
			thisDict.TryGetValue (new Vector3i (blockPos.x - 1, blockPos.y, blockPos.z), out otherBlock);

			if ((realPos.x > 0 && blockPos.x == 0) || (realPos.x < 0 && blockPos.x == -15)) {
				block.TryGetValue (new Vector3i (chunkPos.x - 1, chunkPos.y, chunkPos.z), out otherDict);
				if (otherDict != null) {
					otherDict.TryGetValue (new Vector3i (0, blockPos.y, blockPos.z), out otherBlock);
					otherDict.TryGetValue (new Vector3i (15, blockPos.y, blockPos.z), out otherBlock);
				}
			} 

			if (otherBlock == null
			    || otherBlock.type == 0
			    || !otherBlock.otherFaces [0]) { // -X face, so check other block's +X face

				Mesh m = new Mesh ();
				m.name = "ScriptedMesh";
				m.vertices = new Vector3[] { cubeVertices [0], cubeVertices [1], cubeVertices [2], cubeVertices [3] };
				m.triangles = new int[] { 0, 1, 2, 3, 2, 1 };
				m.uv = uvs;
				m.RecalculateNormals ();

				blockMeshes.Add (m);
			}
		}

		if (thisBlock.selfFaces [2]) { // +Y face

			Dictionary<Vector3i, Block> otherDict = null;
			Block otherBlock = null;
			thisDict.TryGetValue (new Vector3i (blockPos.x, blockPos.y + 1, blockPos.z), out otherBlock);

			if ((realPos.y > 0 && blockPos.y == 15) || (realPos.y < 0 && blockPos.y == 0)) {
				block.TryGetValue (new Vector3i (chunkPos.x, chunkPos.y + 1, chunkPos.z), out otherDict);
				if (otherDict != null) {
					otherDict.TryGetValue (new Vector3i (blockPos.x, 0, blockPos.z), out otherBlock);
					otherDict.TryGetValue (new Vector3i (blockPos.x, -15, blockPos.z), out otherBlock);
				}
			} 

			if (otherBlock == null
			    || otherBlock.type == 0
			    || !otherBlock.otherFaces [3]) { // +Y face, so check other block's -Y face

				Mesh m = new Mesh ();
				m.name = "ScriptedMesh";
				m.vertices = new Vector3[] { cubeVertices [2], cubeVertices [3], cubeVertices [6], cubeVertices [7] };
				m.triangles = new int[] { 0, 1, 2, 3, 2, 1 };
				m.uv = uvs;
				m.RecalculateNormals ();

				blockMeshes.Add (m);
			}
		}

		if (thisBlock.selfFaces [3]) { // -Y face

			Dictionary<Vector3i, Block> otherDict = null;
			Block otherBlock = null;
			thisDict.TryGetValue (new Vector3i (blockPos.x, blockPos.y - 1, blockPos.z), out otherBlock);

			if ((realPos.y > 0 && blockPos.y == 0) || (realPos.y < 0 && blockPos.y == -15)) {
				block.TryGetValue (new Vector3i (chunkPos.x, chunkPos.y - 1, chunkPos.z), out otherDict);
				if (otherDict != null) {
					otherDict.TryGetValue (new Vector3i (blockPos.x, 0, blockPos.z), out otherBlock);
					otherDict.TryGetValue (new Vector3i (blockPos.x, 15, blockPos.z), out otherBlock);
				}
			} 

			if (otherBlock == null
			    || otherBlock.type == 0
			    || !otherBlock.otherFaces [2]) { // -Y face, so check other block's +Y face

				Mesh m = new Mesh ();
				m.name = "ScriptedMesh";
				m.vertices = new Vector3[] { cubeVertices [0], cubeVertices [1], cubeVertices [4], cubeVertices [5] };
				m.triangles = new int[] { 2, 1, 0, 1, 2, 3 };
				m.uv = uvs;
				m.RecalculateNormals ();

				blockMeshes.Add (m);
			}
		}

		if (thisBlock.selfFaces [4]) { // +Z face

			Dictionary<Vector3i, Block> otherDict = null;
			Block otherBlock = null;
			thisDict.TryGetValue (new Vector3i (blockPos.x, blockPos.y, blockPos.z + 1), out otherBlock);

			if ((realPos.z > 0 && blockPos.z == 15) || (realPos.z < 0 && blockPos.z == 0)) {
				block.TryGetValue (new Vector3i (chunkPos.x, chunkPos.y, chunkPos.z + 1), out otherDict);
				if (otherDict != null) {
					otherDict.TryGetValue (new Vector3i (blockPos.x, blockPos.y, 0), out otherBlock);
					otherDict.TryGetValue (new Vector3i (blockPos.x, blockPos.y, -15), out otherBlock);
				}
			} 

			if (otherBlock == null
			    || otherBlock.type == 0
			    || !otherBlock.otherFaces [5]) { // +Z face, so check other block's -Z face

				Mesh m = new Mesh ();
				m.name = "ScriptedMesh";
				m.vertices = new Vector3[] { cubeVertices [1], cubeVertices [3], cubeVertices [5], cubeVertices [7] };
				m.triangles = new int[] { 2, 1, 0, 1, 2, 3 };
				m.uv = uvs;
				m.RecalculateNormals ();

				blockMeshes.Add (m);
			}
		}

		if (thisBlock.selfFaces [5]) { // -Z face

			Dictionary<Vector3i, Block> otherDict = null;
			Block otherBlock = null;
			thisDict.TryGetValue (new Vector3i (blockPos.x, blockPos.y, blockPos.z - 1), out otherBlock);

			if ((realPos.z > 0 && blockPos.z == 15) || (realPos.z < 0 && blockPos.z == 0)) {
				block.TryGetValue (new Vector3i (chunkPos.x, chunkPos.y, chunkPos.z - 1), out otherDict);
				if (otherDict != null) {
					otherDict.TryGetValue (new Vector3i (blockPos.x, blockPos.y, 0), out otherBlock);
					otherDict.TryGetValue (new Vector3i (blockPos.x, blockPos.y, 15), out otherBlock);
				}
			} 

			if (otherBlock == null
			    || otherBlock.type == 0
			    || !otherBlock.otherFaces [4]) { // -Z face, so check other block's +Z face

				Mesh m = new Mesh ();
				m.name = "ScriptedMesh";
				m.vertices = new Vector3[] { cubeVertices [0], cubeVertices [2], cubeVertices [4], cubeVertices [6] };
				m.triangles = new int[] { 0, 1, 2, 3, 2, 1 };
				m.uv = uvs;
				m.RecalculateNormals ();

				blockMeshes.Add (m);
			}
		}
		// End Face Rendering

		// Model Rendering

		if (thisBlock.type > 1) {
			// TO-DO: Implement Model Rendering 

			if (thisBlock.type == 2) {
				//Half-Block
				//byte rot = (byte)(thisBlock.rotation / 4);
				Mesh m = new Mesh ();
				m.name = "ScriptedMesh";
				m.vertices = new Vector3[] { cubeVertices [0], cubeVertices [1], cubeVertices [4], cubeVertices [5] };
				m.triangles = new int[] { 2, 1, 0, 1, 2, 3 };
				m.uv = uvs;
				m.RecalculateNormals ();

				blockMeshes.Add (m);
			}

		}


		// End Model Rendering



		return blockMeshes;
	}

}
