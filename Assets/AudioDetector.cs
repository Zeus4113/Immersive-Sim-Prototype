using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioDetector : MonoBehaviour
{
	private List<AudioSource> m_sourcesInRange;

	[Header("Detector Variables")]
	[SerializeField] private float m_threshold = 0.5f;
	[SerializeField] private float m_detectorResetTime = 5f;
	[Space(2)]

	[Header("Mesh Materials")]
	[SerializeField] private Material m_passiveMaterial;
	[SerializeField] private Material m_alertMaterial;
	[Space(2)]

	private MeshRenderer m_renderer;
	private float m_detectorTime;


	// Monobehaviour Functions

	private void Start()
	{
		m_sourcesInRange = new List<AudioSource>();
		m_renderer = GetComponent<MeshRenderer>();
		m_detectorTime = m_detectorResetTime;
		m_renderer.material = m_passiveMaterial;
	}

	private void OnTriggerEnter(Collider collision)
	{
		if (collision == null) return;

		if (collision.GetComponentInChildren<AudioSource>() == null) return;

		Debug.Log("Detected:" + gameObject.name);

		m_sourcesInRange.Add(collision.GetComponentInChildren<AudioSource>());
		StartCheckingSources();
	}

	private void OnTriggerExit(Collider other)
	{
		if (other == null) return;

		if (other.GetComponentInChildren<AudioSource>() == null) return;

		Debug.Log("Lost Detection:" + gameObject.name);

		m_sourcesInRange.Remove(other.GetComponentInChildren<AudioSource>());
	}


	// Check Audio Sources Coroutine

	bool c_isChecking = false;
	Coroutine c_checking;

	public void StartCheckingSources()
	{
		if (c_isChecking) return;

		c_isChecking = true;

		if (c_checking != null) return;

		c_checking = StartCoroutine(CheckSources());
	}

	public void StopCheckingSources()
	{
		if (!c_isChecking) return;

		c_isChecking = false;

		if (c_checking == null) return;

		StopCoroutine(c_checking);
		c_checking = null;
	}

	private IEnumerator CheckSources()
	{
		while(c_isChecking && m_sourcesInRange.Count > 0)
		{
			for(int i = 0; i < m_sourcesInRange.Count; i++)
			{
				AudioSource source = m_sourcesInRange[i];

				if (source.isPlaying)
				{
					float sourceVolume = source.volume;
					Debug.Log(sourceVolume);

					if (sourceVolume > m_threshold)
					{
						StartDetectorAlerted();
					}
				}
			}
			yield return new WaitForFixedUpdate();
		}
		StopCheckingSources();
	}

	// Detector Alerted Coroutine

	bool c_isAlerted = false;
	Coroutine c_alerted;

	private void StartDetectorAlerted()
	{
		m_detectorTime = 5f;

		if (c_isAlerted) return;

		c_isAlerted = true;

		if (c_alerted != null) return;

		c_alerted = StartCoroutine(DetectorAlerted());
	}

	private void StopDetectorAlerted()
	{
		if (!c_isAlerted) return;

		c_isAlerted = false;

		if (c_alerted == null) return;

		StopCoroutine(c_alerted);
		c_alerted = null;
	}

	private IEnumerator DetectorAlerted()
	{
		while (c_isAlerted)
		{
			m_renderer.material = m_alertMaterial;

			yield return new WaitForSeconds(1f);

			m_detectorTime--;

			if(m_detectorTime <= 0)
			{
				break;
			}
		}

		m_renderer.material = m_passiveMaterial;
		StopDetectorAlerted();
	}
}
