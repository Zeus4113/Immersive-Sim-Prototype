using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interact : MonoBehaviour
{
	[SerializeField] private InteractTrigger m_trigger;

	private PlayerInput m_input;
	private Player.Controller m_controller;

	public void Init(PlayerInput playerInput, Player.Controller controller)
	{
		m_input = playerInput;
		m_controller = controller;

		if (m_input != null)
		{
			m_input.actions.FindAction("Interact").performed += OnInteraction;
		}
	}

	void OnInteraction(InputAction.CallbackContext ctx)
	{
		Debug.Log("Triggered");

		if (m_trigger == null) return;

		IInteractable interactable = m_trigger.GetInteractable();

		if (interactable != null)
		{
			interactable.Interact();
		}
	}

	public InteractTrigger GetTrigger()
	{
		return m_trigger;
	}

}
