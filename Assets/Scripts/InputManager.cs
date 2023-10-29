using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.Serialization;



[System.Serializable] public class Vector2Event : UnityEvent <Vector2> { }
[System.Serializable] public class BoolEvent : UnityEvent <bool> { }

public class InputManager : MonoBehaviour
{

	[SerializeField] private Vector2Event rotationInput;
	[SerializeField] private Vector2Event movementInput;

	[SerializeField] private UnityEvent crouchInput;
	[SerializeField] private UnityEvent jumpInput;

	[SerializeField] private BoolEvent walkInput;
	[SerializeField] private BoolEvent runInput;

	void OnJump()
	{
		jumpInput?.Invoke();
	}

	void OnCrouch()
	{
		crouchInput?.Invoke();
	}

	void OnWalk(InputValue inputValue)
	{
		bool outputBool = inputValue.isPressed;
		walkInput?.Invoke(outputBool);
	}

	void OnRun(InputValue inputValue)
	{
		bool outputBool = inputValue.isPressed;
		runInput?.Invoke(outputBool);
	}

	void OnRotation(InputValue inputValue)
	{
		Vector2 outputVector = inputValue.Get<Vector2>();
		rotationInput?.Invoke(outputVector);
	}

	void OnMovement(InputValue inputValue)
	{
		Vector2 outputVector = inputValue.Get<Vector2>();
		movementInput?.Invoke(outputVector);
	}
}
