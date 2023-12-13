using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioManager : MonoBehaviour
{
	[SerializeField] private AudioClip walkSound;
	private Rigidbody rb;
	private AudioSource audioSource;
	private bool isRunning;
	private SoundEffect soundEffect;

	private float currentVolume;

    // Start is called before the first frame update
    void Start()
    {
		isRunning = false;
		currentVolume = 0f;
		rb = GetComponentInParent<Rigidbody>();
		audioSource = GetComponent<AudioSource>();
		soundEffect = GetComponent<SoundEffect>();
		StartCoroutine(SoundLoop(walkSound));
	}

    // Update is called once per frame
    void Update()
    {

    }
	
	private IEnumerator SoundLoop(AudioClip SoundEffect)
	{
		while (true)
		{
			float currentVelocity = (rb.velocity.magnitude);

			float baseDelay = 0.4f;

			switch (currentVelocity)
			{
				case < 5f and > 3.51f:
					currentVolume = 0.7f;
					PlayAudio(SoundEffect, currentVolume);
					yield return new WaitForSeconds(baseDelay / 1.25f);
					break;

				case < 3.51f and > 2.625f:
					currentVolume = 0.6f;
					PlayAudio(SoundEffect, currentVolume);
					yield return new WaitForSeconds(baseDelay / 1);
					break;

				case < 2.625f and > 1.75f:
					currentVolume = 0.5f;
					PlayAudio(SoundEffect, currentVolume);
					yield return new WaitForSeconds(baseDelay / 0.75f);
					break;

				case < 1.75f and > 0.875f:
					currentVolume = 0.4f;
					PlayAudio(SoundEffect, currentVolume);
					yield return new WaitForSeconds(baseDelay / 0.5f);
					break;

				default:
					currentVolume = 0f;
					PlayAudio(SoundEffect, currentVolume);
					yield return new WaitForSeconds(0.1f);
					break;
			}
		}
	}

	public void PlayAudio(AudioClip audioFile, float volume)
	{
		audioSource.PlayOneShot(audioFile, volume);
		soundEffect.OnPlayed(volume);
	}
}
