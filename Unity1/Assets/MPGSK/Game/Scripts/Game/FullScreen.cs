using UnityEngine;

namespace Tasharen
{
/// <summary>
/// Toggles between full screen and not full screen
/// </summary>

public class FullScreen : MonoBehaviour
{
	void Start ()
	{
		if (PlayerPrefs.GetInt("FS", 0) == 1) Set(true);
	}

	void Update ()
	{
		if (Input.GetKeyDown(KeyCode.F2))
			Set(!Screen.fullScreen);
	}

	void Set (bool full)
	{
		if (Screen.fullScreen == full) return;

		if (full)
		{
			Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
			PlayerPrefs.SetInt("FS", 1);
		}
		else
		{
			Screen.SetResolution(1280, 720, false);
			PlayerPrefs.SetInt("FS", 0);
		}
	}
}
}
