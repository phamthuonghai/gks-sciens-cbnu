using UnityEngine;

namespace Tasharen
{
/// <summary>
/// Tank extends the Vehicle script, adding specific physics-based movement logic.
/// </summary>

[RequireComponent(typeof(Rigidbody))]
public class Tank : Vehicle
{
	class WheelData
	{
		public Transform trans;
		public GameObject go;
		public WheelCollider col;
		public Vector3 startPos;
		public float rotation = 0f;
		public float heightOffset = 0f;
		public float turnAngle = 0f;
		public float currentSteer = 0f;
		public float speedDifference = 0f;
		public float currentSuspension = 0f;
	};

	/// <summary>
	/// Prefab instantiated when the vehicle gets destroyed on land.
	/// </summary>

	public GameObject landExplosion;

	/// <summary>
	/// Prefab instantiated when the vehicle gets destroyed in the water.
	/// </summary>

	public GameObject waterExplosion;

	/// <summary>
	/// If the vehicle goes below this value, it will become disabled and a water explosion will appear in the area.
	/// If no water explosion is specified, nothing will happen.
	/// </summary>

	public float drownHeight = -1.25f;

	/// <summary>
	/// How quickly the vehicle is able to turn. Only used if "turn using wheels" is off.
	/// </summary>

	public float turnRate = 2f;

	/// <summary>
	/// Higher speed penalizes the turn rate down to this percentage.
	/// </summary>

	public float turnRatePenalty = 0.7f;

	/// <summary>
	/// Whether the vehicle will use wheels to turn or not. A car would be using wheels, while a tank would not be.
	/// </summary>

	public bool turnUsingWheels = false;

	/// <summary>
	/// When driving in reverse, should the left and right be flipped?
	/// </summary>

	public bool intuitiveReverse = true;

	/// <summary>
	/// Torque applied when the vehicle is standing still. Should be a high value for a tank, and a low value for a car.
	/// This is used to allow the tank to stop at an incline when moving instead of sliding back down.
	/// </summary>

	public float engineTorque = 2000f;

	/// <summary>
	/// Torque
	/// </summary>

	public float brakeTorque = 1000f;

	/// <summary>
	/// Friction applied by the engine. Works with brake torque to determine how the vehicle moves.
	/// </summary>

	public float engineFriction = 0.1f;

	/// <summary>
	/// Engine's maximum rotations per minute. Note that larger wheels allow for less RPM needed in order to move at the same speed.
	/// </summary>

	public float maxRPM = 200f;

	/// <summary>
	/// Active suspension adjusts to the height of the terrain underneath.
	/// </summary>

	public bool activeSuspension = true;

	/// <summary>
	/// Adjustment distance when active suspension is used.
	/// </summary>

	public float suspensionDistance = 0.15f;

	/// <summary>
	/// Radius of the vehicle's wheels. Note that the wider the wheels, the less RPM is needed to accelerate the vehicle!
	/// </summary>

	public float wheelRadius = 0.6f;

	/// <summary>
	/// Weight of each wheel.
	/// </summary>

	public float wheelWeight = 60f;

	/// <summary>
	/// How far the wheels are able to turn.
	/// </summary>

	public float wheelTurnAngle = 10f;

	/// <summary>
	/// References to where the vehicle's wheels should be located.
	/// </summary>

	public Transform wheelFR;
	public Transform wheelFL;
	public Transform wheelBR;
	public Transform wheelBL;

	/// <summary>
	/// Center of mass, applied to the rigidbody after the wheels get added.
	/// </summary>

	public Vector3 centerOfMass = new Vector3(0.0f, -0.5f, 0.0f);
	
	// Additional properties, hidden on purpose
	[HideInInspector] public float wheelResponsiveness	= 10f;
	[HideInInspector] public float torqueResponsiveness = 10f;
	[HideInInspector] public float forwardTraction 		= 0.9f;
	[HideInInspector] public float sidewaysTraction 	= 0.9f;
	[HideInInspector] public float springs 				= 900.0f;
	[HideInInspector] public float dampers 				= 45f;

	WheelData[] mWheels = new WheelData[4];
	Rigidbody mRigidbody;
	Vector3 mCenterOfMass;
	float mWheelSpeed = 0f;
	float mTurn = 0f;
	float mLastGround = 0f;
	bool mIsGrounded = false;
	float mPitch = 0f;
	float mAlpha = 0f;

	/// <summary>
	/// Current wheel turning speed, ranging from -1 to 1 (though generally less due to breaking friction).
	/// </summary>

