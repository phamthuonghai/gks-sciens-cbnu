using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// This is an editor script that will place trees on the terrain.
/// </summary>

[ExecuteInEditMode]
[AddComponentMenu("Game/Place Trees")]
public class PlaceTrees : MonoBehaviour
{
	[System.Serializable]
	public class Entry
	{
		public GameObject prefab;
		public float idealHeight = 4f;
		public float heightVariance = 2f;
	}

	public Terrain terrain;
	public Transform root;
	public int seed = 12345;
	public Entry[] trees;
	public bool clearNow = false;
	public bool placeNow = false;
	public Vector2 size = new Vector2(40f, 40f);
	public float treeSize = 2f;
	public float variation = 0.75f;
	public LayerMask layerMask = 0;
	public MergeMeshes.PostMerge postMerge = MergeMeshes.PostMerge.DestroyGameObjects;

	Transform mTrans;
	RandomGenerator mRandom;
	Dictionary<int, GameObject> mGroups;

	/// <summary>
	/// This script should only be running in edit mode.
	/// </summary>

	void Awake () { mTrans = transform; if (Application.isPlaying) enabled = false; }

	/// <summary>
	/// Respond to flag changes.
	/// </summary>

	void Update ()
	{
		if (placeNow)
		{
			placeNow = false;
			clearNow = false;
			Repaint();
		}
		else if (clearNow)
		{
			clearNow = false;
			Clear();
		}
	}

	/// <summary>
	/// Clear all trees.
	/// </summary>

	public void Clear ()
	{
		Transform t = (root == null) ? transform : root;
		bool immediate = !Application.isPlaying;

		for (int i = t.childCount; i > 0; )
		{
			Transform child = t.GetChild(--i);
			if (immediate) DestroyImmediate(child.gameObject);
			else Destroy(child.gameObject);
		}
	}

	/// <summary>
	/// Repaint all trees.
	/// </summary>

	public void Repaint ()
	{
		if (trees.Length < 1) return;
		Clear();

		mTrans = transform;

		if (size.x > 0 && size.y > 0)
		{
			mGroups = new Dictionary<int, GameObject>();

			if (mRandom == null) mRandom = new RandomGenerator();
			mRandom.SetSeed((uint)seed);

			if (terrain == null) terrain = Terrain.activeTerrain;
			float heightOffset = terrain.transform.position.y;
			Vector3 start = mTrans.position;
			start.x -= size.x * 0.5f;
			start.z -= size.y * 0.5f;
			float radius = treeSize * 0.5f;
			float cellSize = treeSize * 20f;

			TerrainData data = terrain.terrainData;
			Vector3 terrOffset = terrain.GetPosition();
			Vector3 terrSize = data.size;
			List<int> choices = new List<int>();

			for (float y = 0; y < size.y; y += treeSize)
			{
				for (float x = 0; x < size.x; x += treeSize)
				{
					choices.Clear();
					Vector3 pos = start;
					pos.x += x + variation * mRandom.GenerateRangeFloat();
					pos.z += y + variation * mRandom.GenerateRangeFloat();
					pos.y = terrain.SampleHeight(pos) + heightOffset;

					float randVal = mRandom.GenerateFloat();
					float randRot = mRandom.GenerateFloat() * 360f;
					float randScale = 0.85f + mRandom.GenerateFloat() * 0.3f;

					// Chance based on angle applies to all trees
					{
						float fx = (pos.x - terrOffset.x) / terrSize.x;
						float fy = (pos.z - terrOffset.z) / terrSize.z;
						Vector3 normal = data.GetInterpolatedNormal(fx, fy);

						float chanceAngle = 1f - Mathf.Clamp01(Vector3.Angle(normal, Vector3.up) / 45f);
						chanceAngle = 1f - chanceAngle;
						chanceAngle *= chanceAngle;
						chanceAngle *= chanceAngle;
						chanceAngle = 1f - chanceAngle;

						if (randVal > chanceAngle) continue;
					}

					// Run through all trees
					for (int i = 0; i < trees.Length; ++i)
					{
						Entry ent = trees[i];
						if (ent.prefab == null) continue;

						// Chance based on height applies to individual trees
						{
							float chanceHeight = Mathf.Clamp((pos.y - ent.idealHeight) / ent.heightVariance, -1f, 1f);
							chanceHeight *= chanceHeight;
							chanceHeight *= chanceHeight;
							chanceHeight = 1f - chanceHeight;
							if (randVal > chanceHeight) continue;
						}
						choices.Add(i);
					}

					// Ensure that there is nothing in the area
					if (choices.Count > 0 && layerMask != 0 && !Physics.CheckSphere(pos, radius, layerMask))
					{
						int ix = Mathf.RoundToInt(x / cellSize);
						int iy = Mathf.RoundToInt(y / cellSize);
						float fx = start.x + (ix * cellSize);
						float fy = start.z + (iy * cellSize);

						// Instantiate the tree
						int treeIndex = choices[mRandom.Range(choices.Count)];
						GameObject inst = AddChild(trees[treeIndex].prefab, fx, fy, treeIndex, ix, iy);
						Transform t = inst.transform;
						t.position = pos;
						t.localRotation = Quaternion.Euler(0f, randRot, 0f);
						t.localScale = new Vector3(randScale, randScale, randScale);
					}
				}
			}
			mGroups.Clear();
			mGroups = null;
		}
#if UNITY_EDITOR
		if (!Application.isPlaying)
			UnityEditor.EditorUtility.SetDirty(this);
#endif
	}

