using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioManager : MonoBehaviour
{
	[SerializeField] private AudioClip walkSound;
	private Rigidbody rb;
	private AudioSource audioSource;
	private bool isRunning;

    // Start is called before the first frame update
    void Start()
    {
		isRunning = false;
		rb = GetComponentInParent<Rigidbody>();
		audioSource = GetComponent<AudioSource>();
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
			Debug.Log(currentVelocity);

			float baseDelay = 0.4f;

			switch (currentVelocity)
			{
				case < 5f and > 3.51f:
					PlayAudio(SoundEffect, 0.7f);
					yield return new WaitForSeconds(baseDelay / 1.25f);
					break;

				case < 3.51f and > 2.625f:
					PlayAudio(SoundEffect, 0.6f);
					yield return new WaitForSeconds(baseDelay / 1);
					break;

				case < 2.625f and > 1.75f:
					PlayAudio(SoundEffect, 0.5f);
					yield return new WaitForSeconds(baseDelay / 0.75f);
					break;

				case < 1.75f and > 0.875f:
					PlayAudio(SoundEffect, 0.4f);
					yield return new WaitForSeconds(baseDelay / 0.5f);
					break;

				default:
					yield return new WaitForSeconds(0.1f);
					break;
			}
		}
	}

	public void PlayAudio(AudioClip audioFile, float volume)
	{
		audioSource.PlayOneShot(audioFile, volume);
	}
}
