using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
	[SerializeField] private float m_onTime, m_offTime, m_flickerDuration;
	[SerializeField] private int m_flickerAmount;

	[SerializeField] private Light m_light;

	private void Awake()
	{
		StartCoroutine(Flicker());
	}

	IEnumerator Flicker()
	{
		while (true && m_light != null)
		{
			m_light.enabled = true;
			yield return new WaitForSeconds(m_onTime);

			for(int i = 0; i <= m_flickerAmount; i++)
			{
				m_light.enabled = !m_light.isActiveAndEnabled;
				yield return new WaitForSeconds(m_flickerDuration);
			}

			m_light.enabled = false;
			yield return new WaitForSeconds(m_offTime);

			for (int i = 0; i < m_flickerAmount; i++)
			{
				m_light.enabled = !m_light.isActiveAndEnabled;
				yield return new WaitForSeconds(m_flickerDuration);
			}

			yield return new WaitForFixedUpdate();
		}
	}
}
