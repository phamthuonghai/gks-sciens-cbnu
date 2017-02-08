using UnityEngine;

namespace Tasharen
{
/// <summary>
/// Helicopter type vehicle.
/// </summary>

[RequireComponent(typeof(Rigidbody))]
public class Helicopter : Vehicle
{
	/// <summary>
	/// Prefab instantiated when the vehicle gets destroyed.
	/// </summary>

	public GameObject explosionPrefab;

	/// <summary>
	/// How quickly the vehicle is able to turn.
	/// </summary>

	public float turnRate = 3f;

	/// <summary>
	/// Higher speed penalizes the turn rate down to this percentage.
	/// </summary>

	public float turnRatePenalty = 0.75f;

	/// <summary>
	/// Used to drive the helicopter forward.
	/// </summary>

	public float forwardForce = 10f;

	/// <summary>
	/// How much the helicopter will tilt when moving forward or back.
	/// </summary>

	public float forwardTilt = 20f;

	/// <summary>
	/// How much the helicopter will tilt when moving left or right.
	/// </summary>

	public float sidewaysTilt = 30f;

	/// <summary>
	/// This is the "minimum" height used for liftoff calculations.
	/// </summary>

	public float waterHeight = 0f;

	/// <summary>
	/// Layer mask used to determine what the helicopter can lift off of.
	/// In most cases it's a good idea to limit it only to the terrain for performance reasons.
	/// </summary>

	public LayerMask hoverMask = 1;

	RigidbodyConstraints mConstraints = RigidbodyConstraints.None;
	float mTurn = 0f;
	float mDrag = 0f;
	float mAngDrag = 0f;
	Vector2 mBodyTilt = Vector2.zero;
	float mPitch = 0f;

	/// <summary>
	/// Current engine speed, ranging from -1 to 1.
	/// </summary>

	public override float engineSpeed { get { return Mathf.Max(0f, mMove.y); } }

	/// <summary>
	/// Disable the heavy drag and the constraints, allowing the rigidbody to free-fall.
	/// </summary>

	protected override void OnDeath ()
	{
		mConstraints = mRb.constraints;
		mDrag = mRb.drag;
		mAngDrag = mRb.angularDrag;
		mRb.constraints = RigidbodyConstraints.None;
		mRb.drag = 0.1f;
		mRb.angularDrag = 0.1f;
	}

	/// <summary>
	/// Restore the drag and the constraints.
	/// </summary>

	protected override void OnRespawn ()
	{
		mRb.constraints = mConstraints;
		mRb.drag = mDrag;
		mRb.angularDrag = mAngDrag;
	}

	/// <summary>
	/// Tilt the helicopter based on input.
	/// </summary>

	protected override void OnUpdate()
	{
		if (isActive)
		{
			Vector3 euler = mTrans.localEulerAngles;
			float delta = Time.deltaTime * 2f;
			float x = Mathf.Clamp(mMove.x + mTilt.x, -1f, 1f);
			float y = Mathf.Clamp(mMove.y + mTilt.y, -1f, 1f);
			mBodyTilt.x = Mathf.Lerp(mBodyTilt.x, x * sidewaysTilt, delta);
			mBodyTilt.y = Mathf.Lerp(mBodyTilt.y, y * forwardTilt, delta);
			mTrans.localRotation = Quaternion.Euler(mBodyTilt.y, euler.y, -mBodyTilt.x);
		}

		// Adjust the pitch of the audio source
		if (engineSound != null)
		{
			float delta = Time.fixedDeltaTime * 5.0f;
			float pitch = 0.8f + Mathf.Abs(engineSpeed) * 0.2f;
			mPitch = Mathf.Lerp(mPitch, pitch, delta);
			engineSound.pitch = mPitch;

			// If the audio source is not playing, start playing it (gradually fading it in)
			if (!engineSound.isPlaying)
			{
				engineSound.loop = true;
				engineSound.volume = 0f;
				engineSound.Play();
			}

			// Adjust the volume
			engineSound.volume = Mathf.Lerp(engineSound.volume, isActive ? 1f : 0f, delta);
		}

		if (isActive && Input.GetKeyDown(KeyCode.Space)) OnTriggerEnter();
	}

