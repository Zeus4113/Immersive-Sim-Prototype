using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastCheck : MonoBehaviour
{
	private Transform[] raycastOrigins = new Transform[3];
	private float playerVisibility;
	private IEnumerator[] runningCoroutines = new IEnumerator[0];
	private Transform[] currentLightingObjects = new Transform[0];

	private void Start()
	{

		for(int i = 0; i < raycastOrigins.Length; i++)
		{
			raycastOrigins[i] = Instantiate(new GameObject().transform, (transform.position - new Vector3(0, (i / 2), 0)),Quaternion.identity);
			raycastOrigins[i].transform.SetParent(transform);

        }
	}

	public float GetVisibility()
	{
		return playerVisibility;
	}

	public void TriggerCoroutine(Transform lightSource, bool isTrue)
	{
		if (isTrue)
		{
			StartCoroutine(CheckRaycasts(lightSource, raycastOrigins));
		}
		else
		{
			StopAllCoroutines();
			playerVisibility = 0f;
		}
	}

	private IEnumerator CheckRaycasts(Transform lightSource, Transform[] originTransforms)
    {
		while (true)
		{
			float currentVisibility = 0f;

			for (int i = 0; i < originTransforms.Length; i++)
			{
				RaycastHit hit = new RaycastHit();
				LayerMask mask = LayerMask.GetMask("Player", "Environment");
				Physics.Raycast(lightSource.position, (originTransforms[i].position - lightSource.position).normalized, out hit);
				Debug.DrawLine(lightSource.position, (originTransforms[i].position - lightSource.position).normalized * 100f, Color.red, 0.1f);

				if(hit.collider != null)
				{
					if (hit.transform.CompareTag("Player")) { currentVisibility += 1; };
				}
			}

			playerVisibility = (currentVisibility / 3);

			yield return new WaitForSeconds(0.1f);
		}
	}
}
