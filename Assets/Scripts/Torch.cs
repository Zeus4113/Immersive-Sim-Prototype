using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Torch : MonoBehaviour
{
    private Light realtimeComponent;
	private Light bakedComponent;
    private SphereCollider sphereCollider;
	private PlayerDetector playerDetector;

	private void Awake()
	{
		realtimeComponent = transform.Find("Realtime Light").GetComponent<Light>();
		bakedComponent = transform.Find("Baked Light").GetComponent<Light>();

		SetupLight(realtimeComponent, bakedComponent);

		sphereCollider = this.AddComponent<SphereCollider>();
		SetupCollider(sphereCollider);

		playerDetector = this.AddComponent<PlayerDetector>();
		playerDetector.SetParentTorchReference(this.transform);
    }

	private void SetupLight(Light realtimeLight, Light bakedLight)
	{
		realtimeLight.intensity = bakedLight.intensity;
		realtimeLight.range = bakedLight.range;
		realtimeLight.color = bakedLight.color;
	}

	private void SetupCollider(SphereCollider collider)
	{
		collider.isTrigger = true;
        collider.transform.position = transform.position;
        collider.radius = bakedComponent.range * 8;
	}
}
