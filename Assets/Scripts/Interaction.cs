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
			foreach (GameObject x in m_interactables)
			{
				if(x != null)
				{
					x.GetComponent<IInteractable>().Interact(this);
				}
			}
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

		IEnumerator CheckingInteractables()
		{
			while (c_isChecking && m_interactables.Count > 0)
			{
				Debug.Log("Checking...");
				UpdateIcon?.Invoke(GetLowestDistance(m_interactables.ToArray()));

				yield return new WaitForFixedUpdate();
			}

			UpdateIcon?.Invoke(null);
			StopChecking();
		}

		// Trigger Enter & Exit

		private void OnTriggerEnter(Collider other)
		{
			if (m_interactables.Contains(other.gameObject)) return;

			LayerMask layerMask = LayerMask.GetMask("Interactables");
			RaycastHit hit;
			Physics.Raycast(transform.parent.position, other.transform.position - transform.parent.position, out hit, 5f, layerMask);

			if (hit.collider != other) return;
			m_interactables.Add(hit.collider.gameObject);
			StartChecking();

		}

		private void OnTriggerExit(Collider other)
		{
			if (!m_interactables.Contains(other.gameObject)) return;
			m_interactables.Remove(other.gameObject);
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

