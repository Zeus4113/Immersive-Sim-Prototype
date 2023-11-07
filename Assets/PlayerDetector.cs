using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerDetector : MonoBehaviour
{

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject == null) return;

		if (other.gameObject.CompareTag("Player"))
		{
			RaycastManager raycastCheck = other.transform.GetChild(1).GetChild(0).GetComponent<RaycastManager>();
            raycastCheck.RegisterNewRaycast(this.transform);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject == null) return;

		if (other.gameObject.CompareTag("Player"))
		{
			RaycastManager raycastCheck = other.transform.GetChild(1).GetChild(0).GetComponent<RaycastManager>();
			raycastCheck.RemoveRegisteredRaycast(this.transform);

		}
	}
}
