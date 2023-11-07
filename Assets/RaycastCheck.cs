using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastCheck : MonoBehaviour
{
	private Transform[] raycastOrigins;
	private float playerVisibility;

	private void Start()
	{
		playerVisibility = 0f;
		raycastOrigins = new Transform[transform.childCount];

		for(int i = 0; i < raycastOrigins.Length; i++)
		{
			raycastOrigins[i] = transform.GetChild(i);
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
			StartCoroutine(CheckRaycasts(lightSource));
		}
		else
		{
			StopAllCoroutines();
			playerVisibility = 0f;
		}
	}

	private IEnumerator CheckRaycasts(Transform lightSource)
	{
		while (true)
		{
			float currentVisibility = 0f;

			for (int i = 0; i < raycastOrigins.Length; i++)
			{
				RaycastHit hit = new RaycastHit();
				LayerMask mask = LayerMask.GetMask("Player", "Environment");
				Physics.Raycast(lightSource.position, (raycastOrigins[i].position - lightSource.position).normalized, out hit);
				Debug.DrawLine(lightSource.position, (raycastOrigins[i].position - lightSource.position).normalized * 100f, Color.red, 0.1f);

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
