using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Tasharen
{
/// <summary>
/// Abstract vehicle class.
/// </summary>

public abstract class Vehicle : MonoBehaviour
{
	class MatEntry
	{
		public Material mat;
		public Color color;
		public Color color0;
		public Color color1;
		public Color color2;
		public Color spec;
	}

	/// <summary>
	/// Particle system for dust trails.
	/// </summary>

	public ParticleSystem dustEmitter;

	/// <summary>
	/// Audio source that's supposed to be playing the engine sound.
	/// </summary>

	public AudioSource engineSound;

	/// <summary>
	/// Children to deactivate when the vehicle explodes.
	/// </summary>

	public GameObject[] disableOnExplosion;

	List<MatEntry> mSaved = new List<MatEntry>();
	bool mIsActive = true;

	protected Transform mTrans;
	protected Rigidbody mRb;
	protected Vector2 mMove = Vector2.zero;
	protected Vector2 mTilt = Vector2.zero;
	protected Color mEmitterColor = new Color32(0, 0, 0, 0);
	protected Transform mEmitterTrans;

	/// <summary>
	/// Whether the vehicle is currently active or has been destroyed.
	/// </summary>

	public bool isActive { get { return mIsActive; } set { mIsActive = value; } }

	/// <summary>
	/// Current engine speed, ranging from -1 to 1 (though generally less due to breaking friction).
	/// </summary>

	public abstract float engineSpeed { get; }

	/// <summary>
	/// Cache the audio source.
	/// </summary>

	void Awake ()
	{
		mTrans = transform;
		mRb = rigidbody;

		if (dustEmitter != null)
		{
			mEmitterColor = dustEmitter.startColor;
			mEmitterTrans = dustEmitter.transform;
			dustEmitter.enableEmission = false;
		}
	}

	/// <summary>
	/// Update the movement values.
	/// </summary>

	void Update ()
	{
		// Current controller angle
		if (isActive)
		{
			mMove = InputManager.leftThumb;
			mTilt = InputManager.rightThumb;
		}
		else
		{
			mMove = Vector2.zero;
			mTilt = Vector2.zero;
		}

		// Run all custom logic
		OnUpdate();
	}

	/// <summary>
	/// Kill the vehicle, optionally exploding it for the fun of it!
	/// </summary>

	public void SetState (Vector3 pos, Quaternion rot, bool activeState, bool explode)
	{
		if (activeState)
		{
			mIsActive = true;

			if (explode)
			{
				// Restore all colors
				MeshRenderer[] rens = GetComponentsInChildren<MeshRenderer>();

				foreach (MeshRenderer ren in rens)
				{
					Material[] mats = ren.materials;

					foreach (Material mat in mats)
					{
						foreach (MatEntry me in mSaved)
						{
							if (me.mat == mat)
							{
								mat.color = me.color;
								mat.SetColor("_Color0", me.color0);
								mat.SetColor("_Color1", me.color1);
								mat.SetColor("_Color2", me.color2);
								mat.SetColor("_SpecColor", me.spec);
								break;
							}
						}
					}
				}
				mSaved.Clear();
			}

			// Activate the game objects
			if (disableOnExplosion != null)
			{
				for (int i = 0, imax = disableOnExplosion.Length; i < imax; ++i)
				{
					GameObject go = disableOnExplosion[i];
					if (go != null) go.SetActive(true);
				}
			}

			mTrans.position = pos;
			mTrans.rotation = rot;
			mIsActive = true;
			OnRespawn();
		}
		else
		{
			mIsActive = false;
			mTrans.position = pos;
			mTrans.rotation = rot;

			// Disable particle emission
			if (dustEmitter != null) dustEmitter.enableEmission = false;

			// Deactivate the game objects
			if (explode)
			{
				if (disableOnExplosion != null)
				{
					for (int i = 0, imax = disableOnExplosion.Length; i < imax; ++i)
					{
						GameObject go = disableOnExplosion[i];
						if (go != null) go.SetActive(false);
					}
				}

				// Collect all renderers and tint them
				MeshRenderer[] rens = GetComponentsInChildren<MeshRenderer>();

				Color charred = new Color32(36, 29, 19, 255);

				foreach (MeshRenderer ren in rens)
				{
					Material[] mats = ren.materials;

					foreach (Material mat in mats)
					{
						MatEntry ent = new MatEntry();
						ent.mat = mat;
						mSaved.Add(ent);

						if (mat.HasProperty("_Color")) ent.color = mat.GetColor("_Color");
						if (mat.HasProperty("_Color0")) ent.color0 = mat.GetColor("_Color0");
						if (mat.HasProperty("_Color1")) ent.color1 = mat.GetColor("_Color1");
						if (mat.HasProperty("_Color2")) ent.color2 = mat.GetColor("_Color2");
						if (mat.HasProperty("_SpecColor")) ent.spec = mat.GetColor("_SpecColor");

						mat.color = charred;
						mat.SetColor("_Color0", charred);
						mat.SetColor("_Color1", charred);
						mat.SetColor("_Color2", charred);
						mat.SetColor("_SpecColor", charred);
					}
				}
			}

			// Start the respawn logic
			StartCoroutine(Respawn(4f, explode));

			// Death notification
			OnDeath();
		}
	}

	/// <summary>
	/// Create an explosion at the specified location.
	/// </summary>

	public void AddExplosionForce (Vector3 pos, float hitRadius, float force, float forceRadius)
	{
		if (hitRadius > 0f)
		{
			Vector3 offset = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));
			float mag = offset.magnitude;
			if (mag != 0f) offset /= mag;
			pos += offset * hitRadius;
		}

		pos.y -= 1f;
		Collider[] cols = Physics.OverlapSphere(pos, forceRadius);
		List<Rigidbody> rbs = new List<Rigidbody>();

		foreach (Collider c in cols)
		{
			Rigidbody rb = FindInParents<Rigidbody>(c.gameObject);

			if (rb != null && !rbs.Contains(rb))
			{
				rbs.Add(rb);
				Vehicle v = rb.GetComponent<Vehicle>();

				if (v != null)
				{
					// TODO: This would be the place to deal damage and subtract from the vehicle's health
					if (v.isActive && Vector3.Distance(v.transform.position, pos) < 2.5f)
					{
						v.Explode();
					}
					else
					{
						rb.AddExplosionForce(force * rb.mass, pos, forceRadius);
					}
				}
				else
				{
					rb.AddExplosionForce(force * rb.mass, pos, forceRadius);
				}
			}
		}
	}

	/// <summary>
	/// Explode the vehicle.
	/// </summary>

	protected virtual void Explode ()
	{
		isActive = false;
		float force = 400f;
		float forceRadius = 5f;
		SetState(mTrans.position, mTrans.rotation, false, true);
		AddExplosionForce(mTrans.position, (force != 0f) ? 1f : 0f, force, forceRadius);
	}

	/// <summary>
	/// Respawn the tank.
	/// </summary>

	IEnumerator Respawn (float delay, bool explode)
	{
		yield return new WaitForSeconds(delay);

		Random.seed = (int)(System.DateTime.Now.Ticks / 10000);
		Vector3 pos = Quaternion.Euler(0f, Random.Range(-180f, 180f), 0f) * Vector3.forward;
		Quaternion rot = Quaternion.LookRotation(-pos);
		pos = pos * 30f + Vector3.up * 12f;
		SetState(pos, rot, true, explode);
	}

	/// <summary>
	/// Finds the specified component on the game object or one of its parents.
	/// </summary>

	static public T FindInParents<T> (GameObject go) where T : Component
	{
		if (go == null) return null;
		object comp = go.GetComponent<T>();

		if (comp == null)
		{
			Transform t = go.transform.parent;

			while (t != null && comp == null)
			{
				comp = t.gameObject.GetComponent<T>();
				t = t.parent;
			}
		}
		return (T)comp;
	}

	protected virtual void OnDeath () { }
	protected virtual void OnRespawn () { }
	protected virtual void OnUpdate () { }
}
}
