using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LightIndicator : MonoBehaviour
{
	private Image indicatorImage;

	private void Awake()
	{
		indicatorImage = GetComponent<Image>();
	}

	private void LerpImageColor(Color currentColor, Color targetColor, float duration )
	{
		indicatorImage.color = Color.Lerp(currentColor, targetColor, duration);
	}

	public void SetImageColor(float targetOpacity)
	{
		indicatorImage.color = new Color(indicatorImage.color.r, indicatorImage.color.g, indicatorImage.color.b, targetOpacity);
	}
}
