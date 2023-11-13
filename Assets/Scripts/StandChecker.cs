using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandChecker : MonoBehaviour
{
	Player.Movement movementComponent;

	private void Awake()
	{
		movementComponent = gameObject.GetComponentInParent<Player.Movement>();
	}

	private void OnTriggerExit(Collider other)
	{
		if(!other.gameObject.CompareTag("Player"))
		{
			movementComponent.CrouchingBlocked(false);
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (!other.gameObject.CompareTag("Player"))
		{
			movementComponent.CrouchingBlocked(true);
		}
	}
}
