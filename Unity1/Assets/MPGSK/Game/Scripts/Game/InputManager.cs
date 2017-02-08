using UnityEngine;

namespace Tasharen
{
/// <summary>
/// This manager class is an intermediary input class. It takes input from sources, and converts it to thrust vectors.
/// </summary>

public class InputManager : MonoBehaviour
{
	public string joystickLeftHorizontal	= "Horizontal";
	public string joystickLeftVertical		= "Vertical";
	public string joystickRightHorizontal	= "RX";
	public string joystickRightVertical		= "RY";

	/// <summary>
	/// Left thumb stick should control the character.
	/// </summary>

	static public Vector2 leftThumb = Vector2.zero;

	/// <summary>
	/// Right thumb stick should control the camera.
	/// </summary>

	static public Vector2 rightThumb = Vector2.zero;

	// Used to disable checking for custom axes if they haven't been set up
	bool mHasSetupAxes = true;

	/// <summary>
	/// Process input from different axes.
	/// </summary>

	void Update()
	{
		leftThumb.x = Input.GetAxis(joystickLeftHorizontal);
		leftThumb.y = Input.GetAxis(joystickLeftVertical);
		AdjustAxis(ref leftThumb);

		if (mHasSetupAxes)
		{
			try
			{
				rightThumb.x = Input.GetAxis(joystickRightHorizontal);
				rightThumb.y = Input.GetAxis(joystickRightVertical);
				AdjustAxis(ref rightThumb);
			}
			catch (System.Exception ex)
			{
				mHasSetupAxes = false;
				Debug.LogWarning(ex.Message, this);
			}
		}
		else
		{
			rightThumb.x = 0f;
			rightThumb.y = 0f;
		}
	}

	/// <summary>
	/// Analog joysticks and 360 controllers are limited to a magnitude of 1, while keyboards can reach
	/// magnitude of 1.41 (1 on the X, 1 on the Y). It's a good idea to unify the two.
	/// </summary>

	void AdjustAxis (ref Vector2 v)
	{
		v.x = Mathf.Clamp(v.x, -0.707f, 0.707f) * 1.41f;
		v.y = Mathf.Clamp(v.y, -0.707f, 0.707f) * 1.41f;
	}
}
}
