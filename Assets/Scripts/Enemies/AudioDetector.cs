using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class AudioDetector : MonoBehaviour, IBehaviourable
{
	private EnemyManager m_enemyManager;
	private List<AudioSource> m_sourcesInRange;

	[SerializeField] private GameObject[] m_triggerables;


	[Header("Detector Variables")]
	[SerializeField] private float m_threshold = 0.5f;
	[SerializeField] private float m_detectorResetTime = 5f;
	[SerializeField] private bool m_hasFalloff = true;
	[Space(2)]

	[Header("Mesh Materials")]
	[SerializeField] private Material m_passiveMaterial;
	[SerializeField] private Material m_alertMaterial;
	[Space(2)]

	[Header("Audio Clips")]
	[SerializeField] private AudioClip m_alertSound;

	private MeshRenderer m_renderer;
	private float m_detectorTime;
	private AudioSource m_source;


	// Monobehaviour Functions

	public void Init(EnemyManager em)
	{
		m_enemyManager = em;

        m_sourcesInRange = new List<AudioSource>();
		m_renderer = GetComponent<MeshRenderer>();
        m_source = GetComponent<AudioSource>();

		ResetBehaviour();
	}

	public void ResetBehaviour()
	{
        m_detectorTime = m_detectorResetTime;
        m_renderer.material = m_passiveMaterial;
        StopDetectorAlerted();
		StopThinking();
    }

	private void OnTriggerEnter(Collider collision)
	{
		if (collision == null) return;

		if (collision.GetComponentInChildren<AudioSource>() == null) return;

		Debug.Log("Detected:" + gameObject.name);

		m_sourcesInRange.Add(collision.GetComponentInChildren<AudioSource>());
        StartThinking();
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

	public void StartThinking()
	{
		if (c_isChecking) return;

		c_isChecking = true;

		if (c_checking != null) return;

		c_checking = StartCoroutine(CheckSources());
	}

	public void StopThinking()
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
					float distance = Vector3.Distance(transform.position, source.transform.position);

					if(m_hasFalloff)
					{
						sourceVolume = sourceVolume / distance;
                    }

					if (source.GetComponent<PlayerAudioManager>())
					{
						float modifer = source.GetComponent<PlayerAudioManager>().GetModifier();
						sourceVolume =  sourceVolume * modifer;
                    }

					if (sourceVolume > m_threshold)
					{
						StartDetectorAlerted();
					}

					Debug.Log("Volume: " + sourceVolume);
				}
			}
			yield return new WaitForFixedUpdate();
		}
		StopThinking();
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
        m_source.Play();

    }

	private void StopDetectorAlerted()
	{
		if (!c_isAlerted) return;

		c_isAlerted = false;

		if (c_alerted == null) return;

		StopCoroutine(c_alerted);
		c_alerted = null;
        m_source.Stop();

    }

	private IEnumerator DetectorAlerted()
	{
		AudioSource audioSource = GetComponent<AudioSource>();

        for(int i = 0; i < m_triggerables.Length; i++)
		{
			m_triggerables[i].GetComponent<IInteractable>().Interact();
		}


        while (c_isAlerted)
		{
			if (audioSource != null && !audioSource.isPlaying)
			{
				audioSource.Play();
			}

			m_renderer.material = m_alertMaterial;

			yield return new WaitForSeconds(1f);

			m_detectorTime--;

			if(m_detectorTime <= 0)
			{
				break;
			}
		}

		if (audioSource != null && audioSource.isPlaying)
		{
			audioSource.Stop();
		}

		m_renderer.material = m_passiveMaterial;
		StopDetectorAlerted();
	}
}
