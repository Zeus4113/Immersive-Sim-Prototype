using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundedChecker : MonoBehaviour
{
	Player.Movement movementComponent;

	private void Awake()
	{
		movementComponent = gameObject.GetComponentInParent<Player.Movement>();
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject != this.transform.parent.gameObject)
		{
			movementComponent.CheckGrounded(false);
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (other.gameObject != this.transform.parent.gameObject)
		{
			movementComponent.CheckGrounded(true);
		}
	}
}
