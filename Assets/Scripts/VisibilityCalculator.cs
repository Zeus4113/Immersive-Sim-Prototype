using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class VisibilityCalculator : MonoBehaviour
{
	[SerializeField] private float raycastWeighting = 1f;
	[SerializeField] private float textureWeighting = 1f;

	[SerializeField] private floatEvent updateHud;

	private RaycastCheck playerRaycastCheck;
	private TextureDetector playerTextureDetector;
	private List<PlayerDetector> lightSources = new List<PlayerDetector>();

	private void Start()
	{
		//playerRaycastCheck = GetComponentInChildren<RaycastCheck>();
		playerTextureDetector = GetComponentInChildren<TextureDetector>();
		StartCoroutine(CalculateVisibility());
	}

	private IEnumerator CalculateVisibility()
	{
		while (true)
		{
			float totalValue = DetermineTotalVisibility(DetermineRaycastVisibility(), DetermineTextureVisibility());

			float indicatorOpacity = 0f;
			indicatorOpacity = (totalValue / 100);
			updateHud.Invoke(indicatorOpacity);


			yield return new WaitForSeconds(0.01f);
		}
	}

	public void AddLight(PlayerDetector lightSource)
	{
		lightSources.Add(lightSource);
	}

	public void RemoveLight(PlayerDetector lightSource)
	{
		lightSources.Remove(lightSource);
	}

	private float DetermineTotalVisibility(float raycastVisibility, float textureVisibility)
	{
		float totalVisibility = 0;

		totalVisibility += (raycastVisibility * raycastWeighting);
		totalVisibility += (textureVisibility * textureWeighting);

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

			//Debug.Log(lightSources.Count);

			visibilityTotal = visibilityTotal / lightSources.Count;

			visibilityTotal = Mathf.Round(Mathf.Clamp((visibilityTotal) * 100, 0, 100));
		}

		return visibilityTotal;
	}

	private float DetermineTextureVisibility()
	{
		if (playerTextureDetector == null) return 0;

		float visibilityTotal = 0f;

		visibilityTotal += playerTextureDetector.GetVisibility();

		visibilityTotal = Mathf.Round(Mathf.Clamp((visibilityTotal) * 100, 0, 100));

		return visibilityTotal;
	}
}
