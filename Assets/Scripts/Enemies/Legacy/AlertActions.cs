using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
	public class AlertActions : MonoBehaviour
	{
		private float m_attackCooldown = 1f;
		private NavMeshAgent m_agent;

		public void Init(float cooldown)
		{
			m_agent = GetComponent<NavMeshAgent>();
			m_attackCooldown = cooldown;
		}

		public void StartAlerted(Transform player)
		{
			StartMoving(player);
		}

		public void StopAlerted()
		{
			StopMoving();
		}

		// Alerted Coroutine

		bool c_isMoving = false;
		Coroutine c_moving;

		void StartMoving(Transform targetPosition)
		{
			if (c_isMoving) return;

			c_isMoving = true;

			if (c_moving != null) return;

            Debug.Log("Pursuing target");
            c_moving = StartCoroutine(MoveToEnemy(targetPosition));
		}

		void StopMoving()
		{
			if (!c_isMoving) return;

			c_isMoving = false;

			if (c_moving == null) return;

			StopCoroutine(c_moving);
			c_moving = null;
		}

		IEnumerator MoveToEnemy(Transform targetPosition)
		{
			while (c_isMoving && targetPosition != null)
			{
				m_attackCooldown -= Time.fixedDeltaTime;
				if (m_attackCooldown < 0f) m_attackCooldown = 0f;

				m_agent.destination = targetPosition.position;

				RaycastHit hit;

				if (Vector3.Distance(transform.position, targetPosition.position) < 2f)
				{
					m_agent.isStopped = true;
					Physics.Raycast(transform.position, targetPosition.position - transform.position, out hit);

					if (hit.collider != null && hit.collider.CompareTag("Player"))
					{
						AttackTarget(hit.collider);
					}
				}
				else
				{
					m_agent.isStopped = false;
				}

				yield return new WaitForFixedUpdate();
			}

			StopMoving();
		} 

		// Attack Function

		void AttackTarget(Collider other)
		{
			if (m_attackCooldown != 0) return;
			if(other == null) return;

			// Attack

			m_attackCooldown = 1f;
		}
	}
}

