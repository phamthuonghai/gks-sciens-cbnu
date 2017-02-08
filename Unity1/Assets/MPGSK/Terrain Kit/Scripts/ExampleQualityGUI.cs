using UnityEngine;

/// <summary>
/// This example script adds the ability to change some basic run-time quality settings via a menu in the top right corner.
/// </summary>

public class ExampleQualityGUI : MonoBehaviour
{
	public GameObject water;
	public GameObject treeRoot;
	public Light sunlight;

	Renderer[] mRenderers;
	int mShadows = 2;
	int mFrames = 0;
	int mFPS = 0;
	float mNext = 0f;

	void Update ()
	{
		++mFrames;
		mNext += Time.deltaTime;

		if (mNext > 0.5f)
		{
			mFPS = (mFrames << 1);
			mFrames = 0;
			mNext = 0f;
		}
	}

	void OnGUI ()
	{
		string buttonText;
		Rect rect = new Rect(Screen.width - 120f, 0f, 120f, 30f);

		if (water != null)
		{
			buttonText = water.activeInHierarchy ? "<b>Water: ON</b>" : "<b>Water: OFF</b>";
			GUI.backgroundColor = water.activeInHierarchy ? Color.green : Color.white;
			if (GUI.Button(rect, buttonText)) water.SetActive(!water.activeInHierarchy);
			rect.x -= rect.width;
		}

		buttonText = "<b>Shadows: OFF</b>";
		if (mShadows == 1) buttonText = "<b>Shadows: LOW</b>";
		else if (mShadows == 2) buttonText = "<b>Shadows: MID</b>";
		else if (mShadows == 3) buttonText = "<b>Shadows: HIGH</b>";

		GUI.backgroundColor = (mShadows == 0) ? Color.white : Color.green;
		
		if (GUI.Button(rect, buttonText))
		{
			if (++mShadows > 3) mShadows = 0;
			
			if (mShadows == 0) sunlight.shadows = LightShadows.None;
			else if (mShadows == 3) sunlight.shadows = LightShadows.Soft;
			else sunlight.shadows = LightShadows.Hard;

			if (mShadows != 0)
			{
				if (mRenderers == null || mRenderers[0] == null)
					mRenderers = treeRoot.GetComponentsInChildren<Renderer>();

				foreach (Renderer ren in mRenderers)
					ren.castShadows = (mShadows > 1);
			}
		}

		GUI.backgroundColor = Color.white;

		GUI.Box(new Rect(Screen.width * 0.5f - 40f, 0f, 80f, 20f), "<b>FPS: " + mFPS + "</b>");

		Vector2 pos = Input.mousePosition;
		pos.y = Screen.height - pos.y;
		
		if (pos.y < 30f) ExampleCamera.allowMouseInput = false;
	}
}
