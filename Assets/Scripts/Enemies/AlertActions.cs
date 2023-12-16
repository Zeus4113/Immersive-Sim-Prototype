using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
	public class AlertActions : MonoBehaviour
	{

		[SerializeField] private Transform m_playerTransform;
		[SerializeField] private float m_attackCooldown = 1f;

		private NavMeshAgent m_agent;

		private void Start()
		{
			m_agent = GetComponent<NavMeshAgent>();
			StartMoving(m_playerTransform);
		}

		bool TrackingEnemy()
		{
			if (m_playerTransform == null) return false;
			return true;
		}

		bool c_isMoving = false;
		Coroutine c_moving;

		void StartMoving(Transform targetPosition)
		{
			if (c_isMoving) return;

			c_isMoving = true;

			if (c_moving != null) return;

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
			while (c_isMoving && TrackingEnemy())
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

		void AttackTarget(Collider other)
		{
			if (m_attackCooldown != 0) return;
			if(other == null) return;

			// Attack

			m_attackCooldown = 1f;
		}
	}
}

