using UnityEngine;
using System.Collections;

namespace Tasharen
{
public class Turret : MonoBehaviour
{
	/// <summary>
	/// Gun barrel, if separate from the turret.
	/// </summary>

	public Transform gunBarrel;
	
	/// <summary>
	/// Point from which to fire.
	/// </summary>

	public Transform firingPoint;

	/// <summary>
	/// Laser pointer associated with the gun.
	/// </summary>

	public Renderer laserPointer;

	/// <summary>
	/// Prefab used to show a nozzle flash when firing.
	/// </summary>

	public GameObject nozzleFlashPrefab;

	/// <summary>
	/// Prefab used for explosion when hitting a target.
	/// </summary>

	public GameObject explosionPrefab;

	/// <summary>
	/// Radius of the final explosion.
	/// </summary>

	public float explosionRadius = 5f;

	/// <summary>
	/// Amount of force to apply at the heart of the explosion radiating outwards.
	/// </summary>

	public float explosionForce = 400f;

	/// <summary>
	/// Active firing range.
	/// </summary>
 
	public float rangeOfFire = 17f;

	/// <summary>
	/// How many shots are possible per second.
	/// </summary>
 
	public float secondsPerShot = 2f;

	/// <summary>
	/// How quickly the turret is able to turn per second in degrees.
	/// </summary>
	
	public float turnRate = 120f;
	
	/// <summary>
	/// Limit on how far the turret can turn in degrees (min pitch, max pitch, yaw range).
	/// </summary>
	
	public Vector3 turnRange = new Vector3(-10f, 45f, 360f);
	
	/// <summary>
	/// Whether to always show the aiming crosshairs (used for the player's turret).
	/// </summary>
	
	public bool alwaysShowCrosshairs = false;
	
	// Saved values
	Transform mTrans;
	Rigidbody mRb;
	Vehicle mVehicle;

	// Target position and calculated direction that the turret is trying to aim for
	Vector3 mTargetPos;
	Vector3 mTargetDir;
	Vector3 mAimingPos;

	// The last time the turret has fired
	float mLastFireTime = 0f;
	float mNextFireTime = 0f;
	
	// Whether the laser sight should be shown or not
	bool mShowLaserSight = false;

	/// <summary>
	/// Whether the laser sight will be shown.
	/// </summary>
	
	public bool showLaserSight
	{
		get
		{
			if (alwaysShowCrosshairs) return true;
			return mShowLaserSight && rechargePercent > 0.8f;
		}
		set
		{
			mShowLaserSight = value;
		}
	}
	
	/// <summary>
	/// Gets or sets the target direction.
	/// </summary>
	
	public Vector3 targetDir { get { return mTargetDir; } }
	
	/// <summary>
	/// Gets or sets the target position the turret will aim at.
	/// </summary>
	
	public Vector3 targetPos
	{
		get
		{
			return mTargetPos;
		}
		set
		{
			mTargetPos = value;
			Vector3 dir = (mTargetPos - gunBarrel.transform.position);
			float mag = dir.magnitude;
			if (mag > 0.001f) mTargetDir = dir * (1.0f / mag);
		}
	}

	/// <summary>
	/// Current position the turret will fire at if fired. Updated every frame.
	/// </summary>
	
	public Vector3 aimingPos { get { return mAimingPos; } }
	
	/// <summary>
	/// Turret recharge factor: 0-1 range.
	/// </summary>
	
	public float rechargePercent { get { return 1.0f - Mathf.Clamp01((mNextFireTime - Time.time)); } }
	
	/// <summary>
	/// Gets the remaining recharge cooldown in seconds.
	/// </summary>
	
	public float rechargeCooldown { get { return Mathf.Max(0f, mNextFireTime - Time.time); } }
	
	/// <summary>
	/// Gets a value indicating whether this turret is ready to fire.
	/// </summary>
	
	public bool readyToFire { get { return Time.time > mNextFireTime; } }

	/// <summary>
	/// Cache components.
	/// </summary>

	void Awake ()
	{
		mTrans = transform;
		mVehicle = Vehicle.FindInParents<Vehicle>(gameObject);
		mRb = mVehicle.rigidbody;
	}

	/// <summary>
	/// Keep the current direction by default.
	/// </summary>
	
	void Start()
	{
		mTargetDir = mTrans.rotation * Vector3.forward;
		mTargetPos = mTrans.position + mTargetDir * rangeOfFire;
		mAimingPos = mTargetPos;
	}

	/// <summary>
	/// Adjust the rotation based on the specified target direction.
	/// </summary>
	
