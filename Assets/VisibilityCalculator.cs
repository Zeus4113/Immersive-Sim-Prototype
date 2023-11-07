using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibilityCalculator : MonoBehaviour
{
	private RaycastCheck playerRaycastCheck;
	private TextureDetector playerTextureDetector;

	private void Start()
	{
		playerRaycastCheck = GetComponentInChildren<RaycastCheck>();
		playerTextureDetector = GetComponentInChildren<TextureDetector>();
	}

	private void FixedUpdate()
	{
		DetermineVisibility();
	}

	private void DetermineVisibility()
	{
		if (playerRaycastCheck == null) return;
		if (playerTextureDetector == null) return;

		float visibilityTotal = 0f; 
		visibilityTotal += playerRaycastCheck.GetVisibility();

		Debug.Log("Raycast: " + playerRaycastCheck.GetVisibility());

		visibilityTotal += playerTextureDetector.GetVisibility();

		Debug.Log("Texture: " + playerTextureDetector.GetVisibility());

		float visibilityPercentage = Mathf.Round(Mathf.Clamp((visibilityTotal) * 100, 0, 100));

		Debug.Log("Total Visibility: " + visibilityPercentage + "%");
	}
}
