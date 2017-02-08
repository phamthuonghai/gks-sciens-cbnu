using UnityEngine;
using LibNoise;

[ExecuteInEditMode]
public class CreateTerrain : MonoBehaviour
{
	static int mLastSeed = 0;

	/// <summary>
	/// Used in Edit mode -- generate a new terrain.
	/// </summary>

	public bool generateNow = false;

	/// <summary>
	/// Terrain this script will be working with.
	/// </summary>

	public Terrain terrain;

	/// <summary>
	/// Script used to paint the terrain after generation.
	/// </summary>

	public PaintTerrain paintTerrain;

	/// <summary>
	/// Script used to place trees after generation.
	/// </summary>

	public PlaceTrees placeTrees;

	/// <summary>
	/// Is there a Tasharen Water present? Link it here.
	/// </summary>

	public Renderer waterRenderer;

	/// <summary>
	/// Material to assign to the water after terrain generation.
	/// </summary>

	public Material waterMaterial;

	/// <summary>
	/// Random seed used for terrain generation.
	/// </summary>

	public int seed = 123;

	/// <summary>
	/// Density of the first perlin noise.
	/// </summary>

	[Range(1, 16)]
	public int noiseDensity1 = 6;

	/// <summary>
	/// Curve for the first noise map.
	/// </summary>

	public AnimationCurve curve1;

	/// <summary>
	/// Density of the second perlin noise.
	/// </summary>

	[Range(1, 16)]
	public int noiseDensity2 = 6;

	/// <summary>
	/// Curve for the second noise map.
	/// </summary>

	public AnimationCurve curve2;

	/// <summary>
	/// Density of the perlin noise that's used to mix the first two.
	/// </summary>

	[Range(2, 32)]
	public int controlDensity = 8;

	/// <summary>
	/// Curve for the noise map that mixes the two noise maps together.
	/// </summary>

	public AnimationCurve control;

	/// <summary>
	/// Distance-sampled value added to the height.
	/// </summary>

	public AnimationCurve distanceAdjustment;

	/// <summary>
	/// Distance-sampled value that the sampled height gets multiplied by.
	/// </summary>

	public AnimationCurve distanceMultiplier;

	/// <summary>
	/// Sampled height values gets adjusted by this curve.
	/// </summary>

	public AnimationCurve heightAdjustment;

	/// <summary>
	/// Re-generate the terrain.
	/// </summary>

	void OnEnable ()
	{
		mLastSeed = seed;
		Generate(false);
	}

	/// <summary>
	/// Create the terrain if asked for it.
	/// </summary>

	void Update () { if (generateNow) Generate(!Application.isPlaying); }

	/// <summary>
	/// Generate everything.
	/// </summary>

	public void Generate (bool force)
	{
		generateNow = false;

		if (terrain == null) terrain = Terrain.activeTerrain;
		if (terrain == null) return;

		// Re-generate the terrain
		if (force || mLastSeed != seed)
		{
			mLastSeed = seed;

			// Generate the terrain by sampling the noise.
			Vector3 offset = transform.position;
			Vector3 size = terrain.terrainData.size;
			int vertexWidth = terrain.terrainData.heightmapResolution;

			float invSize = size.x / (vertexWidth - 1f);

			Perlin n0 = new Perlin();
			n0.Seed = seed;
			n0.Frequency = noiseDensity1 / 256f;
			n0.OctaveCount = 3;

			Perlin n1 = new Perlin();
			n1.Seed = seed;
			n1.Frequency = noiseDensity2 / 256f;
			n1.OctaveCount = 6;

			Perlin nc = new Perlin();
			nc.Seed = seed + 321;
			nc.Frequency = controlDensity / 256f;
			nc.OctaveCount = 2;

			float[,] heights = new float[vertexWidth, vertexWidth];
			float center = (vertexWidth - 1) * 0.5f;
			float invCenter = 1f / center;
			Vector2 dist = Vector2.zero;

			for (int y = 0; y < vertexWidth; ++y)
			{
				float fy = offset.z + invSize * y;
				dist.y = Mathf.Abs(center - y) * invCenter;

				for (int x = 0; x < vertexWidth; ++x)
				{
					float fx = offset.x + invSize * x;
					dist.x = Mathf.Abs(center - x) * invCenter;
					float dm = dist.magnitude;
					dm = Mathf.Min(1f, dm);

					float multiplier = distanceMultiplier.Evaluate(dm);
					float addition = distanceAdjustment.Evaluate(dm);

					float h0 = (float)n0.GetValue(fx, 0f, fy) * 0.5f + 0.5f;
					h0 = Mathf.Clamp01(h0) * multiplier;
					float f0 = curve1.Evaluate(h0);
					f0 = heightAdjustment.Evaluate(f0);

					float h1 = (float)n1.GetValue(fx, 0f, fy) * 0.5f + 0.5f;
					h1 = Mathf.Clamp01(h1) * multiplier;
					float f1 = curve2.Evaluate(h1);
					f1 = heightAdjustment.Evaluate(f1);

					float ct = Mathf.Clamp01((float)nc.GetValue(fx, 0f, fy) * 0.5f + 0.5f);
					ct = control.Evaluate(ct);
					ct = Mathf.Lerp(f0, f1, ct);
					ct = Mathf.Clamp01(ct + addition);
					heights[x, y] = ct;
				}
			}
			terrain.terrainData.SetHeights(0, 0, heights);

			// Repaint all trees
			if (placeTrees != null)
			{
				placeTrees.terrain = terrain;
				placeTrees.seed = seed;
				placeTrees.Repaint();
			}

			// Repaint the terrain
			if (paintTerrain != null)
			{
				paintTerrain.terrain = terrain;
				paintTerrain.Repaint();
			}

#if UNITY_EDITOR
			if (!Application.isPlaying)
				UnityEditor.EditorUtility.SetDirty(this);
#endif
		}
		else if (paintTerrain != null)
		{
			paintTerrain.terrain = terrain;
			paintTerrain.UpdateShaderUniforms();
		}

		// Replace the water
		if (waterRenderer != null && waterMaterial != null)
			waterRenderer.sharedMaterial = waterMaterial;
	}
}
