using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
	public class PassiveActions : MonoBehaviour
	{
		private Transform m_patrolPathHolder;
		private float m_waitTime = 0.25f;
		private NavMeshAgent m_agent;

		public void Init(Transform pathHolder, float waitTime)
		{
			m_agent = GetComponent<NavMeshAgent>();
			m_patrolPathHolder = pathHolder;
			m_waitTime = waitTime;
		}

		// Patrol Coroutine

		bool c_isMoving = false;
		Coroutine c_moving;

		public void StartPatrol()
		{
			if (c_isMoving) return;
			
			c_isMoving = true;

			if (c_moving != null) return;

            //Debug.Log("Patrolling");
            c_moving = StartCoroutine(FollowPatrolPath(GetWaypoints()));
		}

		public void StopPatrol()
		{
			if (!c_isMoving) return;

			c_isMoving = false;

			if (c_moving == null) return;

			StopCoroutine(c_moving);
			c_moving = null;
		}

		IEnumerator FollowPatrolPath(Vector3[] waypoints)
		{
			//transform.position = waypoints[0];

			int targetWaypointIndex = 0;
			Vector3 targetWaypoint = waypoints[targetWaypointIndex];

			while (c_isMoving)
			{
                Debug.Log("Patrolling");
                //Debug.Log("P - Moving");
                m_agent.destination = targetWaypoint;

				if(transform.position.x == targetWaypoint.x && transform.position.z == targetWaypoint.z)
				{
					targetWaypointIndex = (targetWaypointIndex + 1) % waypoints.Length;
					targetWaypoint = waypoints[targetWaypointIndex];
					yield return new WaitForSeconds(m_waitTime);
				}

				yield return new WaitForFixedUpdate();
			}

			StopPatrol();
		}

		// Misc Functions

		private Vector3[] GetWaypoints()
		{
			Vector3[] waypoints = new Vector3[m_patrolPathHolder.childCount];

			for (int i = 0; i < waypoints.Length; i++)
			{
				waypoints[i] = m_patrolPathHolder.GetChild(i).position;
			}

			return waypoints;
		}
	}
}

