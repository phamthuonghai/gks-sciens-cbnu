using UnityEngine;

/// <summary>
/// This script creates the GUI used to change terrain properties at run time.
/// </summary>

public class ExampleTerrainGUI : MonoBehaviour
{
	public GameObject terrainRoot;
	public bool offset = false;

	CreateTerrain[] mTerrains;
	CreateTerrain mTerrain;
	int mStage = 0;

	void Awake ()
	{
		mTerrains = terrainRoot.GetComponentsInChildren<CreateTerrain>(true);
	}

	void Update ()
	{
		if (mStage == 2 && mTerrain != null)
		{
			if (!mTerrain.gameObject.activeInHierarchy)
				mTerrain.gameObject.SetActive(true);
			mTerrain.Generate(true);
			mStage = 0;
		}
	}

	void OnGUI ()
	{
		if (offset) GUILayout.Space(30f);

		if (mStage != 0)
		{
			GUILayout.BeginVertical("box");
			GUILayout.BeginHorizontal();
			GUILayout.Label("<b>Please wait... This may take a few seconds.</b>", "button");
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
			mStage = 2;
			return;
		}

		GUILayout.BeginVertical("box");
		GUILayout.BeginHorizontal();

		CreateTerrain selection = null;

		for (int i = 0; i < mTerrains.Length; ++i)
		{
			CreateTerrain t = mTerrains[i];
			bool highlight = t.gameObject.activeInHierarchy;
			if (highlight) mTerrain = t;

			GUI.backgroundColor = highlight ? Color.green : Color.gray;

			if (GUILayout.Toggle(highlight, "<b>" + t.name + "</b>", "Button") && !highlight)
				selection = t;
			
			GUI.backgroundColor = Color.white;
		}
		GUILayout.EndHorizontal();

		if (mTerrain != selection && selection != null)
		{
			mTerrain.gameObject.SetActive(false);
			mTerrain = selection;
			mStage = 1;
		}

		if (mTerrain != null)
		{
			bool regenerate = false;

			GUILayout.BeginHorizontal();
			GUILayout.Label("<b>Control Density 1</b>", GUILayout.Width(120f));
			mTerrain.noiseDensity1 = Mathf.RoundToInt(GUILayout.HorizontalSlider(mTerrain.noiseDensity1, 1f, 16f));
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Label("<b>Control Density 2</b>", GUILayout.Width(120f));
			mTerrain.noiseDensity2 = Mathf.RoundToInt(GUILayout.HorizontalSlider(mTerrain.noiseDensity2, 1f, 16f));
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Label("<b>Control Density 3</b>", GUILayout.Width(120f));
			mTerrain.controlDensity = Mathf.RoundToInt(GUILayout.HorizontalSlider(mTerrain.controlDensity, 1f, 16f));
			GUILayout.EndHorizontal();

			if (GUILayout.Button("<b>Regenerate</b>") || regenerate)
				mStage = 1;
		}
		GUILayout.EndVertical();
		
		// Prevent mouse input
		Rect rect = GUILayoutUtility.GetLastRect();
		Vector2 pos = Input.mousePosition;
		pos.y = Screen.height - pos.y;
		if (rect.Contains(pos))
			ExampleCamera.allowMouseInput = false;
	}
}
