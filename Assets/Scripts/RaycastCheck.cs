using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastCheck : MonoBehaviour
{
	private Transform[] raycastOrigins = new Transform[3];
	private float playerVisibility;
	private float sensorCount;

	[SerializeField] private float intensityModifier = 0f;
	[SerializeField] private float distanceModifier = 0f;

	private void Start()
	{
		sensorCount = 3;
		for (int i = 0; i < raycastOrigins.Length; i++)
		{
			raycastOrigins[i] = transform.GetChild(i);
        }
	}

	public void OnCrouch()
	{
		if (sensorCount == 3)
		{
			sensorCount = 2;
		}
		else if(sensorCount == 2)
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
				if (hit.transform.CompareTag("Player")) { hitCount += 1; };
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
