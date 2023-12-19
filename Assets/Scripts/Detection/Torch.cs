using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Torch : MonoBehaviour
{
    private Light lightComponent;
    private SphereCollider sphereCollider;
	private PlayerDetector playerDetector;

	private void Awake()
	{
        lightComponent = GetComponent<Light>();

		sphereCollider = this.AddComponent<SphereCollider>();
		SetupCollider(sphereCollider);

		playerDetector = this.AddComponent<PlayerDetector>();
		//playerDetector.SetParentTorchReference(this.transform);
    }

	private void SetupCollider(SphereCollider collider)
	{
		collider.isTrigger = true;
        collider.transform.position = transform.position;
        collider.radius = lightComponent.range * 8;
	}
}
