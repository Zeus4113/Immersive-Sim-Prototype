using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
	private Torch torchRef;
	private float playerVisibility;

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject == null) return;

		if (other.gameObject.CompareTag("Player"))
		{
			StartCoroutine(OnPlayerPresent(other.gameObject));
			other.gameObject.GetComponent<VisibilityCalculator>().AddLight(this);
		}
		else { //Debug.Log("Player not found (Enter)"); 
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject == null) return;

		if (other.gameObject.CompareTag("Player"))
		{	
			StopAllCoroutines();
			other.gameObject.GetComponent<VisibilityCalculator>().RemoveLight(this);
		}
		else { //Debug.Log("Player not found (Exit)"); 
		}

	}

	private IEnumerator OnPlayerPresent(GameObject playerRef)
	{
		if (!playerRef.GetComponentInChildren<RaycastCheck>()) yield return null;

		RaycastCheck raycastCheck = playerRef.GetComponentInChildren<RaycastCheck>();

		while (true)
		{
			//torchRef.SetPlayerVisibility(raycastCheck.RaycastToLightSource(this.transform));

			playerVisibility = raycastCheck.RaycastToLightSource(this.transform);

			yield return new WaitForSeconds(0.1f);
		}
	}

	public void SetParentTorchReference(Transform gameObject)
	{
		torchRef = gameObject.GetComponent<Torch>();
	}

	public float GetPlayerVisibility()
	{
		return playerVisibility;
	}
}
