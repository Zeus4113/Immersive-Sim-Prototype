using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject == null) return;

		if (other.gameObject.CompareTag("Player"))
		{
			RaycastCheck playerChecker = other.gameObject.GetComponentInChildren<RaycastCheck>();
			playerChecker.TriggerCoroutine(this.transform, true);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject == null) return;

		if (other.gameObject.CompareTag("Player"))
		{
			RaycastCheck playerChecker = other.gameObject.GetComponentInChildren<RaycastCheck>();
			playerChecker.TriggerCoroutine(this.transform, false);
		}
	}
}
