using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

namespace Player
{
	//Movement States Enum
	public enum MovementStates
	{
		idle,
		normal,
		walking,
		running,
		crouched,
		crouchedWalking,
	}

	public class Movement : MonoBehaviour
	{
		[SerializeField] private PlayerInput m_playerInput;


		// Player speed + modifier variables
		[Header("Player Speed Variables")]
		[SerializeField] private float baseMovementSpeed = 10f;
		[SerializeField] private float runSpeedModifier = 1.5f;
		[SerializeField] private float walkSpeedModifier = 0.5f;
		[SerializeField] private float crouchWalkSpeedModifer = 0.3f;
		[SerializeField] private float crouchSpeedModifer = 0.3f;
		[Space(2)]

		[Header("Player Rotation Sensitivity")]
		// Player rotation sensitivity variable
		[SerializeField] private float mouseSensitivityPlayer = 0.8f;
		[Space(2)]


		[Header("Player Jump Variables")]
		// Player jump variable
		[SerializeField] private float jumpForce = 100f;
		[Space(2)]

		[Header("Camera Variables")]
		// Camera Settings
		[SerializeField] private float mouseSensitivityCamera;
		[SerializeField] private float minCameraRotation;
		[SerializeField] private float maxCameraRotation;


		// Movement
		private Vector2 currentMovementValue;
		private float currentMovementSpeed;
		private MovementStates currentMovementState;

		// Rotation
		private float playerRotationValue;
		private float cameraRotationValue;

		// Components
		private Collider playerCollider;
		private MeshRenderer playerMesh;
		private Rigidbody playerRigidbody;
		private Camera playerCamera;

		private bool crouchingBlocked;
		private bool isGrounded;

		private void Start()
		{
			// Grab Components
			playerCollider = GetComponent<Collider>();
			playerRigidbody = GetComponent<Rigidbody>();
			playerMesh = GetComponentInChildren<MeshRenderer>();
			playerCamera = GetComponentInChildren<Camera>();

			if (m_playerInput != null) EnableInputEvents(true);

			// Lock mouse cursor
			Cursor.lockState = CursorLockMode.Locked;

			// Set defaults
			SetMovementMode(MovementStates.normal);
		}

		private void EnableInputEvents(bool isTrue)
		{
			if (isTrue)
			{
				m_playerInput.actions.FindAction("Movement").performed += StartMovement;
				m_playerInput.actions.FindAction("Movement").canceled += StopMovement;

				m_playerInput.actions.FindAction("Rotation").performed += StartRotation;
				m_playerInput.actions.FindAction("Rotation").canceled += StopRotation;

				m_playerInput.actions.FindAction("Jump").performed += OnJump;

				m_playerInput.actions.FindAction("Crouch").performed += OnCrouch;
				m_playerInput.actions.FindAction("Crouch").canceled += OnCrouch;

				m_playerInput.actions.FindAction("Walk").performed += OnWalk;
				m_playerInput.actions.FindAction("Walk").canceled += OnWalk;

				m_playerInput.actions.FindAction("Run").performed += OnRun;
				m_playerInput.actions.FindAction("Run").canceled += OnRun;
			}
			else if (!isTrue)
			{
				m_playerInput.actions.FindAction("Movement").performed -= StartMovement;
				m_playerInput.actions.FindAction("Movement").canceled -= StopMovement;

				m_playerInput.actions.FindAction("Rotation").performed -= StartRotation;
				m_playerInput.actions.FindAction("Rotation").canceled -= StopRotation;

				m_playerInput.actions.FindAction("Jump").performed -= OnJump;

				m_playerInput.actions.FindAction("Crouch").performed -= OnCrouch;
				m_playerInput.actions.FindAction("Crouch").canceled -= OnCrouch;

				m_playerInput.actions.FindAction("Walk").performed -= OnWalk;
				m_playerInput.actions.FindAction("Walk").canceled -= OnWalk;

				m_playerInput.actions.FindAction("Run").performed -= OnRun;
				m_playerInput.actions.FindAction("Run").canceled -= OnRun;
			}
		}

		bool c_isMoving = false;
		Coroutine c_moving;

		void StartMovement(InputAction.CallbackContext ctx)
		{
			if (c_isMoving) return;

			c_isMoving = true;

			if (c_moving != null) return;

			c_moving = StartCoroutine(Moving(ctx));
		}

		void StopMovement(InputAction.CallbackContext ctx)
		{
			if (!c_isMoving) return;

			c_isMoving = false;

			if (c_moving == null) return;

			StopCoroutine(c_moving);
			c_moving = null;
		}

		IEnumerator Moving(InputAction.CallbackContext ctx)
		{
			while (c_isMoving)
			{
				currentMovementValue = ctx.ReadValue<Vector2>();
				Vector3 direction = playerRigidbody.rotation * new Vector3(currentMovementValue.x, 0, currentMovementValue.y);
				Vector3 velocity = direction * currentMovementSpeed * 100 * Time.fixedDeltaTime;
				velocity.y = playerRigidbody.velocity.y;
				playerRigidbody.velocity = velocity;

				yield return new WaitForFixedUpdate();
			}

			StopMovement(ctx);

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
				playerRotationValue = ctx.ReadValue<Vector2>().x;
				cameraRotationValue = ctx.ReadValue<Vector2>().y;

				Quaternion rotation = Quaternion.Euler(new Vector3(0, playerRotationValue * mouseSensitivityPlayer, 0) * Time.fixedDeltaTime);
				playerRigidbody.MoveRotation(playerRigidbody.rotation * rotation);

				RotateCamera(cameraRotationValue);
				yield return new WaitForFixedUpdate();
			}