	/// <summary>
	/// Apply the directional physics.
	/// </summary>

	void FixedUpdate ()
	{
		if (mRb.isKinematic || !isActive) return;

		// Turning effectiveness gets reduced as the speed gets higher
		float turn = turnRate * Mathf.Lerp(mMove.x, mMove.x * turnRatePenalty, Mathf.Max(0f, mMove.y));

		// Linear interpolation for smoother results
		mTurn = Mathf.Lerp(mTurn, turn, Time.fixedDeltaTime * 4f);
		mRb.AddRelativeTorque(0f, mTurn * mRb.mass * 10f, 0f);

		RaycastHit hit;
		const float effectiveHeight = 12f;
		bool closeToGround = false;
		float distance = 0f;
		
		Plane waterPlane = new Plane(Vector3.up, waterHeight);
		Ray ray = new Ray(mTrans.position, mTrans.rotation * Vector3.down);

		// Determine the point directly underneath the helicopter's rotor
		if (Physics.Raycast(ray, out hit, effectiveHeight, hoverMask))
		{
			closeToGround = true;
			distance = waterPlane.Raycast(ray, out distance) ? Mathf.Min(hit.distance, distance) : hit.distance;
		}
		else closeToGround = waterPlane.Raycast(ray, out distance);

		// Close to the ground?
		if (closeToGround)
		{
			float distanceFactor = Mathf.Clamp01(distance / effectiveHeight);
			
			// The amount of force applied vertically depends on how close the helicopter is to the ground.
			// A real-life helicopter would take air density into account instead (ie: height), but for
			// this example using distance to the ground gives far more pleasing results.
			float upwardsForce = 1f - distanceFactor;
			upwardsForce *= upwardsForce;
			upwardsForce = Mathf.Lerp(10f, 80f, upwardsForce);
			upwardsForce *= mRb.mass;

			// Tilting requires more force in order to maintain the same height
			float a = Mathf.Deg2Rad * Mathf.Abs(mBodyTilt.x);
			float b = Mathf.Deg2Rad * Mathf.Abs(mBodyTilt.y);
			upwardsForce *= 1f + Mathf.Sin(Mathf.Max(a, b));

			// Add an upwards force, keeping the helicopter afloat
			mRb.AddRelativeForce(Vector3.up * upwardsForce);

			// Change the dust emitter's color based on height and on what's underneath
			if (dustEmitter != null)
			{
				dustEmitter.enableEmission = true;

				Vector3 pos = ray.GetPoint(distance);
				mEmitterTrans.position = pos;

				// Emitter's alpha depends on how close the helicopter is to the ground
				Color c = mEmitterColor;
				c.a = Mathf.Lerp(c.a, c.a * 0.25f, distanceFactor);

				// At water level -- make the particles white (to simulate foam on water instead of dust)
				if (pos.y < waterHeight + 0.001f)
				{
					c.r = 1f;
					c.g = 1f;
					c.b = 1f;
				}
				dustEmitter.startColor = c;
			}
		}
		else if (dustEmitter != null)
		{
			dustEmitter.enableEmission = false;
		}

		// Drive the helicopter forward
		mRb.AddRelativeForce(Vector3.forward * Mathf.Max(0f, mMove.y * forwardForce * mRb.mass));
	}

	/// <summary>
	/// When a rotor hits something, the helicopter gets instantly destroyed.
	/// </summary>

	void OnTriggerEnter () { if (isActive) Explode(); }

	/// <summary>
	/// Explode the vehicle.
	/// </summary>

	protected override void Explode ()
	{
		base.Explode();
		GameObject go = (GameObject)Instantiate(explosionPrefab, mTrans.position, Quaternion.identity);
		Destroy(go, 4f);
	}
}
}
