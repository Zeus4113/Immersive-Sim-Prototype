using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisibilityMeter : MonoBehaviour
{
	private Image m_image;

	private void Awake()
	{
		m_image = transform.Find("Indicator").GetComponent<Image>();
	}

	public void SetIconAlpha(float newAlpha)
	{
		m_image.material.color = new Color(m_image.material.color.r, m_image.material.color.g, m_image.material.color.b, newAlpha);
	}
}
