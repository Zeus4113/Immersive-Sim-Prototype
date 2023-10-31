using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class LightBasedDetection : MonoBehaviour
{

	[SerializeField] private floatEvent visibilityReading;

	private LightProbeGroup storageRoomLightProbes;
	private SphericalHarmonicsL2[] probes = new SphericalHarmonicsL2[2];
	private Renderer playerMeshRenderer;

	private float visibilityPercentage;

	[SerializeField] private Transform startingProbeTransform;

	public float GetVisibility()
	{
		return visibilityPercentage;
	}

	private void Start()
	{
		playerMeshRenderer = GetComponent<Renderer>();
		StartCoroutine(CheckProbes());
	}

	private IEnumerator CheckProbes()
	{
		while (true)
		{
			Color resultTotal = new Color(0, 0, 0, 1);

			// Take detection readings
			for (int i = 0; i < 2; i++)
			{
				LightProbes.GetInterpolatedProbe(startingProbeTransform.position + new Vector3(0, -i, 0), playerMeshRenderer, out probes[i]);
				resultTotal += LightLevelAverage(probes[i]);
			}

			// Average probe data
			Color32 convertedColor = resultTotal / probes.Length;
			float[] colorComponent = new float[3];

			colorComponent[0] = convertedColor.r;
			colorComponent[1] = convertedColor.g;
			colorComponent[2] = convertedColor.b;

			float colorTotal = 0f;

			for(int i = 0; i < 3; i++)
			{
				Debug.Log(colorComponent[i]);
				colorTotal += colorComponent[i];
			}

			// Calculate visibility percentage
			colorTotal = colorTotal / colorComponent.Length;

			visibilityPercentage = (colorTotal / 255) * 100;

			visibilityReading?.Invoke(255 - colorTotal);

			yield return new WaitForSeconds(0.5f);
		}
	}

	private Color LightLevelAverage(SphericalHarmonicsL2 inputProbeData)
	{

		// Color and Vector arrays
		Vector3[] directions = new Vector3[]
		{
			// Up and down
			new Vector3(0.0f, 1.0f, 0.0f),
			new Vector3(0.0f, -1.0f, 0.0f),

			// Forwards and backwards
			new Vector3(1.0f, 0f, 0f),
			new Vector3(-1.0f, 0f, 0f),

			// Left and right
			new Vector3(0f, 0f, 1.0f),
			new Vector3(0f, 0f, -1.0f),

		};

		Color[] results = new Color[directions.Length];

		// Evaulate Harmonics
		inputProbeData.Evaluate(directions, results);

		// Average probe result
		Color averageResult = new Color(0,0,0,1);

		for (int i = 0; i < results.Length; i++)
		{
			averageResult += results[i];
		}

		averageResult = averageResult / results.Length;

		return averageResult;
	}
}
