using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraRotation : MonoBehaviour
{
	// Camera Settings
	[SerializeField] private float mouseSensitivityCamera;
	[SerializeField] private float minCameraRotation;
	[SerializeField] private float maxCameraRotation;

	void OnRotation(InputValue inputValue)
	{
		float CameraRotationValue = inputValue.Get<Vector2>().y;

		RotateCamera(CameraRotationValue);
	}

	private void RotateCamera(float inputCameraRotationValue)
	{
		Vector3 currentRotationalValue = transform.rotation.eulerAngles;
		float clampedValue = MathTools.ClampAngle(currentRotationalValue.x - (inputCameraRotationValue * mouseSensitivityCamera), minCameraRotation, maxCameraRotation);
		transform.localRotation = Quaternion.Euler(clampedValue, 0, 0);
	}
}
