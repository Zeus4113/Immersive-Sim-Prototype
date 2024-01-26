using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Footsteps : MonoBehaviour
{
	[SerializeField] float m_stepDistance = 2f;

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
	[SerializeField] private StandChecker m_standChecker;
	[SerializeField] private Transform m_cameraPosition;
	[SerializeField] private CapsuleCollider m_collider;

	[Header("Crouch Camera Transforms")]
	[SerializeField] private Transform m_standTransform;
	[SerializeField] private Transform m_crouchTransform;
	[SerializeField] private Transform m_playerMesh;

	Player.Movement m_movement;

	float m_steppingDistance = 0f;
	int fullstepSwitcher = 1;
	int halfstepSwitcher = 1;

	bool m_isFalling = false;
	bool m_isCrouched = false;

	float m_leanAmount = 0f;

	private Player.Controller m_controller;
	private PlayerInput m_playerInput;

	public void Init(PlayerInput input, Player.Controller controller)
	{
		m_playerInput = input;
		m_controller = controller;

		m_rigidbody = GetComponent<Rigidbody>();
		m_audioSource = GetComponent<AudioSource>();
		m_movement = GetComponent<Player.Movement>();

		BindEvents();
		StartUpdating();

	}

	void BindEvents()
	{
		m_playerInput.actions.FindAction("Crouch").performed += Crouch;
		m_playerInput.actions.FindAction("Crouch").canceled += Crouch;

		m_playerInput.actions.FindAction("Lean").performed += StartPlayerLean;
		m_playerInput.actions.FindAction("Lean").canceled += StopPlayerLean;

		m_playerInput.actions.FindAction("Rotation").performed += StartRotation;
		m_playerInput.actions.FindAction("Rotation").canceled += StopRotation;

		m_playerInput.actions.FindAction("Tool Menu").performed += SwitchMenuOpen;
		m_playerInput.actions.FindAction("Tool Menu").canceled += SwitchMenuOpen;
	}

	bool m_menuOpen = false;

	void SwitchMenuOpen(InputAction.CallbackContext ctx)
	{
		m_menuOpen = !m_menuOpen;
	}

	bool c_isUpdating = false;
	Coroutine c_updating;

	void StartUpdating()
	{
		if (c_isUpdating) return;
		c_isUpdating = true;

		if (c_updating != null) return;
		c_updating = StartCoroutine(Updating());
	}

	void StopUpdating()
	{
		if (!c_isUpdating) return;
		c_isUpdating = false;

		if(c_updating == null) return;
		StopCoroutine(c_updating);
		c_updating = null;
	}

	IEnumerator Updating()
	{
		while (c_isUpdating)
		{
			PlayerHeadbob();
			JumpSound();
			CrouchCamera();
			yield return new WaitForFixedUpdate();
		}
	}

	void CrouchCamera()
	{
		if (m_isCrouched && m_cameraPosition.position != m_crouchTransform.position)
		{
			m_cameraPosition.position = Vector3.Lerp(m_cameraPosition.position, m_crouchTransform.position, Time.fixedDeltaTime * 2);
			m_playerMesh.localScale = Vector3.Lerp(m_playerMesh.localScale, new Vector3(1, 0.5f, 1), Time.fixedDeltaTime * 2);
			m_collider.height = m_playerMesh.localScale.y * 2;
		}
		else if (!m_isCrouched && m_cameraPosition.position != m_standTransform.position)
		{
			m_cameraPosition.position = Vector3.Lerp(m_cameraPosition.position, m_standTransform.position, Time.fixedDeltaTime * 2);
			m_playerMesh.localScale = Vector3.Lerp(m_playerMesh.localScale, new Vector3(1, 1, 1), Time.fixedDeltaTime * 2);
			m_collider.height = m_playerMesh.localScale.y * 2;
		}
	}

	void Crouch(InputAction.CallbackContext ctx)
	{
		if (m_standChecker.IsBlocked() && m_isCrouched)
		{

		}
		else if(!m_standChecker.IsBlocked())
		{
			m_isCrouched = !m_isCrouched;
		}
	}

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

	void PlayerHeadbob()
	{
		Vector3 velocity = transform.InverseTransformDirection(m_rigidbody.velocity);
		Vector3 headbob;
		float xAxisSinWave;
		float yAxisCosWave;

		if (velocity.magnitude < 0.1f)
		{
			//Debug.Log(m_cameraPosition.localRotation.eulerAngles);

			m_cameraPosition.localPosition = Vector3.Lerp(
				m_cameraPosition.localPosition,
				new Vector3(GetLeanAmount(), m_cameraPosition.localPosition.y, 0),
				(Time.fixedDeltaTime * m_lerpSpeed)
			);

			return;
		}

		if (velocity.magnitude > 0.1f && !m_rigidbodyChecker.IsGrounded() && m_isFalling)
		{
			m_cameraPosition.localPosition = Vector3.Lerp(
				m_cameraPosition.localPosition,
				new Vector3(0, m_cameraPosition.localPosition.y + (velocity.y / 4), 0),
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

			headbob = new Vector3((xAxisSinWave * m_xAxisHeadbobModifier) + GetLeanAmount(), (m_cameraPosition.localPosition.y - (m_yAxisHeadbobModifier / 2)) + (yAxisCosWave * m_yAxisHeadbobModifier), 0);

			m_cameraPosition.localPosition = Vector3.Lerp(m_cameraPosition.localPosition, headbob, Time.fixedDeltaTime * m_lerpSpeed);

			//m_cameraPosition.localRotation = Quaternion.Lerp(m_cameraPosition.localRotation, Quaternion.Euler(m_movement.GetCameraRotation(), m_cameraPosition.localRotation.y, m_cameraPosition.localRotation.z + (GetLeanAmount() * 10f)), Time.fixedDeltaTime);
		}
	}

	float direction = -1;

	public void StartPlayerLean(InputAction.CallbackContext ctx)
	{
		direction = ctx.ReadValue<float>();
		m_leanAmount = 0.65f * -direction;
		RotateCamera(0f);
	}

	public void StopPlayerLean(InputAction.CallbackContext ctx)
	{
		direction = ctx.ReadValue<float>();
		m_leanAmount = 0f * -direction;
		RotateCamera(0f);
	}

	public float GetLeanAmount()
	{
		return m_leanAmount;
	}

	float mouseSensitivityCamera = 0.2f;
	float minCameraRotation = -80f;
	float maxCameraRotation = 60f;

	private void RotateCamera(float inputCameraRotationValue)
	{
		Vector3 currentRotationalValue = m_cameraPosition.transform.rotation.eulerAngles;
		float clampedValue = MathTools.ClampAngle(currentRotationalValue.x - (inputCameraRotationValue * mouseSensitivityCamera), minCameraRotation, maxCameraRotation);
		m_cameraPosition.transform.localRotation = Quaternion.Euler(clampedValue, 0, (0f - (GetLeanAmount())));
	}


	bool c_isRotating = false;
	Coroutine c_rotating;

	void StartRotation(InputAction.CallbackContext ctx)
	{
		if (c_isRotating) return;

		c_isRotating = true;

		if (c_rotating != null) return;

		c_rotating = StartCoroutine(Rotating(ctx));
	}

	void StopRotation(InputAction.CallbackContext ctx)
	{
		if (!c_isRotating) return;

		c_isRotating = false;

		if (c_rotating == null) return;

		StopCoroutine(c_rotating);
		c_rotating = null;
	}

	IEnumerator Rotating(InputAction.CallbackContext ctx)
	{
		if (c_isRotating)
		{
			if (!m_menuOpen && Time.timeScale != 0)
			{
				float cameraRotationValue = ctx.ReadValue<Vector2>().y;
				RotateCamera(cameraRotationValue);
			}

			yield return new WaitForFixedUpdate();
		}

		StopRotation(ctx);

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
