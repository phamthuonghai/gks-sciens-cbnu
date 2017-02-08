using UnityEngine;

/// <summary>
/// Script used to paint the terrain.
/// </summary>

[ExecuteInEditMode]
public class PaintTerrain : MonoBehaviour
{
	/// <summary>
	/// Used in edit mode -- will repaint the terrain.
	/// </summary>

	public bool paintNow = false;

	/// <summary>
	/// Whether the result will be baked into the terrain, or remain dynamic.
	/// Dynamic allows real-time modifications, and takes much less space.
	/// Static lets you use a standard Unity Terrain shader instead.
	/// </summary>

	public bool bakeResult = false;

	/// <summary>
	/// Terrain this script will be working with.
	/// </summary>

	public Terrain terrain;

	/// <summary>
	/// Fog and background clear color.
	/// </summary>

	public Color fogColor = new Color32(180, 224, 254, 255);

	/// <summary>
	/// Texture used to paint slopes.
	/// </summary>

	public Texture2D slopeTexture;

	/// <summary>
	/// Texture used by the shore and sea floor.
	/// </summary>

	public Texture2D shoreTexture;

	/// <summary>
	/// Texture used at low heights.
	/// </summary>

	public Texture2D grassTexture;

	/// <summary>
	/// Texture used up on the hills.
	/// </summary>

	public Texture2D hillTexture;

	/// <summary>
	/// Optional tint texture (opposite of a detail texture).
	/// </summary>

	public Texture2D tintTexture;

	/// <summary>
	/// Optional normal map for the slope (used at high quality).
	/// </summary>

	public Texture2D slopeNormal;

	/// <summary>
	/// Texture tiling.
	/// </summary>

	public Vector4 textureTileSize = Vector4.zero;

	/// <summary>
	/// Slope is in degrees:
	/// Everything below the X is not affected by the slope texture.
	/// Everything above Y is 100% slope texture. Blend in-between.
	/// </summary>
	
	public Vector2 slopeRange = new Vector2(40f, 43f);

	/// <summary>
	/// Tex1 -> Tex2 blending.
	/// </summary>
 
	public Vector2 blendRange0 = new Vector2(0.15f, 0.4f);

	/// <summary>
	/// Tex1 -> Tex2 blending.
	/// </summary>

	public Vector2 blendRange1 = new Vector2(1.5f, 3f);

	/// <summary>
	/// Access to splat prototypes.
	/// </summary>

	public SplatPrototype[] splatPrototypes
	{
		get
		{
			TerrainData data = (terrain != null) ? terrain.terrainData : null;
			return (data != null) ? data.splatPrototypes : null;
		}
	}

	/// <summary>
	/// This script shouldn't be active while the application is playing.
	/// </summary>

	void OnEnable ()
	{
		RenderSettings.fogColor = fogColor;
		if (Camera.main != null) Camera.main.backgroundColor = fogColor;

		if (terrain == null) terrain = Terrain.activeTerrain;

		if (terrain != null && terrain.terrainData != null)
		{
			SplatPrototype[] splats = terrain.terrainData.splatPrototypes;

			if (splats != null && splats.Length == 4)
			{
				if (slopeTexture == null) slopeTexture = splats[0].texture;
				if (shoreTexture == null) shoreTexture = splats[1].texture;
				if (grassTexture == null) grassTexture = splats[2].texture;
				if (hillTexture  == null) hillTexture  = splats[3].texture;

				if (textureTileSize.x < 1f) textureTileSize.x = splats[0].tileSize.x;
				if (textureTileSize.y < 1f) textureTileSize.y = splats[1].tileSize.x;
				if (textureTileSize.z < 1f) textureTileSize.z = splats[2].tileSize.x;
				if (textureTileSize.w < 1f) textureTileSize.w = splats[3].tileSize.x;
			}
		}
		Repaint();
	}

	/// <summary>
	/// Generate the terrain if asked for it.
	/// </summary>

	void Update ()
	{
		if (paintNow)
		{
			paintNow = false;
			Repaint();
		}
	}

	/// <summary>
	/// Update the shader uniforms.
	/// </summary>

	public void UpdateShaderUniforms ()
	{
		if (terrain == null) terrain = Terrain.activeTerrain;
		if (terrain == null) return;

		float rangeFactor0 = (blendRange0.y - blendRange0.x);
		rangeFactor0 = (rangeFactor0 == 0f) ? 0f : 1.0f / Mathf.Abs(rangeFactor0);

		float rangeFactor1 = (blendRange1.y - blendRange1.x);
		rangeFactor1 = (rangeFactor1 == 0f) ? 0f : 1.0f / Mathf.Abs(rangeFactor1);

		float slopeFactor = 1f / (slopeRange.y - slopeRange.x);
		Vector4 data = new Vector4(
			terrain.transform.position.y,
			(terrain.terrainData != null) ? terrain.terrainData.heightmapScale.y : 16f,
			slopeFactor, -slopeRange.x * slopeFactor);

		Shader.SetGlobalVector("terrainData", data);
		Shader.SetGlobalVector("terrainBlend", new Vector4(rangeFactor0, -blendRange0.x * rangeFactor0, rangeFactor1, -blendRange1.x * rangeFactor1));
		if (tintTexture != null) Shader.SetGlobalTexture("terrainTint", tintTexture);
		if (slopeNormal != null) Shader.SetGlobalTexture("terrainNormal", slopeNormal);
	}

