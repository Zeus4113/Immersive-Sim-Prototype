using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class LightBasedDetection : MonoBehaviour
{

	[SerializeField] private floatEvent visibilityReading;

	private LightProbeGroup storageRoomLightProbes;
	private SphericalHarmonicsL2[] probes = new SphericalHarmonicsL2[6];
	private MeshRenderer playerMeshRenderer;

	private float visibilityPercentage;

	[SerializeField] private Transform startingProbeTransform;

	[SerializeField] private Transform[] probePositions;

	public float GetVisibility()
	{
		return visibilityPercentage;
	}

	private void Start()
	{
		playerMeshRenderer = transform.GetChild(0).GetComponentInChildren<MeshRenderer>();
		StartCoroutine(CheckProbes());
	}

	private IEnumerator CheckProbes()
	{
		while (true)
		{
			Color resultTotal = new Color(0, 0, 0, 1);

			Vector3[] probeLocations = new Vector3[]
			{
				// Y Axis
				new Vector3(0,1,0),
				new Vector3(0,-1,0),

				// X Axis
				new Vector3(0.5f,0,0),
				new Vector3(-0.5f,0,0),

				// Z Axis
				new Vector3(0,0,0.5f),
				new Vector3(0,0,-0.5f),
			};




			// Take detection readings
			for (int i = 0; i < probeLocations.Length; i++)
			{
				LightProbes.GetInterpolatedProbe(startingProbeTransform.position + probeLocations[i], playerMeshRenderer, out probes[i]);
				resultTotal += LightLevelAverage(probes[i], i);
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
				colorTotal += colorComponent[i];
			}

			// Calculate visibility percentage
			colorTotal = (colorTotal / colorComponent.Length) * 3;
			colorTotal = (colorTotal / 255) * 100;

			visibilityPercentage = Mathf.Clamp(colorTotal, 0f, 100f);

			//visibilityReading?.Invoke(255 - colorTotal);

			Debug.Log("Visibility Percentage: " + visibilityPercentage + "%");

			yield return new WaitForSeconds(1f);
		}
	}

	private Color LightLevelAverage(SphericalHarmonicsL2 inputProbeData, int index)
	{
		Vector3[] directions = new Vector3[5];

		switch (index)
		{
			case 0:
				directions[0] = new Vector3(0.0f, 1.0f, 0.0f);
				directions[1] = new Vector3(0.5f, 0, 0);
				directions[2] = new Vector3(-0.5f, 0, 0);
				directions[3] = new Vector3(0.0f, 0.0f, 0.5f);
				directions[4] = new Vector3(0.0f, 0.0f, -0.5f);
				break;
			case 1:
				directions[0] = new Vector3(0.0f, -1.0f, 0.0f);
				directions[1] = new Vector3(0.5f, 0, 0);
				directions[2] = new Vector3(-0.5f, 0, 0);
				directions[3] = new Vector3(0.0f, 0.0f, 0.5f);
				directions[4] = new Vector3(0.0f, 0.0f, -0.5f);
				break;
			case 2:
				directions[0] = new Vector3(0.0f, 1.0f, 0.0f);
				directions[1] = new Vector3(0.0f, -1.0f, 0.0f);
				directions[2] = new Vector3(0.5f, 0, 0);
				directions[3] = new Vector3(0.0f, 0.0f, 0.5f);
				directions[4] = new Vector3(0.0f, 0.0f, -0.5f);
				break;
			case 3:
				directions[0] = new Vector3(0.0f, 1.0f, 0.0f);
				directions[1] = new Vector3(0.0f, -1.0f, 0.0f);
				directions[2] = new Vector3(-0.5f, 0, 0);
				directions[3] = new Vector3(0.0f, 0.0f, 0.5f);
				directions[4] = new Vector3(0.0f, 0.0f, -0.5f);
				break;
			case 4:
				directions[0] = new Vector3(0.0f, 1.0f, 0.0f);
				directions[1] = new Vector3(0.0f, -1.0f, 0.0f);
				directions[2] = new Vector3(0.5f, 0, 0);
				directions[3] = new Vector3(-0.5f, 0, 0);
				directions[4] = new Vector3(0.0f, 0.0f, 0.5f);
				break;
			case 5:
				directions[0] = new Vector3(0.0f, 1.0f, 0.0f);
				directions[1] = new Vector3(0.0f, -1.0f, 0.0f);
				directions[2] = new Vector3(0.5f, 0, 0);
				directions[3] = new Vector3(-0.5f, 0, 0);
				directions[4] = new Vector3(0.0f, 0.0f, -0.5f);
				break;
			default:
				break;
		}

		//// Color and Vector arrays
		//Vector3[] directions = new Vector3[]
		//{
		//	// Up and down
		//	new Vector3(0.0f, 1.0f, 0.0f),
		//	new Vector3(0.0f, -1.0f, 0.0f),

		//	// Forwards and backwards
		//	new Vector3(1.0f, 0f, 0f),
		//	new Vector3(-1.0f, 0f, 0f),

		//	// Left and right
		//	new Vector3(0f, 0f, 1.0f),
		//	new Vector3(0f, 0f, -1.0f),

		//};

		Color[] results = new Color[directions.Length];

		// Evaulate Harmonics
		inputProbeData.Evaluate(directions, results);

		for(int i = 0; i < results.Length; i++)
		{
			Debug.Log(results[i]);
		}


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
