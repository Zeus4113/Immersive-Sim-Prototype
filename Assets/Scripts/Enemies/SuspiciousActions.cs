using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
	public class SuspiciousActions : MonoBehaviour
	{
		[SerializeField] private float m_lingerTime = 0.5f;
		[SerializeField] private Transform m_targetLocation;

		private NavMeshAgent m_agent;

		private void Start()
		{
			m_agent = GetComponent<NavMeshAgent>();
			EnemySuspicious(m_targetLocation.position);
		}
		public void EnemySuspicious(Vector3 target)
		{
			StartMoving(target);
		}
		private void OnEnable()
		{
			
		}
		private void OnDisable()
		{
			StopMoving();
			StopInvestigating();
		}

		// Moving Coroutine

		bool c_isMoving = false;
		Coroutine c_moving;

		void StartMoving(Vector3 targetLocation)
		{
			if (c_isMoving) return;

			c_isMoving = true;

			if (c_moving != null) return;

			c_moving = StartCoroutine(MoveToDisturbance(targetLocation));
		}

		void StopMoving()
		{
			if (!c_isMoving) return;

			c_isMoving = false;

			if (c_moving == null) return;

			StopCoroutine(c_moving);
			c_moving = null;
		}

		private IEnumerator MoveToDisturbance(Vector3 targetLocation)
		{
			while (c_isMoving)
			{
				m_agent.destination = targetLocation;

				if(transform.position.x == targetLocation.x && transform.position.z == targetLocation.z)
				{
					StartInvestigating(targetLocation);
					break;
				}

				yield return new WaitForFixedUpdate();
			}

			StopMoving();
		}

		// Investigating Coroutine

		bool c_isInvestigating = false;
		Coroutine c_investigating;

		void StartInvestigating(Vector3 targetArea)
		{
			if (c_isInvestigating) return;

			c_isInvestigating = true;

			if (c_investigating != null) return;

			c_investigating = StartCoroutine(InvestigateArea(targetArea));
		}

		void StopInvestigating()
		{
			if (!c_isInvestigating) return;

			c_isInvestigating = false;

			if (c_investigating == null) return;

			StopCoroutine(c_investigating);
			c_investigating = null;
		}

		private IEnumerator InvestigateArea(Vector3 targetArea)
		{
			Vector3 newPosition = targetArea + new Vector3(Random.insideUnitCircle.x * 5, targetArea.y, Random.insideUnitCircle.y * 5);
			NavMeshHit hit;

			while (c_isInvestigating)
			{
				NavMeshPath path = new NavMeshPath();
				m_agent.CalculatePath(newPosition, path);

				Debug.Log(path.status);

				if(path.status == NavMeshPathStatus.PathComplete)
				{
					m_agent.destination = newPosition;

					Debug.Log(Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(newPosition.x, newPosition.z)));

					if (Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(newPosition.x, newPosition.z)) < 0.5f)
					{
						newPosition = targetArea + new Vector3(Random.insideUnitCircle.x * 5, targetArea.y, Random.insideUnitCircle.y * 5);

						yield return new WaitForSeconds(m_lingerTime);
					}
				}
				else
				{
					newPosition = targetArea + new Vector3(Random.insideUnitCircle.x * 5, targetArea.y, Random.insideUnitCircle.y * 5);
				}

				yield return new WaitForFixedUpdate();
			}
		}
	}
}

