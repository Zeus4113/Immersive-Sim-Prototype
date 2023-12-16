using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RaycastCheck : MonoBehaviour
{

	[SerializeField] private PlayerInput m_playerInput;
	[Space(2)]

	[Header("Visibility Weighting")]
	[SerializeField] private float m_headSensorWeighting, m_torsoSensorWeighting, m_legSensorWeighting;
	[Space(2)]

	private float[] m_sensorWeightings;

	private Transform[] raycastOrigins = new Transform[3];
	private float playerVisibility;
	private float sensorCount;

	//[SerializeField] private float intensityModifier = 0f;
	//[SerializeField] private float distanceModifier = 0f;

	private void Start()
	{
		m_sensorWeightings = new float[3]{
			m_legSensorWeighting,
			m_torsoSensorWeighting,
			m_headSensorWeighting
		};

		sensorCount = m_sensorWeightings.Length;

		for (int i = 0; i < raycastOrigins.Length; i++)
		{
			raycastOrigins[i] = transform.GetChild(i);
        }

		if (m_playerInput != null) EnablePlayerInput(true);
	}

	void EnablePlayerInput(bool isTrue)
	{
		if (isTrue)
		{
			m_playerInput.actions.FindAction("Crouch").performed += OnCrouch;
			m_playerInput.actions.FindAction("Crouch").canceled += OnCrouch;
		}
		else if (!isTrue)
		{
			m_playerInput.actions.FindAction("Crouch").performed -= OnCrouch;
			m_playerInput.actions.FindAction("Crouch").canceled -= OnCrouch;
		}

	}

	public void OnCrouch(InputAction.CallbackContext ctx)
	{
		bool isCroutched = ctx.performed;

		if (isCroutched)
		{
			sensorCount = 2;
		}
		else if (!isCroutched)
		{
			sensorCount = 3;
		}
	}

	public float GetVisibility()
	{
		return playerVisibility;
	}

	public float RaycastToLightSource(Transform lightSource)
	{
		float hitCount = 0f;

		for (int i = 0; i < sensorCount; i++)
		{
			RaycastHit hit = new RaycastHit();
			LayerMask mask = LayerMask.GetMask("Player", "Environment");
			Physics.Raycast(lightSource.position, (raycastOrigins[i].position - lightSource.position).normalized, out hit);
			Debug.DrawLine(lightSource.position, raycastOrigins[i].position, Color.red, 0.1f);

			if (hit.collider != null)
			{
				if (hit.transform.CompareTag("Player")) { hitCount += (1 * m_sensorWeightings[i]); };
			}
		}

		float currentVisbility = (hitCount / sensorCount);
		float distance = Vector3.Distance(transform.position, lightSource.position);
		float intensity = lightSource.GetComponentInChildren<Light>().intensity;

		if (lightSource.GetComponentInChildren<Light>().type == LightType.Point)
		{
			distance = distance * distance;
			intensity = intensity / distance;

			currentVisbility = (currentVisbility * intensity);
			//Debug.Log(currentVisbility);
		}
		else if(lightSource.GetComponentInChildren<Light>().type == LightType.Spot)
		{
			currentVisbility = currentVisbility * intensity;
		}

		return currentVisbility;
	}
}
