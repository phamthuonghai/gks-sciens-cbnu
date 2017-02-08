using UnityEngine;

/// <summary>
/// Randomize the audio source's pitch.
/// </summary>

[RequireComponent(typeof(AudioSource))]
public class RandomizePitch : MonoBehaviour
{
	public float minimum = 0.85f;
	public float maximum = 1.15f;

	void Awake ()
	{
		audio.pitch = Random.Range(minimum, maximum);
	}
}
