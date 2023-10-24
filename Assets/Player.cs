using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

namespace Player
{
	public class Movement : MonoBehaviour
	{
		private Vector2 currentMovementValue;
		private float currentMovementSpeed;
		private Axis currentRotationValue;

		[SerializeField] private float playerNormalSpeed = 10f;
		[SerializeField] private float playerFastSpeed = 15f;
		[SerializeField] private float playerSlowSpeed = 5f;

		[SerializeField] private float jumpForce = 100f;

		private Transform playerTransform;
		private Vector3 playerPosition;
		private Quaternion playerRotation;

		private Collider playerCollider;
		private MeshRenderer  playerMesh;
		private Rigidbody playerRigidbody;

		private Camera playerCamera;

		private PlayerInput playerInput;

		private void Start()
		{
			playerPosition = transform.position;
			playerInput = gameObject.GetComponent<PlayerInput>();
			playerCollider = gameObject.GetComponentInChildren<Collider>();
			playerMesh = gameObject.GetComponentInChildren<MeshRenderer>();
			playerRigidbody = gameObject.GetComponentInChildren<Rigidbody>();
			playerCamera = gameObject.GetComponentInChildren<Camera>();

			playerCamera.transform.parent = transform;

			currentMovementSpeed = playerNormalSpeed;

		}

		void OnJump()
		{
			playerRigidbody.AddForce(0, jumpForce, 0, ForceMode.Impulse);
		}

		void OnCrouch()
		{
			 
		}

		void OnMovement(InputValue inputValue)
		{
			currentMovementValue = inputValue.Get<Vector2>();
			Debug.Log("Current Movement:" + currentMovementValue.ToString());
		}

		void OnRotation(InputValue inputValue)
		{
			currentRotationValue = inputValue.Get<Axis>();
			Debug.Log("Current Rotation:" + currentRotationValue.ToString());
		}

		private void Update()
		{
			transform.Translate(new Vector3(currentMovementValue.x, transform.position.y, currentMovementValue.y) * currentMovementSpeed * Time.deltaTime);
			transform.Rotate(new Vector3(transform.rotation.x, transform.rotation.y + ((float)currentRotationValue), transform.rotation.x));
		}

	}
}