	/// <summary>
	/// Helper function -- add a child game object, automatically placing it in the appropriate group.
	/// </summary>

	GameObject AddChild (GameObject prefab, float fx, float fy, int tree, int cx, int cy)
	{
		int index = (tree << 24) | (cy << 12) | cx;
		GameObject go = null;
		
		if (!mGroups.TryGetValue(index, out go))
		{
			go = TerrainKit.AddChild(root != null ? root.gameObject : gameObject);
			go.name = cx + " " + cy + " " + tree;
			go.transform.position = new Vector3(fx, 0f, fy);
			MergeMeshes mm = go.AddComponent<MergeMeshes>();
			mm.afterMerging = postMerge;
			mm.enableTint = true;
			mGroups.Add(index, go);
		}
		return TerrainKit.AddChild(go, prefab);
	}

	/// <summary>
	/// Invalidate all trees that fall within the specified sphere (Y is ignored).
	/// </summary>

	public void Invalidate (Vector3 pos, float radius)
	{
		// We can't do anything if the original objects have already been destroyed
		if (postMerge == MergeMeshes.PostMerge.DestroyGameObjects) return;

		List<Transform> list = new List<Transform>();

		float xmin = pos.x - radius;
		float xmax = pos.x + radius;
		float ymin = pos.z - radius;
		float ymax = pos.z + radius;

		GetGroups(xmin, ymin, list);
		GetGroups(xmax, ymin, list);
		GetGroups(xmin, ymax, list);
		GetGroups(xmax, ymax, list);

		for (int i = 0, imax = list.Count; i < imax; ++i)
		{
			Transform t = list[i];
			MergeMeshes mm = t.GetComponent<MergeMeshes>();
			if (mm != null) mm.Clear();
			Validate(t, xmin, ymin, xmax, ymax);
			if (mm != null) mm.Merge(true);
		}
	}

	/// <summary>
	/// Retrieve the merged groups taking care of the specified position.
	/// </summary>

	void GetGroups (float x, float y, List<Transform> list)
	{
		if (root == null) root = mTrans;

		Vector3 start = mTrans.position;
		start.x -= size.x * 0.5f;
		start.z -= size.y * 0.5f;

		float cellSize = treeSize * 20f;
		int ix = Mathf.RoundToInt((x - start.x) / cellSize);
		int iy = Mathf.RoundToInt((y - start.z) / cellSize);

		string match = ix + " " + iy;

		for (int i = 0, imax = root.childCount; i < imax; ++i)
		{
			Transform child = root.GetChild(i);
			if (child.name.StartsWith(match) && !list.Contains(child)) list.Add(child);
		}
	}

	/// <summary>
	/// Run through all of the specified transform's children and validate their position.
	/// </summary>

	bool Validate (Transform t, float xmin, float ymin, float xmax, float ymax)
	{
		bool changed = false;

		for (int i = 0, imax = t.childCount; i < imax; ++i)
		{
			Transform child = t.GetChild(i);
			Vector3 pos = child.position;

			// Not within range? No point in continuing
			if (pos.x < xmin || pos.x > xmax || pos.z < ymin || pos.z > ymax) continue;

			// Check to see if the height is the same as it was
			float heightOffset = terrain.transform.position.y;
			float height = terrain.SampleHeight(pos) + heightOffset;

			// Make sure the tree is grounded
			if (Mathf.Abs(pos.y - height) > 0.01f)
			{
				pos.y = height;
				child.position = pos;
				changed = true;
			}
			
			// Check to see if the height has changed -- and if not, check if anything is now blocking the tree
			bool blocked = Physics.CheckSphere(child.position, treeSize * 0.5f, layerMask);

			// Deactivate the object
			if (blocked)
			{
				child.gameObject.SetActive(false);
				changed = true;
			}
		}
		return changed;
	}

	/// <summary>
	/// It's always better to show the area.
	/// </summary>

	void OnDrawGizmos ()
	{
		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.color = Color.cyan;
		Gizmos.DrawWireCube(Vector3.zero, new Vector3(size.x, 20f, size.y));
	}
}
