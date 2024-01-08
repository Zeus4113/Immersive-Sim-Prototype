using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Player;

public class PlayerAudioManager : MonoBehaviour
{
	private Movement m_movementComponent;
	private Controller m_playerController;

	[SerializeField] private AudioClip walkSound;
	private Rigidbody rb;
	private AudioSource audioSource;
	private bool isRunning;

	[SerializeField] private GroundedChecker m_detectionTrigger;

	[SerializeField] private AudioClip[] m_floorSoundEffects;
    [SerializeField] private AudioClip[] m_tileSoundEffects;
    [SerializeField] private AudioClip[] m_carpetSoundEffects;
    [SerializeField] private AudioClip[] m_woodSoundEffects;

	private float m_volumeModifier = 1f;

	[SerializeField] float m_baseDelay = 0.4f;
	[SerializeField] float m_baseVolume = 0.4f;

	// Start is called before the first frame update
	public void Init(Movement movement, Controller controller)
    {
		m_movementComponent = movement;
		m_playerController = controller;

		isRunning = false;
		rb = GetComponentInParent<Rigidbody>();
		audioSource = GetComponent<AudioSource>();

		StartWalkingAudio();
	}

	AudioClip GetRandomClip()
	{
		switch (m_detectionTrigger.GetTag())
		{
			case "Floor":
				m_volumeModifier = 1f;

                return m_floorSoundEffects[Random.Range(0, m_floorSoundEffects.Length)];

            case "Tile":
				m_volumeModifier = 2f;

                return m_tileSoundEffects[Random.Range(0, m_tileSoundEffects.Length)];

            case "Wood":
                m_volumeModifier = 0.8f;

                return m_woodSoundEffects[Random.Range(0, m_woodSoundEffects.Length)];

            case "Carpet":
                m_volumeModifier = 0.5f;

                return m_carpetSoundEffects[Random.Range(0, m_carpetSoundEffects.Length)];

			default:
				return null;
        }
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

		while (c_isWalking)
		{
			//float currentVelocity = (rb.velocity.magnitude);
			//float currentVelocity = new Vector2(rb.velocity.x, rb.velocity.z).magnitude;

			MovementStates currentState = m_movementComponent.GetMovementState();

			float delay = m_baseDelay;
			float volume = m_baseVolume;

			switch (currentState)
			{

				case MovementStates.normal:

					volume = volume * 1f;
					delay = delay / 1f;
					PlayAudio(volume);

					break;

				case MovementStates.running:

					volume = volume * 1.5f;
					delay = delay / 1.5f;
					PlayAudio(volume);

					break;

				case MovementStates.walking:

					volume = volume * 0.5f;
					delay = delay / 0.5f;
					PlayAudio(volume);

					break;

				case MovementStates.crouched:

					volume = volume * 0.75f;
					delay = delay / 0.75f;
					PlayAudio(volume);

					break;

				case MovementStates.crouchedWalking:

					volume = volume * 0.25f;
					delay = delay / 0.25f;
					PlayAudio(volume);

					break;

				default:

					volume = volume * 0f;
					delay = delay / 0f;
					PlayAudio(volume);

					break;

			}

			yield return new WaitForSeconds(delay);

			//switch (currentVelocity)
			//{
			//	case <= 7f and > 3.51f:
			//		currentVolume = m_baseVolume * 1.5f;
			//		PlayAudio();
			//		yield return new WaitForSeconds(m_baseDelay / 1.5f);
			//		break;

			//	case <= 3.51f and > 2.625f:
			//		currentVolume = m_baseVolume * 1;
			//		PlayAudio();
			//		yield return new WaitForSeconds(m_baseDelay / 1);
			//		break;

			//	case <= 2.625f and > 1.75f:
			//		currentVolume = m_baseVolume * 0.5f;
			//		PlayAudio();
			//		yield return new WaitForSeconds(m_baseDelay / 0.5f);
			//		break;

			//	case <= 1.75f and > 0.875f:
			//		currentVolume = m_baseVolume * 0.25f;
			//		PlayAudio();
			//		yield return new WaitForSeconds(m_baseDelay / 0.25f);
			//		break;

			//	default:
			//		currentVolume = 0f;
			//		StopWalkingAudio();
			//		break;
			//}
		}
	}

	public void PlayAudio(float volume)
	{
		if (m_detectionTrigger.IsGrounded() && rb.velocity.magnitude > 0.1f)
		{
			audioSource.volume = volume;
			audioSource.clip = GetRandomClip();
			audioSource.Play();
		}
	}

	public float GetModifier()
	{
		return m_volumeModifier;
	}
}
