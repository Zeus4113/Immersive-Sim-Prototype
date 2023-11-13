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
		normal,
		walking,
		running,
		crouched,
		crouchedWalking,
	}

	public class Movement : MonoBehaviour
	{

		// Player speed + modifier variables
		[SerializeField] private float baseMovementSpeed = 10f;
		[SerializeField] private float runSpeedModifier = 1.5f;
		[SerializeField] private float walkSpeedModifier = 0.5f;
		[SerializeField] private float crouchWalkSpeedModifer = 0.3f;
		[SerializeField] private float crouchSpeedModifer = 0.3f;

		// Player rotation sensitivity variable
		[SerializeField] private float mouseSensitivityPlayer = 0.8f;

		// Player jump variable
		[SerializeField] private float jumpForce = 100f;

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

			// Lock mouse cursor
			Cursor.lockState = CursorLockMode.Locked;

			// Set defaults
			SetMovementMode(MovementStates.normal);
		}

		private void Update()
		{
			transform.Rotate(new Vector3(0, playerRotationValue * mouseSensitivityPlayer, 0));
			transform.Translate(new Vector3(currentMovementValue.x, 0, currentMovementValue.y) * currentMovementSpeed * Time.deltaTime);
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

		void SetMovementMode(MovementStates currentState)
		{
			switch(currentState)
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

			//Debug.Log("Current State: " + currentMovementState);
		}

		public MovementStates GetMovementState()
		{
			return currentMovementState;
		}

		void SetCapsule(string type)
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
		void SetSpeed(float multiplier)
		{
			currentMovementSpeed = baseMovementSpeed * multiplier;
			//Debug.Log("Current Speed: " + currentMovementSpeed);
		}

		public void OnMovement(Vector2 inputValue)
		{
			currentMovementValue = inputValue;
		}

		public void OnRotation(Vector2 inputValue)
		{
			playerRotationValue = inputValue.x;
			cameraRotationValue = inputValue.y;
			RotateCamera(cameraRotationValue);
		}

		public void OnWalk(bool isPressed)
		{
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

		public void OnRun(bool isPressed)
		{
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

		public void OnJump()
		{
			if (isGrounded)
			{
				playerRigidbody.AddForce(0, jumpForce, 0, ForceMode.Impulse);
			}
		}

		public void OnCrouch()
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
