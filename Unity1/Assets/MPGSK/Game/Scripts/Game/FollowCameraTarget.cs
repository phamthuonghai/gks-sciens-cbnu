using UnityEngine;

namespace Tasharen
{
/// <summary>
/// Simple script that will follow the camera's target. Meant to be placed on the camera.
/// </summary>

public class FollowCameraTarget : MonoBehaviour
{
	[Range(1, 20)]
	public float positionStrength = 5f;

	[Range(1, 20)]
	public float rotationStrength = 5f;

	Transform mTrans;
	Vector3 mPos = Vector3.zero;
	Vector3 mDir = Vector3.forward;

	void Start ()
	{
		mTrans = transform;
	}

	void Update ()
	{
		if (CameraTarget.target != null)
		{
			mPos = CameraTarget.target.transform.position;
			mPos.y += CameraTarget.target.heightOffset;
			Vector3 dir = CameraTarget.target.transform.rotation * Vector3.forward;
			dir.y = 0f;
			float mag = dir.magnitude;
			if (mag > 0f) mDir = dir / mag;
		}

		float delta = Time.deltaTime;
		mTrans.position = Vector3.Lerp(mTrans.position, mPos, positionStrength * delta);
		mTrans.rotation = Quaternion.Slerp(mTrans.rotation, Quaternion.LookRotation(mDir), rotationStrength * delta);
	}
}
}
