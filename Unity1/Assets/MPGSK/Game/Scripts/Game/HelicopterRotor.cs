using UnityEngine;

namespace Tasharen
{
/// <summary>
/// Script used to animate a helicopter rotor when the helicopter is flying.
/// </summary>

public class HelicopterRotor : MonoBehaviour
{
	public enum Axis
	{
		X,
		Y,
		Z,
	}

	/// <summary>
	/// Axis to rotate around.
	/// </summary>

	public Axis axis = Axis.Y;

	/// <summary>
	/// Rotations per second in degrees.
	/// </summary>

	public float degreesPerSecond = 2000f;

	Vehicle mVehicle;
	Transform mTrans;
	float mRot = 0f;
	float mSpeed = 0f;
	Vector3 mOriginal;

	void Start ()
	{
		mTrans = transform;
		mOriginal = mTrans.localEulerAngles;
		mVehicle = Vehicle.FindInParents<Vehicle>(gameObject);
	}

	void Update ()
	{
		float delta = Time.deltaTime;
		float speed = Mathf.Abs(mVehicle.engineSpeed);
		mSpeed = Mathf.Lerp(mSpeed, mVehicle.isActive ? Mathf.Lerp(0.85f, 1f, speed) : 0f, 2f * delta);
		mRot += degreesPerSecond * delta * mSpeed;
		while (mRot > 360f) mRot -= 360f;

		if (axis == Axis.Y)
		{
			mTrans.localRotation = Quaternion.Euler(mOriginal.x, mRot, mOriginal.z);
		}
		else if (axis == Axis.Z)
		{
			mTrans.localRotation = Quaternion.Euler(mOriginal.x, mOriginal.y, mRot);
		}
		else
		{
			mTrans.localRotation = Quaternion.Euler(mRot, mOriginal.y, mOriginal.z);
		}
	}
}
}
