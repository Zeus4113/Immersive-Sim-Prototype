using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HintPopup : MonoBehaviour
{
	private TMPro.TextMeshProUGUI m_TextMeshPro;
	private Image m_backgroundImage;

	private void Awake()
	{
		m_TextMeshPro = GetComponentInChildren<TMPro.TextMeshProUGUI>();
		m_backgroundImage = GetComponentInChildren<Image>();
	}

	public void DisplayHint(string hint)
	{
		if (hint == null || hint == "")
		{
			m_TextMeshPro.text = "";
			m_backgroundImage.enabled = false;
		}
		else
		{
			m_TextMeshPro.text = hint;
			m_backgroundImage.enabled = true;
		}
	}
}
