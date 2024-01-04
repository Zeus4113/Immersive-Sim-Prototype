using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractTrigger : MonoBehaviour
{
	public delegate void TriggerEvent(bool isTrue);
	public TriggerEvent IsPresent;
	private IInteractable m_overlappingInteractable = null;

	public IInteractable GetInteractable()
	{
		return m_overlappingInteractable;
	}

	private void OnTriggerEnter(Collider other)
	{
		LayerMask layerMask = LayerMask.GetMask("Interactables");
		RaycastHit hit;
		Physics.Raycast(transform.parent.position, other.transform.position - transform.parent.position, out hit, 5f, layerMask);
		Debug.DrawRay(transform.parent.position, other.transform.position - transform.parent.position, Color.red, 5f);

		if(hit.collider == null) return;

		IInteractable interactable = hit.collider.GetComponent<IInteractable>();

		if (interactable == null) return;

		if (m_overlappingInteractable == interactable) return;

		if (hit.collider == other)
		{
			m_overlappingInteractable = other.GetComponent<IInteractable>();
		}

		Debug.Log(m_overlappingInteractable);

		IsPresent?.Invoke(true);


	}

	private void OnTriggerExit(Collider other)
	{
		if (other.GetComponent<IInteractable>() == m_overlappingInteractable)
		{
			m_overlappingInteractable = null;
			IsPresent?.Invoke(false);
		}
	}
}
