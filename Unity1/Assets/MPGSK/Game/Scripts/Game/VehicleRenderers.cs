using UnityEngine;

namespace Tasharen
{
public class VehicleRenderers : MonoBehaviour
{
	Vehicle mVehicle;
	Transform mTrans;
	Vector3 mPos;
	Quaternion mRot;
	float mAlpha = 0f;

	void Start ()
	{
		mTrans = transform;
		mPos = mTrans.position;
		mRot = mTrans.rotation;
		mVehicle = Vehicle.FindInParents<Vehicle>(gameObject);
	}

	void Update ()
	{
		float factor = Time.deltaTime * 15f;
		mAlpha = Mathf.Lerp(mAlpha, mVehicle.isActive ? 1f : 0f, factor);
		
		if (mAlpha > 0.01f)
		{
			mPos = Vector3.Lerp(mPos, mTrans.parent.position, factor);
			mRot = Quaternion.Slerp(mRot, mTrans.parent.rotation, factor);
			
			mTrans.position = mPos;
			mTrans.rotation = mRot;
			
			mTrans.localPosition = mTrans.localPosition * mAlpha;
			mTrans.localRotation = Quaternion.Slerp(Quaternion.identity, mTrans.localRotation, mAlpha);
		}
		else
		{
			mTrans.localPosition = Vector3.zero;
			mTrans.localRotation = Quaternion.identity;
		}
	}
}
}
