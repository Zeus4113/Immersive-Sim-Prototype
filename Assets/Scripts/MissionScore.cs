using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionScore : MonoBehaviour
{
	private TMPro.TextMeshProUGUI m_TextMeshPro;

	private void Awake()
	{
		m_TextMeshPro = GetComponentInChildren<TMPro.TextMeshProUGUI>();
	}

	public void SetText(int amount)
	{
		m_TextMeshPro.text = amount.ToString();
	}
}
