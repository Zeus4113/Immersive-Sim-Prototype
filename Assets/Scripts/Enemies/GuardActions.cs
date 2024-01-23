 using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    public class GuardActions : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform m_patrolPathHolder;
        [SerializeField] private NavMeshAgent m_agent;
		[SerializeField] private GameObject m_deadBody;
        [Space(2)]

        [Header("Floats")]
        [SerializeField] private float m_patrolWaitTime;
        [SerializeField] private float m_investigatingWaitTime;
        [SerializeField] private float m_attackDistance;
        [SerializeField] private float m_attackDamage;

		[Header("Meshes")]
		[SerializeField] private MeshRenderer[] m_eyeMeshes;

		[Header("Gun")]
		[SerializeField] private Transform m_gunTransform;
		[SerializeField] private Transform m_holsterTransform;
		[SerializeField] private Transform m_heldTransform;
		[SerializeField] private ParticleSystem m_muzzleFlash;
		[SerializeField] private AudioSource m_gunSound;

		[Header("Materials")]
		[SerializeField] private Material m_redMaterial;
		[SerializeField] private Material m_yellowMaterial;
		[SerializeField] private Material m_greenMaterial;

        bool c_isPatrolling = false;
        Coroutine c_patrolling;

        bool c_isInvestigating = false;
        Coroutine c_investigating;

        bool c_isPursuing = false;
        Coroutine c_pursuing;


		private void SetMeshColour(string colorName)
		{
			Material mat = m_redMaterial;

			switch (colorName.ToLower())
			{
				case "red":
					mat = m_redMaterial;
					break;
				case "yellow":
					mat = m_yellowMaterial;
					break;
				case "green":
					mat = m_greenMaterial;
					break;
			}

			for(int i = 0; i < m_eyeMeshes.Length; i++)
			{
				m_eyeMeshes[i].material = mat;
			}
		}

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
			SetMeshColour("green");

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
			SetMeshColour("Yellow");

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
			SetMeshColour("red");

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
				DrawGun(false);
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

		Vector3 m_investigationPosition = Vector3.zero;

        IEnumerator Investigating(Vector3 targetPosition)
        {
            Vector3 investigationPosition = targetPosition;
			m_investigationPosition = targetPosition;

			Vector3[] pointsInArea = GetRandomPointsInArea(investigationPosition, 5, 2f);

            int pointsInAreaIndex = 0;

            while (c_isInvestigating)
            {
				DrawGun(false);

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
				DrawGun(true);

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

		void DrawGun(bool isDrawn)
		{
			if (isDrawn && m_gunTransform != m_heldTransform)
			{
				m_gunTransform.position = Vector3.Lerp(m_gunTransform.position, m_heldTransform.position, Time.fixedDeltaTime * 3);
				m_gunTransform.rotation = Quaternion.Lerp(m_gunTransform.rotation, m_heldTransform.rotation, Time.fixedDeltaTime * 3);
			}
			else if (!isDrawn && m_gunTransform != m_holsterTransform)
			{
				m_gunTransform.position = Vector3.Lerp(m_gunTransform.position, m_holsterTransform.position, Time.fixedDeltaTime * 3);
				m_gunTransform.rotation = Quaternion.Lerp(m_gunTransform.rotation, m_holsterTransform.rotation, Time.fixedDeltaTime * 3);
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

				int counter = 0;
				
                while (path.status != NavMeshPathStatus.PathComplete && counter < 100)
                {
					counter++;
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

            if(health != null && health.IsAlive())
            {
				m_muzzleFlash.Play();
				m_gunSound.Play();
				health.Damage(m_attackDamage);
            }
        }

		public Vector3 GetInvestigationPosition()
		{
			return m_investigationPosition;
		}

		public void GuardUnconcious()
		{
			if(this.GetComponent<GuardBehaviour>().GetAlertLevel() != AlertLevel.alerted)
			{
				Instantiate(m_deadBody, transform.position, transform.rotation);
				Destroy(gameObject);
			}
		}

		bool c_isCounting = false;
		Coroutine c_counting;

		public void StartCountdown()
		{
			if (c_isCounting) return;
			c_isCounting = true;

			if (c_counting != null) return;
			c_counting = StartCoroutine(Countdown());
		}

		public void StopCountdown()
		{
			if (!c_isCounting) return;
			c_isCounting = false;

			if (c_counting == null) return;
			StopCoroutine(c_counting);
			c_counting = null;
		}

		IEnumerator Countdown()
		{
			yield return new WaitForSeconds(5f);

			GuardUnconcious();
			StopCountdown();
		}

    }


}

