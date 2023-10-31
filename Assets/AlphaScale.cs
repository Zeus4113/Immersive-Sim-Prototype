using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlphaScale : MonoBehaviour
{
	[SerializeField] private SpriteRenderer scaledSprite;

	public void ScaleAlpha(float targetAlpha)
	{
		Color32 spriteColor = scaledSprite.material.color;
		byte currentAlpha = spriteColor.a;

		scaledSprite.material.color = Color32.Lerp(new Color32(spriteColor.r, spriteColor.g, spriteColor.b, currentAlpha), new Color32(spriteColor.r, spriteColor.g, spriteColor.b, ((byte)targetAlpha)), 0.5f);


	}
}