			StopRotation(ctx);

		}

		public void CrouchingBlocked(bool isTrue)
		{
			crouchingBlocked = isTrue;
		}

		public void CheckGrounded(bool isTrue)
		{
			isGrounded = isTrue;
		}

		private void RotateCamera(float inputCameraRotationValue)
		{
			Vector3 currentRotationalValue = playerCamera.transform.rotation.eulerAngles;
			float clampedValue = MathTools.ClampAngle(currentRotationalValue.x - (inputCameraRotationValue * mouseSensitivityCamera), minCameraRotation, maxCameraRotation);
			playerCamera.transform.localRotation = Quaternion.Euler(clampedValue, 0, 0);
		}

		private void SetMovementMode(MovementStates newState)
		{
			switch(newState)
			{
				case MovementStates.normal:
					SetCapsule("standing");
					SetSpeed(1f);
					currentMovementState = MovementStates.normal;
					break;

				case MovementStates.walking:
					SetCapsule("standing");
					SetSpeed(walkSpeedModifier);
					currentMovementState = MovementStates.walking;
					break;

				case MovementStates.running:
					SetCapsule("standing");
					SetSpeed(runSpeedModifier);
					currentMovementState = MovementStates.running;
					break;

				case MovementStates.crouched:
					SetCapsule("crouching");
					SetSpeed(crouchSpeedModifer);
					currentMovementState = MovementStates.crouched;
					break;

				case MovementStates.crouchedWalking:
					SetCapsule("crouching");
					SetSpeed(crouchWalkSpeedModifer);
					currentMovementState = MovementStates.crouchedWalking;
					break;


				default:
					break;
			}
		}

		public MovementStates GetMovementState()
		{
			return currentMovementState;
		}

		private void SetCapsule(string type)
		{
			if (type.ToLower() == "standing")
			{
				if(playerCollider.transform.localScale == new Vector3(1, 0.5f, 1))
				{
					playerCollider.transform.localScale = new Vector3(1, 1, 1);
					playerCollider.transform.position = transform.position + new Vector3(0, 0.5f, 0);
				}
			}
			else if (type.ToLower() == "crouching")
			{
				if(playerCollider.transform.localScale == new Vector3(1, 1, 1))
				{
					playerCollider.transform.localScale = new Vector3(1, 0.5f, 1);
					playerCollider.transform.position = transform.position + new Vector3(0, -0.5f, 0);
				}
			}
		}
		private void SetSpeed(float multiplier)
		{
			currentMovementSpeed = baseMovementSpeed * multiplier;
		}

		public void OnWalk(InputAction.CallbackContext ctx)
		{
			bool isPressed = ctx.performed;

			if (isPressed)
			{
				if (currentMovementState == MovementStates.normal)
				{
					SetMovementMode(MovementStates.walking);
				}
				else if (currentMovementState == MovementStates.crouched)
				{
					SetMovementMode(MovementStates.crouchedWalking);
				}
				else
				{

				}
			}
			else
			{
				if (currentMovementState == MovementStates.walking)
				{
					SetMovementMode(MovementStates.normal);
				}
				else if (currentMovementState == MovementStates.crouchedWalking)
				{
					SetMovementMode(MovementStates.crouched);
				}
			}
		}

		public void OnRun(InputAction.CallbackContext ctx)
		{
			bool isPressed = ctx.performed;

			if (isPressed)
			{
				if (currentMovementState == MovementStates.normal)
				{
					SetMovementMode(MovementStates.running);
				}
			}
			else
			{
				if (currentMovementState == MovementStates.running)
				{
					SetMovementMode(MovementStates.normal);
				}
			}
		}

		public void OnJump(InputAction.CallbackContext ctx)
		{
			if (isGrounded)
			{
				playerRigidbody.AddForce(0, jumpForce, 0, ForceMode.Impulse);
			}
		}

		public void OnCrouch(InputAction.CallbackContext ctx)
		{
			if (currentMovementState == MovementStates.normal || currentMovementState == MovementStates.walking)
			{
				if (currentMovementState == MovementStates.normal)
				{
					SetMovementMode(MovementStates.crouched);
				}
				else if (currentMovementState == MovementStates.walking)
				{
					SetMovementMode(MovementStates.crouchedWalking);
				}
				else
				{

				}
			}
			else if (currentMovementState == MovementStates.crouched || currentMovementState == MovementStates.crouchedWalking)
			{
				if (crouchingBlocked)
				{
					Debug.Log("Cannot Stand Up Here!");
				}
				else 
				{
					if (currentMovementState == MovementStates.crouched)
					{
						SetMovementMode(MovementStates.normal);
					}
					else if (currentMovementState == MovementStates.crouchedWalking)
					{
						SetMovementMode(MovementStates.walking);
					}
				}
				
			}

		}


	}
}
