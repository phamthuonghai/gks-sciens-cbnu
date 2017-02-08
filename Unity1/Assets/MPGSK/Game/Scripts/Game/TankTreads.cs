using UnityEngine;

namespace Tasharen
{
/// <summary>
/// Used to animate the tank's treads.
/// </summary>

[RequireComponent(typeof(Renderer))]
public class TankTreads : MonoBehaviour
{
	public int materialIndex = 0;
	public float animationSpeed = 1f;
	
	Vehicle mVehicle = null;
	Material mMat = null;
	float mOffset = 0f;

	void Start()
	{
		mVehicle = Vehicle.FindInParents<Vehicle>(gameObject);
		
		if (mVehicle == null)
		{
			Debug.LogError("No Vehicle found");
			Destroy(this);
			return;
		}
		
		if (materialIndex < renderer.materials.Length)
		{
			mMat = renderer.materials[materialIndex];
		}
		if (mMat == null) Destroy(this);
	}

	void Update()
	{
		mOffset += animationSpeed * Time.deltaTime * (mVehicle.engineSpeed * 4f) * mMat.mainTextureScale.x;
		while (mOffset < 0f) mOffset += 1f;
		while (mOffset > 1f) mOffset -= 1f;
		mMat.mainTextureOffset = new Vector2(mOffset, 0f);
	}
}
}
