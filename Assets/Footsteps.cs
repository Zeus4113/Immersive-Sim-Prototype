using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footsteps : MonoBehaviour
{
	float m_stepDistance = 2f;
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
	[Space(2)]

	[Header("References")]
	[SerializeField] private GroundedChecker m_rigidbodyChecker;
	[SerializeField] private Transform m_cameraPosition;

	private void Start()
	{
		m_rigidbody = GetComponent<Rigidbody>();
		m_audioSource = GetComponent<AudioSource>();
	}

	private void FixedUpdate()
	{
		FootstepSound();
		HeadbobEffect();
		JumpSound();
	}

	void FootstepSound()
	{
		float magnitude = m_rigidbody.velocity.magnitude - m_rigidbody.velocity.y;

		if (magnitude > 0.01f && m_rigidbodyChecker.IsGrounded())
		{
			m_distanceSinceLastStep += magnitude * Time.fixedDeltaTime;

			if (m_distanceSinceLastStep > m_stepDistance)
			{
				m_distanceSinceLastStep = 0;
				m_audioSource.clip = DetermineAudioClip(m_rigidbodyChecker.GetTag());
				m_audioSource.volume = (magnitude * 0.1f) * DetermineVolumeModifer(m_rigidbodyChecker.GetTag());
				m_audioSource.Play();
			}
		}
	}

	void JumpSound()
	{
		float magnitude = m_rigidbody.velocity.y;

		if (magnitude! > 0.1f || magnitude! < -0.1f) return;

		if (magnitude < -2f)
		{
			if (m_rigidbodyChecker.IsGrounded())
			{
				Debug.LogWarning("Grounded " + m_rigidbodyChecker.GetTag());
				m_audioSource.clip = DetermineAudioClip(m_rigidbodyChecker.GetTag());
				m_audioSource.volume = 0.35f * DetermineVolumeModifer(m_rigidbodyChecker.GetTag());
				m_audioSource.Play();
			}
		}

	}
		void HeadbobEffect()
		{
			Vector3 velocity = transform.InverseTransformDirection(m_rigidbody.velocity);

			float xAxisMovement = velocity.x;
			float zAxisMovement = velocity.z;
			float yAxisMovement = Mathf.Sin(m_distanceSinceLastStep / m_stepDistance);

			Vector3 headMovement = Vector3.Lerp(
				m_cameraPosition.localPosition,
				new Vector3((xAxisMovement * m_xAxisHeadbobModifier), 0.5f + (yAxisMovement * m_yAxisHeadbobModifier), (zAxisMovement * m_zAxisHeadbobModifier)),
				Time.fixedDeltaTime);

			m_cameraPosition.localPosition = headMovement;

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
