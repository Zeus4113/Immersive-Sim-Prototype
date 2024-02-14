using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
	public class Interaction : MonoBehaviour
	{
		public delegate void UpdateUI(GameObject target);
		public event UpdateUI UpdateIcon;

		private PlayerInput m_input;
		private Controller m_controller;
		private LayerMask m_mask;

		[SerializeField] private Transform m_pickupTransform;
		[SerializeField] private Transform m_dropTransform;

		public void Init(PlayerInput playerInput, Player.Controller controller)
		{
			m_input = playerInput;
			m_controller = controller;
			m_mask = LayerMask.GetMask("Interactables", "Environment");

			if (m_input != null)
			{
				m_input.actions.FindAction("Interact").performed += OnInteraction;
			}
		}

		void OnInteraction(InputAction.CallbackContext ctx)
		{
			if (m_pickedObject != null && m_isPickedUp)
			{
				m_pickedObject.Interact(this);
			}
			else
			{
				foreach (GameObject x in m_accessableInteractables)
				{
					if (x.GetComponent<IInteractable>() != null)
					{
						x.GetComponent<IInteractable>().Interact(this);
					}
				}
			}
		}

		public Transform GetPickedUpTransform()
		{
			return m_pickupTransform;
		}

		public Transform GetDroppedTransform()
		{
			return m_dropTransform;
		}



		bool m_isPickedUp = false;
		IInteractable m_pickedObject = null;

		public void OnPickedUp(GameObject myObject)
		{
			m_controller.GetMovement().IsPickedUp(true);
			m_isPickedUp = true;
			m_pickedObject = myObject.GetComponent<IInteractable>();
			UpdateIcon?.Invoke(myObject);
			StopChecking();
		}

		public void OnDropped()
		{
			m_controller.GetMovement().IsPickedUp(false);
			m_isPickedUp = false;
			m_pickedObject = null;
			UpdateIcon?.Invoke(null);
			StartChecking();
		}

		// Checking Coroutine

		bool c_isChecking = false;
		Coroutine c_checking;
		List<GameObject> m_interactables = new List<GameObject>();

		private void StartChecking()
		{
			if (c_isChecking) return;
			c_isChecking = true;

			if (c_checking != null) return;
			c_checking = StartCoroutine(CheckingInteractables());
		}

		private void StopChecking()
		{
			if (!c_isChecking) return;
			c_isChecking = false;

			if (c_checking == null) return;
			StopCoroutine(c_checking);
			c_checking = null;
		}

		List<GameObject> m_accessableInteractables = new List<GameObject>();

		IEnumerator CheckingInteractables()
		{
			while (c_isChecking && m_interactables.Count > 0)
			{
				foreach(GameObject obj in m_interactables)
				{
					RaycastHit hit;

					Physics.Raycast(transform.parent.position, obj.transform.position - transform.parent.position, out hit, Mathf.Infinity, m_mask);
					Debug.DrawRay(transform.parent.position, obj.transform.position - transform.parent.position, Color.green, 10f);

					if (hit.collider == obj.GetComponent<Collider>() && !m_accessableInteractables.Contains(obj)) m_accessableInteractables.Add(obj);
				}

				if(m_accessableInteractables.Count > 0) UpdateIcon?.Invoke(GetLowestDistance(m_accessableInteractables.ToArray()));
				else UpdateIcon?.Invoke(null);

				yield return new WaitForFixedUpdate();
			}

			UpdateIcon?.Invoke(null);
			StopChecking();
		}

		// Trigger Enter & Exit

		private void OnTriggerEnter(Collider other)
		{
			if (m_interactables.Contains(other.gameObject)) return;
			m_interactables.Add(other.gameObject);

			if(!m_isPickedUp) StartChecking();
		}

		private void OnTriggerExit(Collider other)
		{
			if (!m_interactables.Contains(other.gameObject)) return;
			m_interactables.Remove(other.gameObject);

			if (!m_accessableInteractables.Contains(other.gameObject)) return;
			m_accessableInteractables.Remove(other.gameObject);
		}

		// Extra Functions

		public Controller GetController()
		{
			return m_controller;
		}

		private GameObject GetLowestDistance(GameObject[] objects)
		{
			float[] distance = new float[objects.Length];

			for (int i = 0; i < objects.Length; i++)
			{
				distance[i] = Vector3.Distance(objects[i].transform.position, transform.position);
			}

			float value = float.PositiveInfinity;
			int index = -1;

			for (int i = 0; i < distance.Length; i++)
			{
				if (distance[i] < value)
				{
					index = i;
					value = distance[i];
				}
			}

			return objects[index];
		}
	}
}

