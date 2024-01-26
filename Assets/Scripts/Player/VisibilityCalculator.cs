using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class VisibilityCalculator : MonoBehaviour
{
	private Player.Controller m_controller;

	[SerializeField] private float raycastWeighting = 1f;
	[SerializeField] private float speedWeighting = 1f;
	[SerializeField] private floatEvent updateHud;

	[SerializeField] private bool m_isAwake = false;

	private Rigidbody m_rb;
	private List<PlayerDetector> lightSources = new List<PlayerDetector>();
	private float m_visibilityLevel = 0f;

	private void Awake()
	{
		if (m_isAwake)
		{
			m_rb = GetComponentInParent<Rigidbody>();
			StartCoroutine(CalculateVisibility());
		}
	}

	public void Init(Player.Controller controller)
	{
		m_controller = controller;
		m_rb = GetComponent<Rigidbody>();

		StartCoroutine(CalculateVisibility());
	}

	private IEnumerator CalculateVisibility()
	{
		float velocity;

		while (true)
		{
			velocity = m_rb.velocity.magnitude;
			m_visibilityLevel = DetermineTotalVisibility(DetermineRaycastVisibility(), velocity);

			if (!m_isAwake)
			{
				if (m_flashlightEnabled) updateHud.Invoke((m_visibilityLevel + 75) / 100);
				else updateHud.Invoke(m_visibilityLevel / 100);
			}

			yield return new WaitForSeconds(0.01f);
		}
	}

	public float GetVisibility()
	{
		return m_visibilityLevel;

    }

	public void AddLight(PlayerDetector lightSource)
	{
		lightSources.Add(lightSource);
	}

	public void RemoveLight(PlayerDetector lightSource)
	{
		lightSources.Remove(lightSource);
	}

	private float DetermineTotalVisibility(float raycastVisibility, float rigidbodySpeed)
	{
		float totalVisibility = 0;

		totalVisibility += (raycastVisibility * raycastWeighting);
		totalVisibility += (rigidbodySpeed * speedWeighting);

		totalVisibility = Mathf.Clamp(totalVisibility, 0, 100);

		return totalVisibility;
	}

	private float DetermineRaycastVisibility()
	{
		if(lightSources.Count == 0) return 0f;

		float visibilityTotal = 0f; 

		if(lightSources.Count > 0)
		{

			for (int i = 0; i < lightSources.Count; i++)
			{
				visibilityTotal += lightSources[i].GetPlayerVisibility();
			}

			visibilityTotal = Mathf.Round(Mathf.Clamp((visibilityTotal) * 100, 0, 100));
		}

		return visibilityTotal;
	}

	bool m_flashlightEnabled;

	public void FlashlightEnabled(bool isEnabled)
	{
		m_flashlightEnabled = isEnabled;
	}
}
