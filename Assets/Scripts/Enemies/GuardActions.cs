 using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    public class GuardActions : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform m_patrolPathHolder;
        [SerializeField] private NavMeshAgent m_agent;
        [Space(2)]

        [Header("Floats")]
        [SerializeField] private float m_patrolWaitTime;
        [SerializeField] private float m_investigatingWaitTime;
        [SerializeField] private float m_attackDistance;
        [SerializeField] private float m_attackDamage;

        bool c_isPatrolling = false;
        Coroutine c_patrolling;

        bool c_isInvestigating = false;
        Coroutine c_investigating;

        bool c_isPursuing = false;
        Coroutine c_pursuing;

        private void StopActionCoroutines()
        {
            if(c_investigating != null) StopInvestigating();
            if (c_patrolling != null) StopPatrolling();
            if (c_pursuing != null) StopPursuing();
        }


        // Start & Stop Patrolling

        public void StartPatrolling()
        {
            if (c_isPatrolling) return;
            c_isPatrolling = true;

            if (c_patrolling != null) return;

            StopActionCoroutines();
            c_patrolling = StartCoroutine(Patrolling(GetWaypoints()));

            Debug.Log("Action - Patrolling");
        }

        public void StopPatrolling()
        {
            if (!c_isPatrolling) return;
            c_isPatrolling = false;

            if (c_patrolling == null) return;
            StopCoroutine(c_patrolling);
            c_patrolling = null;
        }

        // Start & Stop Investigating

        public void StartInvestigating(Vector3 target)
        {
            if (c_isInvestigating) return;
            c_isInvestigating = true;

            if (c_investigating != null) return;

            StopActionCoroutines();
            c_investigating = StartCoroutine(Investigating(target));

            Debug.Log("Action - Investigating");
        }

        public void StopInvestigating()
        {
            if (!c_isInvestigating) return;
            c_isInvestigating = false;

            if (c_investigating == null) return;
            StopCoroutine(c_investigating);
            c_investigating = null;
        }

        // Start & Stop Pursuing

        public void StartPursuing(Transform player)
        {
            if (c_isPursuing) return;
            c_isPursuing = true;

            if (c_pursuing != null) return;

            StopActionCoroutines();
            c_pursuing = StartCoroutine(Pursuing(player));

            Debug.Log("Action - Pursuing");
        }

        public void StopPursuing()
        {
            if (!c_isPursuing) return;
            c_isPursuing = false;

            if (c_pursuing == null) return;
            StopCoroutine(c_pursuing);
            c_pursuing = null;
        }

        // Action Coroutines

        IEnumerator Patrolling(Vector3[] waypoints)
        {
            int waypointIndex = 0;
            Vector3 nextPosition = waypoints[waypointIndex];

            while (c_isPatrolling)
            {
                m_agent.destination = nextPosition;

                if (Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(nextPosition.x, nextPosition.z)) < 0.25f)
                {
                    waypointIndex = (waypointIndex + 1) % waypoints.Length;
                    nextPosition = waypoints[waypointIndex];
                    yield return new WaitForSeconds(m_patrolWaitTime);
                }

                yield return new WaitForFixedUpdate();
            }
        }

        IEnumerator Investigating(Vector3 targetPosition)
        {
            Vector3 investigationPosition = targetPosition;

            Vector3[] pointsInArea = GetRandomPointsInArea(investigationPosition, 5, 2f);

            int pointsInAreaIndex = 0;

            while (c_isInvestigating)
            {
                m_agent.destination = investigationPosition;

                if (Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(investigationPosition.x, investigationPosition.z)) < 0.25f)
                {
                    pointsInAreaIndex = (pointsInAreaIndex + 1) % pointsInArea.Length;
                    investigationPosition = pointsInArea[pointsInAreaIndex];
                    yield return new WaitForSeconds(m_investigatingWaitTime);
                }

                yield return new WaitForFixedUpdate();
            }
        }

        IEnumerator Pursuing(Transform playerTransform)
        {
            bool trackingPlayer = true;
            Vector3 playerPosition = playerTransform.position;

            while (c_isPursuing && trackingPlayer)
            {
                if (playerTransform == null) trackingPlayer = false;

                playerPosition = playerTransform.position;
                m_agent.destination = playerPosition;

                if (Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(playerPosition.x, playerPosition.z)) < 0.25f)
                {
                    // Attack
                }

                yield return new WaitForFixedUpdate();
            }
        }

        Vector3[] GetWaypoints()
        {
            Vector3[] waypoints = new Vector3[m_patrolPathHolder.childCount];

            for (int i = 0; i < waypoints.Length; i++)
            {
                waypoints[i] = m_patrolPathHolder.GetChild(i).transform.position;
            }

            return waypoints;
        }

        Vector3[] GetRandomPointsInArea(Vector3 centre, int amount, float radius)
        {
            Vector3[] randomPoints = new Vector3[amount];

            for (int i = 0; i < amount; i++)
            {
                NavMeshPath path = new NavMeshPath();

                randomPoints[i] = centre + new Vector3(Random.insideUnitCircle.x * radius, centre.y, Random.insideUnitCircle.y * radius);
                m_agent.CalculatePath(randomPoints[i], path);

                Debug.Log(randomPoints[i]);

                while (path.status != NavMeshPathStatus.PathComplete)
                {
                    randomPoints[i] = centre + new Vector3(Random.insideUnitCircle.x * radius, centre.y, Random.insideUnitCircle.y * radius);
                    m_agent.CalculatePath(randomPoints[i], path);
                }
            }
            return randomPoints;
        }

        public void Attack(Transform target)
        {
            LayerMask mask = LayerMask.GetMask("Environment", "Player");

            RaycastHit hit;
            Physics.Raycast(transform.position, target.position - transform.position, out hit, m_attackDistance, mask);

            if (hit.collider == null) return;

            HealthComponent health = hit.collider.GetComponent<HealthComponent>();

            if(health != null)
            {
                health.Damage(m_attackDamage);
            }
        }
    }
}

