using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy;

public class SecurityAlarm : MonoBehaviour
{
	[Header("Floats")]
	[SerializeField] private float m_updateTime = 0.1f;
	[Space(2)]

	[Header("Data Sets")]
	[SerializeField] private PerceptionDataScriptableObject m_perceptionData;
	[Space(2)]

	[Header("References")]
	[SerializeField] private Perception m_perception;
	[SerializeField] private Light m_light;
	[SerializeField] private AudioSource m_audioSource;
	[SerializeField] private MeshRenderer m_renderer;
	[Space(2)]

	[Header("Materials")]
	[SerializeField] private Material m_greenEmissive;
	[SerializeField] private Material m_redEmissive;
	[Space(2)]

	[Header("Triggerables")]
	[SerializeField] private GameObject[] m_triggerables;

	bool c_isAlerted = false;
	Coroutine c_alerted;
	float m_alertTime = 5f;

	private void Awake()
	{
		m_perception.Init(m_perceptionData, m_updateTime);
		m_perception.perceptionAlerted += StartAlerted;

		m_perception.StartListening();

		m_light.enabled = false;
		m_renderer.material = m_greenEmissive;
		m_audioSource.Stop();
	}
	void TriggerObjects()
	{
		if (m_triggerables.Length > 0)
		{
			for (int i = 0; i < m_triggerables.Length; i++)
			{
				m_triggerables[i].GetComponent<ITriggerable>().Trigger();
			}
		}
	}

	void StartAlerted(float amount, Vector3 position)
	{
		m_alertTime = 5f;
		if (c_isAlerted) return;
		c_isAlerted = true;

		if (c_alerted != null) return;
		c_alerted = StartCoroutine(Alerted());
	}

	void StopAlerted()
	{
		if (!c_isAlerted) return;
		c_isAlerted = false;

		if (c_alerted == null) return;
		StopCoroutine(c_alerted);
		c_alerted = null;
	}

	IEnumerator Alerted()
	{
		m_light.enabled = true;
		m_renderer.material = m_redEmissive;
		m_audioSource.Play();
		TriggerObjects();

		while (c_isAlerted && m_alertTime > 0)
		{
			m_alertTime -= Time.fixedDeltaTime;
			if (m_alertTime < 0) m_alertTime = 0;

			yield return new WaitForFixedUpdate();
		}

		m_light.enabled = false;
		m_renderer.material = m_greenEmissive;
		m_audioSource.Stop();

		StopAlerted();

	}
}
