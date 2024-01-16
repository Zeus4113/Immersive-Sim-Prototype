using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public delegate void DetectionEvent(GameObject detector);

public class Alarm : MonoBehaviour, IAlertable
{
	public event DetectionEvent alarmRaised;

	private float m_alertTime;

	[Header("References")]
	[SerializeField] private AudioSource m_audioSource;
	[SerializeField] private MeshRenderer m_renderer;
	[Space(2)]

	[Header("Materials")]
	[SerializeField] private Material m_redEmissive, m_greenEmissive;

	public void StartAlerted(float amount)
	{
		m_alertTime = amount;
		if (c_isAlarmed) return;
		c_isAlarmed = true;

		if (c_alarmed != null) return;
		c_alarmed = StartCoroutine(Alarmed());

		m_audioSource.Play();
		m_renderer.material = m_redEmissive;
	}

	public void StopAlerted()
	{
		if (!c_isAlarmed) return;
		c_isAlarmed = false;

		if (c_alarmed == null) return;
		StopCoroutine(c_alarmed);
		c_alarmed = null;

		m_audioSource.Stop();
		m_renderer.material = m_greenEmissive;
	}

	public void SetAlertTime(float amount)
	{
		m_alertTime = amount;
	}

	bool c_isAlarmed = false;
	Coroutine c_alarmed;

	IEnumerator Alarmed()
	{
		alarmRaised?.Invoke(this.gameObject);

		while(m_alertTime > 0)
		{
			m_alertTime -= Time.fixedDeltaTime;
			yield return new WaitForFixedUpdate();
		}
		StopAlerted();
	}
}
