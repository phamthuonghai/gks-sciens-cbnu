using UnityEngine;

/// <summary>
/// This script contains a 3rd person camera used to allow movement around the generated scene.
/// </summary>

public class ExampleCamera : MonoBehaviour
{
	// Used to disallow input over OnGUI areas
	static public bool allowMouseInput = true;

	public Transform pivotTransform;
	public Transform cameraTransform;

	public float minHeight = 6.5f;
	public float maxHeight = 15f;

	float mRot = 0f;
	Quaternion mTargetRot;
	Vector3 mCamOffset;
	Vector3 mTargetPos;
	Vector2 mLastPos;

	void Start ()
	{
		mLastPos = Input.mousePosition;
		mRot = pivotTransform.localRotation.eulerAngles.y;
		mTargetPos = pivotTransform.position;
		mTargetRot = Quaternion.Euler(0f, mRot, 0f);
		mCamOffset = cameraTransform.localPosition;
		mCamOffset.y = Mathf.Clamp(mCamOffset.y, minHeight, maxHeight);
	}

	void Update ()
	{
		if (allowMouseInput)
		{
			Vector2 pos = Input.mousePosition;
			Vector2 delta = pos - mLastPos;
			mLastPos = pos;

			if (Input.GetKey(KeyCode.Mouse0))
			{
				Vector3 dir = cameraTransform.rotation * Vector3.forward;
				dir.y = 0f;
				dir.Normalize();

				float f = mCamOffset.y * 0.002f;
				mTargetPos -= Quaternion.LookRotation(dir) * new Vector3(delta.x * f, 0f, delta.y * f);
			}
			else if (Input.GetKey(KeyCode.Mouse1))
			{
				mRot += delta.x * 0.2f;
				mTargetRot = Quaternion.Euler(0f, mRot, 0f);
			}

			float scroll = Input.GetAxis("Mouse ScrollWheel");

			if (scroll != 0f)
			{
				mCamOffset.y -= scroll * 10f;
				mCamOffset.y = Mathf.Clamp(mCamOffset.y, minHeight, maxHeight);
			}

			pivotTransform.position = Vector3.Lerp(pivotTransform.position, mTargetPos, 12f * Time.deltaTime);
			pivotTransform.localRotation = Quaternion.Slerp(pivotTransform.localRotation, mTargetRot, 20f * Time.deltaTime);
			cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, mCamOffset, 8f * Time.deltaTime);
			cameraTransform.LookAt(pivotTransform);
		}
	}

	void LateUpdate ()
	{
		allowMouseInput = true;
	}
}
