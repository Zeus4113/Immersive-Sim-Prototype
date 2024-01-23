using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footsteps : MonoBehaviour
{
	[SerializeField] float m_stepDistance = 2f;
	float m_distanceSinceLastStep = 0f;

	AudioSource m_audioSource;
	Rigidbody m_rigidbody;

	[Header("Audio Clip Packs")]
	[SerializeField] private AudioClip[] m_floorClips;
	[SerializeField] private AudioClip[] m_woodClips;
	[SerializeField] private AudioClip[] m_carpetClips;
	[SerializeField] private AudioClip[] m_tileClips;
	[Space(2)]

	[Header("Floats")]
	[SerializeField] private float m_xAxisHeadbobModifier = 1f;
	[SerializeField] private float m_zAxisHeadbobModifier = 1f;
	[SerializeField] private float m_yAxisHeadbobModifier = 1f;
	[SerializeField] private float m_lerpSpeed = 1f;
	[Space(2)]

	[Header("References")]
	[SerializeField] private GroundedChecker m_rigidbodyChecker;
	[SerializeField] private Transform m_cameraPosition;

	Player.Movement m_movement;

	private void Start()
	{
		m_rigidbody = GetComponent<Rigidbody>();
		m_audioSource = GetComponent<AudioSource>();
		m_movement = GetComponent<Player.Movement>();
	}

	private void FixedUpdate()
	{
		PlayerHeadbob();
		JumpSound();
	}

	bool m_isFalling;

	void JumpSound()
	{
		float magnitude = m_rigidbody.velocity.y;

		if(magnitude < -2f && !m_isFalling && !m_rigidbodyChecker.IsGrounded())
		{
			m_isFalling = true;
		}

		if (m_rigidbodyChecker.IsGrounded() & m_isFalling)
		{
			m_steppingDistance = 0f;
			m_isFalling = false;
			m_audioSource.clip = DetermineAudioClip(m_rigidbodyChecker.GetTag());
			m_audioSource.volume = 1f * DetermineVolumeModifer(m_rigidbodyChecker.GetTag());
			m_audioSource.Play();
			//Debug.Log("Jump Sound");
		}

	}

	float m_steppingDistance = 0f;
	int fullstepSwitcher = 1;
	int halfstepSwitcher = 1;

	void PlayerHeadbob()
	{
		Vector3 velocity = transform.InverseTransformDirection(m_rigidbody.velocity);
		Vector3 headbob;
		float xAxisSinWave;
		float yAxisCosWave;

		if (velocity.magnitude < 0.1f)
		{
			m_cameraPosition.localPosition = Vector3.Lerp(
				m_cameraPosition.localPosition,
				new Vector3(m_movement.GetLeanAmount(), m_movement.GetCameraHeight(), 0),
				(Time.fixedDeltaTime * m_lerpSpeed)
			);

			return;
		}

		if (velocity.magnitude > 0.1f && !m_rigidbodyChecker.IsGrounded() && m_isFalling)
		{
			m_cameraPosition.localPosition = Vector3.Lerp(
				m_cameraPosition.localPosition,
				new Vector3(0, m_movement.GetCameraHeight() + (velocity.y / 4), 0),
				(Time.fixedDeltaTime * m_lerpSpeed)
			);

			return;
		}

		if (velocity.magnitude > 0.1f && m_rigidbodyChecker.IsGrounded() && !m_isFalling) 
		{

			m_steppingDistance += velocity.magnitude * halfstepSwitcher * Time.fixedDeltaTime;

			//Debug.Log("Is Walking: " + m_steppingDistance);

			if (m_steppingDistance >= m_stepDistance)
			{
				m_steppingDistance = m_stepDistance;
				halfstepSwitcher = halfstepSwitcher * -1;
				m_audioSource.clip = DetermineAudioClip(m_rigidbodyChecker.GetTag());
				m_audioSource.volume = (velocity.magnitude * 0.1f) * DetermineVolumeModifer(m_rigidbodyChecker.GetTag());
				m_audioSource.Play();
				//Debug.Log("Footstep Sound Firing");
			}

			if (m_steppingDistance <= 0)
			{
				m_steppingDistance = 0;
				fullstepSwitcher = -fullstepSwitcher;
				halfstepSwitcher = halfstepSwitcher * -1;
			}

			yAxisCosWave = Mathf.Cos(m_steppingDistance / m_stepDistance);
			xAxisSinWave = Mathf.Sin(m_steppingDistance / m_stepDistance) * fullstepSwitcher;

			headbob = new Vector3((xAxisSinWave * m_xAxisHeadbobModifier) + m_movement.GetLeanAmount(), (m_movement.GetCameraHeight() - (m_yAxisHeadbobModifier / 2)) + (yAxisCosWave * m_yAxisHeadbobModifier), 0);

			m_cameraPosition.localPosition = Vector3.Lerp(m_cameraPosition.localPosition, headbob, Time.fixedDeltaTime * m_lerpSpeed);
		}
	}

	AudioClip DetermineAudioClip(string tag)
	{
		tag = tag.ToLower();

		switch (tag)
		{
			case "floor":

				return m_floorClips[Random.Range(0, m_floorClips.Length)];

			case "wood":

				return m_woodClips[Random.Range(0, m_woodClips.Length)]; ;

			case "carpet":

				return m_carpetClips[Random.Range(0, m_carpetClips.Length)]; ;

			case "tile":

				return m_tileClips[Random.Range(0, m_tileClips.Length)]; ;

			default:

				return null;
		}
	}

	float DetermineVolumeModifer(string tag)
	{
		tag = tag.ToLower();

		switch (tag)
		{
			case "floor":

				return 1f;

			case "wood":

				return 0.7f;

			case "carpet":

				return 0.35f;

			case "tile":

				return 1.5f;

			default:

				return 0;
		}
	}
	}
