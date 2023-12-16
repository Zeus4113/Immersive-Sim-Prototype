using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSensor : MonoBehaviour
{
	[SerializeField] private Material alertMaterial, passiveMaterial;
	[SerializeField] private float alertThreshold;
	private MeshRenderer sphereRenderer;

	private void Start()
	{
		sphereRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();
		sphereRenderer.material = passiveMaterial;
	}	

	private void ChangeMaterial(Material material)
	{
		sphereRenderer.material = material;
	}

	private void SoundDetected()
	{
		ChangeMaterial(alertMaterial);
		StopAllCoroutines();
		StartCoroutine(SleepTimer());
	}

	private IEnumerator SleepTimer()
	{
		yield return new WaitForSeconds(1f);
		ChangeMaterial(passiveMaterial);
		//yield return null;
	}

	public void isAlerted(float volume, float distance)
	{
		float alertLevel = volume / distance;

		Debug.Log(alertLevel);

		if (alertLevel >= alertThreshold)
		{
			SoundDetected();
		}
	}
}