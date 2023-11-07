using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Torch : MonoBehaviour
{
	[SerializeField] private float lightRadius = 0f;
	[SerializeField] private float intensity = 0f;

    private Light lightComponent;
    private SphereCollider sphereCollider;
	private PlayerDetector playerDetector;

	private void Awake()
	{
        lightComponent = GetComponentInChildren<Light>();
		SetupLight(lightComponent);

		sphereCollider = this.AddComponent<SphereCollider>();
		SetupCollider(sphereCollider);

		playerDetector = this.AddComponent<PlayerDetector>();
    }

	private void SetupLight(Light light)
	{
		light.intensity = intensity;
		light.range = lightRadius;
	}

	private void SetupCollider(SphereCollider collider)
	{
		collider.isTrigger = true;
        collider.transform.position = transform.position;
        collider.radius = lightRadius * 10;
	}
}
