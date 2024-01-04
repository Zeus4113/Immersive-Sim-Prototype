using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

[ExecuteInEditMode]
public class Torch : MonoBehaviour
{
	[SerializeField] private float m_lightIntensity = 1f;
	[SerializeField] private float m_lightRange = 10f;
	[SerializeField] private float m_scaleDivider = 1f;

	[SerializeField] private Vector3 m_lightOffset;

    private Light lightComponent;
    private SphereCollider sphereCollider;
	private PlayerDetector playerDetector;

	private Transform m_lightHolder;

	private void OnEnable()
	{
		m_lightHolder = transform.Find("Components");

		lightComponent = m_lightHolder.GetComponent<Light>();
		sphereCollider = m_lightHolder.GetComponent<SphereCollider>();
		playerDetector = m_lightHolder.GetComponent<PlayerDetector>();

		if(sphereCollider != null)
		{
			SetupCollider(sphereCollider);
		}

		if(lightComponent != null)
		{
			//SetupLight(lightComponent);
		}

		if (lightComponent.type == UnityEngine.LightType.Point)
		{
			m_lightHolder.transform.localPosition = m_lightOffset * m_scaleDivider;
		}

		Debug.Log(gameObject.name + ": " + " Intensity - " + lightComponent.intensity + "  Intensity Value - " + m_lightIntensity);
	}

	private void SetupLight(Light light)
	{
		light.range = m_lightRange;
		light.intensity = m_lightIntensity;
	}

	private void SetupCollider(SphereCollider collider)
	{
		collider.isTrigger = true;
        collider.transform.position = transform.position;
        collider.radius = m_lightRange / m_scaleDivider;
	}
}
