using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
	public class Interaction : MonoBehaviour
	{

		public delegate void TriggerEvent(bool isTrue);
		public TriggerEvent IsPresent;

		private IInteractable m_overlappingInteractable = null;

		private PlayerInput m_input;
		private Controller m_controller;

		public Controller GetController()
		{
			return m_controller;
		}

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
			if (m_overlappingInteractable != null)
			{
				m_overlappingInteractable.Interact();
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			LayerMask layerMask = LayerMask.GetMask("Interactables");
			RaycastHit hit;
			Physics.Raycast(transform.parent.position, other.transform.position - transform.parent.position, out hit, 5f, layerMask);

			if (hit.collider == null) return;


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
}

