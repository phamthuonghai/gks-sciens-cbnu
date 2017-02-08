using UnityEngine;

namespace Tasharen
{
/// <summary>
/// Set the camera's target making the camera follow this object.
/// </summary>

public class CameraTarget : MonoBehaviour
{
	static public CameraTarget target;

	/// <summary>
	/// Height offset to apply to the camera following the target.
	/// </summary>

	public float heightOffset = 0f;

	void OnEnable ()
	{
		target = this;
	}

	void OnDisable ()
	{
		if (target == this)
			target = null;
	}
}
}
