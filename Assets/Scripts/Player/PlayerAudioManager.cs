using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioManager : MonoBehaviour
{
	[SerializeField] private AudioClip walkSound;
	private Rigidbody rb;
	private AudioSource audioSource;
	private bool isRunning;

	[SerializeField] private AudioClip[] soundEffects;

	private float currentVolume;

    // Start is called before the first frame update
    void Start()
    {
		isRunning = false;
		currentVolume = 0f;
		rb = GetComponentInParent<Rigidbody>();
		audioSource = GetComponent<AudioSource>();
	}

    // Update is called once per frame
    void FixedUpdate()
    {
		if(rb.velocity.magnitude > 0.875f)
		{
			StartWalkingAudio();
		}
    }

	AudioClip GetRandomClip()
	{
		return soundEffects[Random.Range(0, soundEffects.Length)];
	}

	bool c_isWalking = false;
	Coroutine c_walking;

	void StartWalkingAudio()
	{
		if (c_isWalking) return;

		c_isWalking = true;

		if (c_walking != null) return;

		c_walking = StartCoroutine(WalkingAudio());

	}

	void StopWalkingAudio()
	{
		if (!c_isWalking) return;

		c_isWalking = false;

		if (c_walking == null) return;

		StopCoroutine(c_walking);
		c_walking = null;
	}

	private IEnumerator WalkingAudio()
	{
		float baseDelay = 0.4f;
		float baseVolume = 0.4f;

		while (c_isWalking)
		{
			//float currentVelocity = (rb.velocity.magnitude);

			float currentVelocity = new Vector2(rb.velocity.x, rb.velocity.z).magnitude;

			switch (currentVelocity)
			{
				case <= 5f and > 3.51f:
					currentVolume = baseVolume * 1.25f;
					PlayAudio();
					yield return new WaitForSeconds(baseDelay / 1.25f);
					break;

				case <= 3.51f and > 2.625f:
					currentVolume = baseVolume * 1;
					PlayAudio();
					yield return new WaitForSeconds(baseDelay / 1);
					break;

				case <= 2.625f and > 1.75f:
					currentVolume = baseVolume * 0.75f;
					PlayAudio();
					yield return new WaitForSeconds(baseDelay / 0.75f);
					break;

				case <= 1.75f and > 0.875f:
					currentVolume = baseVolume * 0.5f;
					PlayAudio();
					yield return new WaitForSeconds(baseDelay / 0.5f);
					break;

				default:
					currentVolume = 0f;
					StopWalkingAudio();
					break;
			}
		}
	}

	public void PlayAudio()
	{
		audioSource.volume = currentVolume;
		audioSource.clip = GetRandomClip();
		audioSource.Play();
		//soundEffect.OnPlayed(volume);
	}
}
