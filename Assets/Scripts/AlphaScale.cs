using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlphaScale : MonoBehaviour
{
	[SerializeField] private Image image;
	public void ScaleAlpha(float targetAlpha)
	{
		Color32 spriteColor = image.color;

		image.color = new Color32(spriteColor.r, spriteColor.g, spriteColor.b, ((byte)targetAlpha));
	}
}