	void Update()
	{
		if (!mVehicle.isActive)
		{
			if (laserPointer != null)
				laserPointer.enabled = false;
		}
		else
		{
			// Update the target position
			{
				RaycastHit hit;
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				if (Physics.Raycast(ray, out hit, 100f)) targetPos = hit.point;
				Vector3 unitPos = (mVehicle != null) ? mVehicle.transform.position : mTrans.position;
				unitPos.y = 0f;
			}

			// Rotate the gun barrel toward the 'targetPos', but only if it's not too close
			Vector3 tp = targetPos;
			tp.y = 0f;

			// Rotate the gun barrel
			if (gunBarrel == null)
			{
				RotateBarrelTowardTarget(mTrans, turnRange);
			}
			else
			{
				RotateBarrelTowardTarget(mTrans, new Vector3(0f, 0f, turnRange.z));
				RotateBarrelTowardTarget(gunBarrel, new Vector3(turnRange.x, turnRange.y, 0f));
			}

			// Scale the laser pointer so that it ends at the hit point
			if (laserPointer != null)
			{
				float targetAlpha = showLaserSight ? Mathf.Min(1f, rechargePercent * 1.3f) : 0f;
				float alpha = Mathf.Lerp(laserPointer.material.color.a, targetAlpha, Time.deltaTime * 3f);

				if (alpha < 0.001f)
				{
					laserPointer.enabled = false;
				}
				else
				{
					laserPointer.enabled = true;

					RaycastHit hit;
					Vector3 dir = firingPoint.rotation * Vector3.forward;
					float distance = rangeOfFire;

					// Raycast into what we would hit so that the laser pointer doesn't go through it
					if (Physics.Raycast(firingPoint.position, dir, out hit, distance))
						distance = hit.distance;

					mAimingPos = firingPoint.position + dir * distance;

					// Scale the laser pointer
					Transform t = laserPointer.transform;
					Vector3 scale = t.localScale;
					scale.z = distance;
					t.localScale = scale;
				}
			}

			// Fire the turret
			if (readyToFire && Input.GetMouseButtonDown(0)) Fire();
		}
	}
	
	/// <summary>
	/// Rotates the specified transform toward the target rotation using the provided limits (min pitch, max pitch, yaw range)
	/// </summary>
	
	void RotateBarrelTowardTarget (Transform trans, Vector3 limits)
	{
		// Target direction in local space
		Vector3 localTargetDir 	= (trans.parent != null) ? Quaternion.Inverse(trans.parent.rotation) * mTargetDir : mTargetDir;
		Vector3 localEuler  	= trans.localRotation.eulerAngles;
		Vector3 targetEuler 	= Quaternion.LookRotation(localTargetDir).eulerAngles;
		
		// How much we're able to turn this frame
		float turnPerFrame = Time.deltaTime * turnRate *  2f;
		
		// Calculate the difference between target and current euler angles
		Vector2 changeEuler = new Vector2(targetEuler.x - localEuler.x, targetEuler.y - localEuler.y);
		
		// Bring all angles within -180 to 180 range
		changeEuler.x = WrapAngle(changeEuler.x);
		changeEuler.y = WrapAngle(changeEuler.y);
		
		// Limit the change by the calculated per-frame maximum
		changeEuler.x = Mathf.Clamp(changeEuler.x, -turnPerFrame, turnPerFrame);
		changeEuler.y = Mathf.Clamp(changeEuler.y, -turnPerFrame, turnPerFrame);
		
		// Adjust the local euler angles by the change
		localEuler.x = WrapAngle(localEuler.x + WrapAngle(changeEuler.x));
		localEuler.y = WrapAngle(localEuler.y + WrapAngle(changeEuler.y));
		
		// Ensure that all angles fall within the specified limits
		localEuler.x = Mathf.Clamp(localEuler.x, -limits.y, -limits.x);
		localEuler.y = Mathf.Clamp(localEuler.y, -limits.z,  limits.z);
		
		trans.localRotation = Quaternion.Euler(localEuler);
	}

	/// <summary>
	/// Wraps the angle, ensuring that it's always in the -360 to 360 range.
	/// </summary>

	static public float WrapAngle (float a)
	{
		while (a < -180.0f) a += 360.0f;
		while (a > 180.0f) a -= 360.0f;
		return a;
	}
	
	/// <summary>
	/// Fires this turret.
	/// </summary>
	
	public bool Fire()
	{
		if (!readyToFire) return false;
		
		mLastFireTime = Time.time;
		mNextFireTime = mLastFireTime + secondsPerShot;
		
		RaycastHit hit;
		Vector3 dir = gunBarrel.rotation * Vector3.forward;
		Vector3 pos;
		
		if (Physics.Raycast(firingPoint.position, dir, out hit, rangeOfFire))
		{
			pos = hit.point + hit.normal * 0.5f;
		}
		else
		{
			pos = firingPoint.position + dir * rangeOfFire;
		}

		FireAt(pos, dir);
		return true;
	}

	void FireAt (Vector3 pos, Vector3 dir)
	{
		// Prefab used to create a puff of smoke when firing the weapon
		if (nozzleFlashPrefab != null && firingPoint != null)
		{
			GameObject go = (GameObject)Instantiate(nozzleFlashPrefab, firingPoint.position, Quaternion.LookRotation(dir));
			Destroy(go, 2f);
		}

		// Apply the knockback force to the turret that discharged the weapon
		if (mRb != null)
		{
			mRb.AddForceAtPosition(dir * (-50f * mRb.mass), firingPoint.position);
		}

		// Fire the weapon
		StartCoroutine(ExplodeShell(pos));
	}

	/// <summary>
	/// Delayed explosion.
	/// </summary>

	IEnumerator ExplodeShell (Vector3 pos)
	{
		float sec = (pos - mTrans.position).magnitude * 0.01f;
		yield return new WaitForSeconds(sec);

		if (explosionPrefab != null)
		{
			GameObject go = (GameObject)Instantiate(explosionPrefab, pos, Quaternion.identity);
			Destroy(go, 4f);
		}
		mVehicle.AddExplosionForce(pos, 0f, explosionForce, explosionRadius);
	}
}
}