	/// <summary>
	/// Set up the terrain's textures.
	/// </summary>

	public void Repaint ()
	{
		if (terrain == null) terrain = Terrain.activeTerrain;

		if (terrain == null || terrain.terrainData == null)
		{
			Debug.LogError("No terrain to work with", this);
			return;
		}

		if (bakeResult) RepaintAndBake();
		else DynamicRepaint();
	}

	/// <summary>
	/// Set up the terrain's textures. Use this texture if you want the textures to be dynamic, based on terrain's geometry.
	/// </summary>

	public void DynamicRepaint()
	{
		SplatPrototype[] splats = new SplatPrototype[4];
		for (int i = 0; i < 4; ++i) splats[i] = new SplatPrototype();

		if (slopeTexture) splats[0].texture = slopeTexture;
		if (shoreTexture) splats[1].texture = shoreTexture;
		if (grassTexture) splats[2].texture = grassTexture;
		if (hillTexture)  splats[3].texture = hillTexture;

		splats[0].tileSize = new Vector2(textureTileSize.x, textureTileSize.x);
		splats[1].tileSize = new Vector2(textureTileSize.y, textureTileSize.y);
		splats[2].tileSize = new Vector2(textureTileSize.z, textureTileSize.z);
		splats[3].tileSize = new Vector2(textureTileSize.w, textureTileSize.w);

		terrain.terrainData.splatPrototypes = splats;
		UpdateShaderUniforms();
	}

	/// <summary>
	/// Generate the terrain alpha mask. Use this function if you want to bake the result into the terrain.
	/// </summary>

	public void RepaintAndBake ()
	{
		TerrainData data = (terrain != null) ? terrain.terrainData : null;

		if (data == null)
		{
			Debug.LogError("Terrain data has not yet been set on " + terrain.name, terrain);
			return;
		}

		if (data.splatPrototypes == null || data.splatPrototypes.Length == 0)
		{
			Debug.LogError("You must set up terrain splat prototypes on " + name + " prior to calling PaintTerrait.Generate.");
			return;
		}

		// Generate the alpha map
		float[,,] alphaMap = Repaint(data, data.splatPrototypes.Length, terrain.transform.position.y);
		data.SetAlphamaps(0, 0, alphaMap);

#if UNITY_EDITOR
		if (!Application.isPlaying)
			UnityEditor.EditorUtility.SetDirty(this);
#endif
	}

	/// <summary>
	/// Calculate the alpha mask for the terrain.
	/// </summary>

	float[, ,] Repaint (TerrainData td, int channels, float heightOffset)
	{
		float slopeFactor = (slopeRange.y - slopeRange.x);
		slopeFactor = (slopeFactor == 0f) ? 0f : 1.0f / Mathf.Abs(slopeFactor);

		float rangeFactor0 = (blendRange0.y - blendRange0.x);
		rangeFactor0 = (rangeFactor0 == 0f) ? 0f : 1.0f / Mathf.Abs(rangeFactor0);

		float rangeFactor1 = (blendRange1.y - blendRange1.x);
		rangeFactor1 = (rangeFactor1 == 0f) ? 0f : 1.0f / Mathf.Abs(rangeFactor1);

		int size = td.alphamapResolution;

		float xm = 1.0f / (size - 1);
		float ym = 1.0f / (size - 1);

		float[,,] map = new float[size, size, channels];

		for (int y = 0; y < size; ++y)
		{
			float fy = y * ym;

			for (int x = 0; x < size; ++x)
			{
				float fx = x * xm;

				// X and Y are intentionally flipped. For some odd reason Unity has them flipped.
				float height = td.GetInterpolatedHeight(fy, fx) + heightOffset;
				float slope = td.GetSteepness(fy, fx);

				Color c = new Color(0f, 0f, 0f, 0f);
				float a1 = Mathf.Clamp01((height - blendRange1.x) * rangeFactor1);

				if (height < blendRange0.x)
				{
					c.g = 1f;
				}
				else if (height < blendRange0.y)
				{
					float val = Mathf.Clamp01((height - blendRange0.x) * rangeFactor0);
					c.g = 1f - val;
					c.b = val;
				}
				else
				{
					float a2 = Mathf.Clamp01((slope - slopeRange.x) * slopeFactor);
					float invSlope = 1.0f - a2;

					if (height < blendRange1.x)
					{
						c.r = a2;
						c.b = invSlope;
					}
					else
					{
						c.r = a2;
						c.b = (1f - a1) * invSlope;
						c.a = a1 * invSlope;
					}
				}

				map[x, y, 0] = c.r;
				if (channels > 0) map[x, y, 1] = c.g;
				if (channels > 1) map[x, y, 2] = c.b;
				if (channels > 2) map[x, y, 3] = c.a;
			}
		}
		return map;
	}
}