	public override float engineSpeed { get { return mWheelSpeed; } }

	/// <summary>
	/// Whether the vehicle is currently considered to be touching the ground.
	/// </summary>

	public bool isGrounded { get { return mIsGrounded || mLastGround + 0.15f > Time.time; } }
	
	/// <summary>
	/// Helper function that creates a wheel at the specified transform.
	/// </summary>
	
	WheelData CreateWheel (Transform wheel, float turnAngle)
	{
		if (wheel == null) return null;

		GameObject go 	= new GameObject("WheelCollider");
		Transform t 	= go.transform;
		t.parent 		= transform;
		t.position 		= wheel.position;
		t.localRotation = wheel.localRotation;

		WheelCollider col = go.AddComponent<WheelCollider>();
		col.motorTorque = 0.0f;
		
		WheelData wd 	= new WheelData();
		wd.trans 		= wheel;
		wd.go 			= go;
		wd.col 			= col;
		wd.startPos 	= wheel.localPosition;
		wd.turnAngle	= turnAngle;

		return wd;
	}
	
	/// <summary>
	/// Helper function that applies initial properties to the specified wheel.
	/// </summary>
	
	void ApplyWheelProperties (WheelData w)
	{
		WheelCollider col 		= w.col;
		JointSpring js 			= col.suspensionSpring;
		js.spring 				= springs;
		js.damper 				= dampers;
		
		col.suspensionDistance 	= suspensionDistance;
		col.suspensionSpring 	= js;
		col.radius 				= wheelRadius;
		col.mass 				= wheelWeight;

		WheelFrictionCurve fc 	= col.forwardFriction;
		fc.asymptoteValue 		= 5000.0f;
		fc.extremumSlip 		= 2.0f;
		fc.asymptoteSlip 		= 20.0f;
		fc.stiffness 			= forwardTraction;
		col.forwardFriction 	= fc;

		fc 						= col.sidewaysFriction;
		fc.asymptoteValue 		= 7500.0f;
		fc.asymptoteSlip 		= 2.0f;
		fc.stiffness 			= sidewaysTraction;

		col.sidewaysFriction 	= fc;
	}

	/// <summary>
	/// Create the wheels and adjust the center of mass.
	/// </summary>
	
	void Start()
	{
		mRigidbody = rigidbody;
		
		if (wheelFR) mWheels[0] = CreateWheel(wheelFR,  wheelTurnAngle);
		if (wheelFL) mWheels[1] = CreateWheel(wheelFL,  wheelTurnAngle);
		if (wheelBR) mWheels[2] = CreateWheel(wheelBR, -wheelTurnAngle);
		if (wheelBL) mWheels[3] = CreateWheel(wheelBL, -wheelTurnAngle);
		
		foreach (WheelData w in mWheels) ApplyWheelProperties(w);
		
		// Calculate and remember the center of mass *after* setting the wheel colliders.
		// Note that at this point the center of mass takes wheels into account.
		mCenterOfMass = mRigidbody.centerOfMass + centerOfMass;
	}

	/// <summary>
	/// Update the engine sound based on how fast the tank is moving.
	/// </summary>

	protected override void OnUpdate ()
	{
		float movement = isActive ? Mathf.Abs(engineSpeed) : 0f;

		// Adjust the pitch of the audio source
		if (engineSound != null)
		{
			float delta = Time.deltaTime * 5.0f;
			float pitch = Mathf.Min(0.5f + Mathf.Abs(engineSpeed) * 0.5f, 1.0f);
			mPitch = Mathf.Lerp(mPitch, pitch, delta);
			engineSound.pitch = mPitch;

			// If the audio source is not playing, start playing it (gradually fading it in)
			if (movement > 0.05f && !engineSound.isPlaying)
			{
				engineSound.loop = true;
				engineSound.volume = 0f;
				engineSound.Play();
			}

			// Adjust the volume
			engineSound.volume = Mathf.Lerp(engineSound.volume, isActive ? 1f : 0f, delta);
		}

		// Adjust the dust emitter's color
		if (dustEmitter != null)
		{
			if (isActive)
			{
				dustEmitter.enableEmission = isActive;

				Color c = mEmitterColor;

				if (mTrans.position.y < 0f)
				{
					c.r = 1f;
					c.g = 1f;
					c.b = 1f;
				}

				mAlpha = Mathf.Lerp(mAlpha, Mathf.Abs(mMove.y), Time.deltaTime * 3f);
				c.a *= mAlpha;
				dustEmitter.startColor = c;
			}
			else dustEmitter.enableEmission = false;
		}

		if (isActive && Input.GetKeyDown(KeyCode.Space)) Explode();
	}

