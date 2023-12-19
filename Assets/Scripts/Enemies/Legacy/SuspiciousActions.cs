using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.UI.Image;

namespace Enemy
{
	public class SuspiciousActions : MonoBehaviour
	{
		private float m_lingerTime = 0.5f;

		private NavMeshAgent m_agent;

		public void Init(float lingerTime)
		{
			m_agent = GetComponent<NavMeshAgent>();
			m_lingerTime = lingerTime;
		}

		public void StartSuspicion(Vector3 target)
		{
			StartMoving(target);
		}

		public void StopSuspicion()
		{
			StopMoving();
			StopInvestigating();
		}

		// Moving Coroutine

		bool c_isMoving = false;
		Coroutine c_moving;

		public bool IsMoving()
		{
			return c_isMoving;
		}

		void StartMoving(Vector3 targetLocation)
		{
            if (c_isMoving) return;

			c_isMoving = true;

			if (c_moving != null) return;

            //Debug.Log("Moving to investigate");
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
			//Vector3 targetPosition = targetLocation;

			while (c_isMoving)
			{
				//Debug.Log("Moving to investigate" + targetLocation);
				Debug.Log("Target Location: " + targetLocation);
                m_agent.destination = targetLocation;
				Debug.Log("Move to position: " +  m_agent.destination);


				if(Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(targetLocation.x, targetLocation.z)) < 0.2f)
				{
					StartInvestigating(targetLocation);
					StopMoving();
                }
  
				yield return new WaitForFixedUpdate();
			}
		}

		// Investigating Coroutine

		bool c_isInvestigating = false;
		Coroutine c_investigating;

		void StartInvestigating(Vector3 targetArea)
		{
			if (c_isInvestigating) return;

			c_isInvestigating = true;

			if (c_investigating != null) return;

            Debug.Log("Investigating");
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
			Vector3 newPosition = GetRandomPositionInRadius(targetArea, 1);

            while (c_isInvestigating)
			{
				NavMeshPath path = new NavMeshPath();
				m_agent.CalculatePath(newPosition, path);

				Debug.Log(path.status);

				if(path.status == NavMeshPathStatus.PathComplete)
				{
                    m_agent.destination = newPosition;

					//Debug.Log(Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(newPosition.x, newPosition.z)));

					if (Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(newPosition.x, newPosition.z)) < 0.5f)
                    {
						newPosition = GetRandomPositionInRadius(targetArea, 1);

                        Debug.Log("Waiting");
                        yield return new WaitForSeconds(m_lingerTime);
					}
				}
				else if(path.status == NavMeshPathStatus.PathInvalid)
				{
					newPosition = GetRandomPositionInRadius(targetArea, 1);
                    Debug.Log("Generating New Vector3: " + newPosition);
					//Handles.DrawSolidDisc(newPosition, Vector3.up, 1f);
                }

				yield return new WaitForFixedUpdate();
			}
		}

		Vector3 GetRandomPositionInRadius(Vector3 origin, float radius)
		{
			Vector3 newPosition = origin + new Vector3((Random.insideUnitCircle.x * radius), origin.y, (Random.insideUnitCircle.y * radius));
			return newPosition;
        }
	}
}

