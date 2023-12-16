using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
	public class PassiveActions : MonoBehaviour
	{
		[SerializeField] private Transform m_patrolPathHolder;
		[SerializeField] private float m_waitTime = 0.25f;

		private NavMeshAgent m_agent;


		private void Start()
		{
			m_agent = GetComponent<NavMeshAgent>();
		}
		private void OnDrawGizmos()
		{
			Vector3 startPosition = m_patrolPathHolder.GetChild(0).position;
			Vector3 previousPosition = startPosition;

			foreach (Transform waypoint in m_patrolPathHolder)
			{
				Gizmos.DrawSphere(waypoint.position, .3f);
				Gizmos.DrawLine(previousPosition, waypoint.position);
				previousPosition = waypoint.position;
			}

			Gizmos.DrawLine(previousPosition, startPosition);
		}
		private Vector3[] GetWaypoints()
		{
			Vector3[] waypoints = new Vector3[m_patrolPathHolder.childCount];

			for (int i = 0; i < waypoints.Length; i++)
			{
				waypoints[i] = m_patrolPathHolder.GetChild(i).position;
			}

			return waypoints;
		}

		// Enemy passive behaviour

		public void EnemyPassive()
		{
			StartPatrol(GetWaypoints());
		}

		bool c_isMoving = false;
		Coroutine c_moving;

		void StartPatrol(Vector3[] waypoints)
		{
			if (c_isMoving) return;
			
			c_isMoving = true;

			if (c_moving != null) return;

			c_moving = StartCoroutine(FollowPatrolPath(waypoints));
		}

		void StopPatrol()
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

	}
}

