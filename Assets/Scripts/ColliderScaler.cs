using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderScaler : MonoBehaviour
{
	private SphereCollider m_col;
	private Light m_light;

	private void Awake()
	{
		Transform componentHolder = transform.Find("Components");

		m_light = componentHolder.GetComponent<Light>();
		m_col = componentHolder.GetComponent<SphereCollider>();
		m_col.radius = m_light.range / transform.localScale.x;
	}
}
