using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
	private TMPro.TextMeshProUGUI m_TextMeshPro;

	private void Awake()
	{
		m_TextMeshPro = GetComponentInChildren<TMPro.TextMeshProUGUI>();
	}

	public void SetText(float amount)
	{
		m_TextMeshPro.text = amount.ToString();
	}
}
