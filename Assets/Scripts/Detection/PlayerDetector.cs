using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
	private float playerVisibility;
	private Transform m_playerTransform;

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject == null) return;

		if (other.gameObject.CompareTag("Player"))
		{
            m_playerTransform = other.gameObject.transform;
            StartPlayerPresent(m_playerTransform);
            m_playerTransform.GetComponent<VisibilityCalculator>().AddLight(this);
		}
		else { //Debug.Log("Player not found (Enter)"); 
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject == null) return;

		if (other.gameObject.CompareTag("Player"))
		{
            StopPlayerPresent();
            m_playerTransform.GetComponent<VisibilityCalculator>().RemoveLight(this);
			m_playerTransform = null;
        }
		else { //Debug.Log("Player not found (Exit)"); 
		}

	}

	private void OnDestroy()
	{
		if (m_playerTransform == null) return;

		m_playerTransform.GetComponent<VisibilityCalculator>().RemoveLight(this);
		m_playerTransform = null;
	}

	bool c_isPresent = false;
	Coroutine c_present;

	void StartPlayerPresent(Transform PlayerRef)
	{
		if (c_isPresent) return;
		c_isPresent = true;

		if (c_present != null) return;
		c_present = StartCoroutine(OnPlayerPresent());

	}

	void StopPlayerPresent()
	{
        if (!c_isPresent) return;
        c_isPresent = false;

        if (c_present == null) return;
        StopCoroutine(c_present);
		c_present = null;

    }

    private IEnumerator OnPlayerPresent()
	{
		if (!m_playerTransform.GetComponentInChildren<RaycastCheck>()) yield return null;

		RaycastCheck raycastCheck = m_playerTransform.GetComponentInChildren<RaycastCheck>();

		while (c_isPresent)
		{
            playerVisibility = raycastCheck.RaycastToLightSource(this.transform);

			yield return new WaitForSeconds(0.1f);
		}

		StopPlayerPresent();
	}

	public float GetPlayerVisibility()
	{
		return playerVisibility;
	}

	public Transform GetPlayerTransform()
	{
		return m_playerTransform;
	}
}