	/// <summary>
	/// Explode the vehicle.
	/// </summary>

	protected override void Explode ()
	{
		base.Explode();
		GameObject go = (GameObject)Instantiate(landExplosion, mTrans.position, Quaternion.identity);
		Destroy(go, 4f);
	}
	
	/// <summary>
	/// Drive the tank using physics.
	/// </summary>

	void FixedUpdate()
	{
		if (mRigidbody.isKinematic) return;
		
		float moveMag = Mathf.Clamp01(mMove.magnitude);
		float brakeFactor = moveMag;
		brakeFactor *= brakeFactor;
		brakeFactor *= brakeFactor;

		mWheelSpeed = 0f;

		// Run through all wheels and apply the torque
		foreach (WheelData wd in mWheels)
		{
			// The faster the movement, the less torque is needed to keep going.
			// Maximum torque should be applied if the movement is in the opposite direction of 'drive'.

			float currentMove = Mathf.Clamp(mMove.y, -1f, 1f);
			float currentSpeed = Mathf.Clamp(wd.col.rpm / maxRPM, -1f, 1f);
			float speedDifference = Mathf.Clamp(currentMove - currentSpeed, -1f, 1f);

			// Interpolation for smoother results
			wd.speedDifference = Mathf.Lerp(wd.speedDifference, speedDifference,
				Mathf.Clamp01(Time.fixedDeltaTime * torqueResponsiveness));

			// Apply the torque
			wd.col.motorTorque = wd.speedDifference * engineTorque;
			wd.col.brakeTorque = (1.0f - (1.0f - engineFriction) * brakeFactor) * brakeTorque;
			mWheelSpeed += currentSpeed;
		}

		// Vehicle's current driving speed
		mWheelSpeed /= mWheels.Length;

		// Turning effectiveness gets reduced as the speed gets higher
		float st = Mathf.Lerp(mMove.x, mMove.x * turnRatePenalty, Mathf.Abs(mMove.y));

		// Run through all wheels and rotate them
		foreach (WheelData wd in mWheels)
		{
			Transform t = wd.col.transform;

			if (turnUsingWheels)
			{
				// Interpolation for smoother results
				wd.currentSteer = Mathf.Lerp(wd.currentSteer, st, Mathf.Clamp01(Time.fixedDeltaTime * wheelResponsiveness));

				// Rotate the wheel
				t.localRotation = Quaternion.Euler(0f, wd.currentSteer * wd.turnAngle, 0f);
			}

			// Optionally adjust the wheel distance, imitating suspension
			if (activeSuspension)
			{
				WheelHit hit;
				float suspension = wd.col.GetGroundHit(out hit) ? 0f : suspensionDistance;
				wd.currentSuspension = Mathf.Lerp(wd.currentSuspension, suspension, Time.fixedDeltaTime * 10f);
				t.localPosition = new Vector3(
					wd.startPos.x,
					wd.startPos.y - wd.currentSuspension,
					wd.startPos.z);
			}

			// Remember when we were last on the ground
			if (wd.col.isGrounded)
			{
				mIsGrounded = true;
				mLastGround = Time.time;
			}
		}

		// Reapply the center of mass after modifying the wheel position
		mRigidbody.centerOfMass = mCenterOfMass;

		if (isActive)
		{
			// Turn the tank manually, without using the wheels
			if (!turnUsingWheels)
			{
				float turn = st * turnRate;

				// Reverse the turning direction when driving in reverse
				if (intuitiveReverse && mMove.y < -0.25f) turn = -turn;

				// Linear interpolation for smoother results
				mTurn = Mathf.Lerp(mTurn, turn, Time.fixedDeltaTime * 10f);

				Quaternion currentRot = mRigidbody.transform.localRotation;
				Quaternion turnRot = Quaternion.Euler(0f, mTurn, 0f);
				mRigidbody.MoveRotation(turnRot * currentRot);
			}

			// Underwater? Drown!
			if (waterExplosion != null && mTrans.position.y < drownHeight)
			{
				GameObject go = (GameObject)Instantiate(waterExplosion, mTrans.position, Quaternion.identity);
				Destroy(go, 4f);
				SetState(mTrans.position, mTrans.rotation, false, false);
			}
		}
	}
}
}
