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
		[SerializeField] private GroundedChecker m_groundedChecker;
		[SerializeField] private Rigidbody m_rigidbody;
		[SerializeField] private AudioSource m_audioSource;
		[Space(2)]

		[Header("Audio Clip Packs")]
		[SerializeField] private AudioClip[] m_floorClips;
		[SerializeField] private AudioClip[] m_woodClips;
		[SerializeField] private AudioClip[] m_carpetClips;
		[SerializeField] private AudioClip[] m_tileClips;
		[Space(2)]

		[Header("Floats")]
        [SerializeField] private float m_patrolWaitTime;
        [SerializeField] private float m_investigatingWaitTime;
        [SerializeField] private float m_attackDistance;
        [SerializeField] private float m_attackDamage;
		[SerializeField] private float m_reactionTime;

		[Header("Bools")]
		[SerializeField] private bool m_isReturner = false;

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
		[SerializeField] private Material m_whiteMaterial;

		bool c_isPatrolling = false;
        Coroutine c_patrolling;

        bool c_isInvestigating = false;
        Coroutine c_investigating;

        bool c_isPursuing = false;
        Coroutine c_pursuing;


		public void SetMeshColour(string colorName)
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
				case "white":
					mat = m_whiteMaterial;
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

		private void OnCollisionEnter(Collision collision)
		{
			if(collision.gameObject.layer == LayerMask.NameToLayer("Player"))
			{
				this.GetComponent<GuardBehaviour>().UpdateAlertLevel(100, collision.transform.position);
			}
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
			//m_audioSource.PlayOneShot(m_passiveClip);

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
			//m_audioSource.PlayOneShot(m_suspiciousClip);

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
			//m_audioSource.PlayOneShot(m_alertedClip);

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

        IEnumerator Patrolling(Transform[] waypoints)
        {
            int waypointIndex = 0;
			int increment = 1;
			Vector3 nextPosition = waypoints[waypointIndex].position;
			Waypoint nextWaypoint = waypoints[waypointIndex].GetComponent<Waypoint>();

			m_isVunerable = true;

			while (c_isPatrolling)
            {
				DrawGun(false);
				FootstepSounds();

				m_agent.destination = nextPosition;

                if (Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(nextPosition.x, nextPosition.z)) < 0.25f)
                {
					yield return new WaitForSeconds(nextWaypoint.GetWaitTime());

					if (m_isReturner)
					{
						if (waypointIndex >= waypoints.Length -1)
						{
							increment = -1;
						}
						else if(waypointIndex <= 0)
						{
							increment = 1;
						}
					}

					waypointIndex = (waypointIndex + increment) % waypoints.Length;
                    nextPosition = waypoints[waypointIndex].position;
					nextWaypoint = waypoints[waypointIndex].GetComponent<Waypoint>();
                }

                yield return new WaitForFixedUpdate();
            }
        }

		Vector3 m_investigationPosition = Vector3.zero;

        IEnumerator Investigating(Vector3 targetPosition)
        {
            Vector3 investigationPosition = targetPosition;
			m_investigationPosition = targetPosition;

			NavMeshPath navPath = new NavMeshPath();
			NavMeshHit hit;

			m_agent.CalculatePath(m_investigationPosition, navPath);
			NavMesh.SamplePosition(m_investigationPosition, out hit, 5f, NavMesh.AllAreas);

			if (navPath.status == NavMeshPathStatus.PathInvalid) m_investigationPosition = hit.position;

			Vector3[] pointsInArea = GetRandomPointsInArea(investigationPosition, 5, 2f);

            int pointsInAreaIndex = 0;

			yield return new WaitForSeconds(m_reactionTime);
			m_isVunerable = false;

			while (c_isInvestigating)
            {
				DrawGun(false);
				FootstepSounds();

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

			yield return new WaitForSeconds(m_reactionTime);
			m_isVunerable = false;

			while (c_isPursuing && trackingPlayer)
            {
				DrawGun(true);
				FootstepSounds();

				if (playerTransform == null) trackingPlayer = false;

				Vector3 playerPosition = playerTransform.position;

				NavMeshPath navPath = new NavMeshPath();

				m_agent.CalculatePath(playerPosition, navPath);

				if (navPath.status == NavMeshPathStatus.PathInvalid)
				{
					NavMeshHit hit;
					NavMesh.SamplePosition(playerPosition, out hit, 5f, NavMesh.AllAreas);
					playerPosition = hit.position;
					//Debug.Log("Sampled Position: " + playerPosition);
				}

				//Debug.Log("Player: " + playerPosition);

				m_agent.destination = playerPosition;

                yield return new WaitForFixedUpdate();
            }

			StopPursuing();
        }

        public void Attack(Transform target)
        {
            LayerMask mask = LayerMask.GetMask("Environment", "Player", "Interactables");

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

		bool m_isVunerable = false;

		public void GuardUnconcious(GameObject player)
		{
			GuardBehaviour behaviour = this.GetComponent<GuardBehaviour>();

			if (player == null)
			{
				Instantiate(m_deadBody, transform.position, transform.rotation);
				behaviour.GetManager().RemoveEnemy(gameObject);
				Destroy(gameObject);
			}
			else if(player != null)
			{
				if (m_isVunerable)
				{
					Instantiate(m_deadBody, transform.position, transform.rotation);
					behaviour.GetManager().RemoveEnemy(gameObject);
					Destroy(gameObject);
				}
				else if (!m_isVunerable)
				{
					this.GetComponent<GuardBehaviour>().UpdateAlertLevel(100, player.transform.position);
				}
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

		Transform[] GetWaypoints()
		{
			Transform[] waypoints = new Transform[m_patrolPathHolder.childCount];

			for (int i = 0; i < waypoints.Length; i++)
			{
				waypoints[i] = m_patrolPathHolder.GetChild(i).transform;
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
			this.GetComponent<GuardBehaviour>().UpdateAlertLevel(100, transform.position);

			yield return new WaitForSeconds(5f);

			GuardUnconcious(null);
			StopCountdown();
		}

		float m_steppingDistance = 0f;
		float m_stepDistance = 0.65f;

		void FootstepSounds()
		{
			Vector3 velocity = transform.InverseTransformDirection(m_agent.velocity);
			//Debug.Log(velocity);

			if (velocity.magnitude > 0.1f && m_groundedChecker.IsGrounded())
			{

				m_steppingDistance += velocity.magnitude * Time.fixedDeltaTime;

				//Debug.Log("Is Walking: " + m_steppingDistance);

				if (m_steppingDistance >= m_stepDistance)
				{
					m_steppingDistance = 0;
					m_audioSource.clip = DetermineAudioClip(m_groundedChecker.GetTag());
					m_audioSource.volume = (velocity.magnitude * 0.5f) * DetermineVolumeModifer(m_groundedChecker.GetTag());
					m_audioSource.Play();
					//Debug.Log("Footstep Sound Firing");
				}

			}
		}

		AudioClip DetermineAudioClip(string tag)
		{
			tag = tag.ToLower();

			switch (tag)
			{
				case "floor":

					return m_floorClips[Random.Range(0, m_floorClips.Length)];

				case "wood":

					return m_woodClips[Random.Range(0, m_woodClips.Length)]; ;

				case "carpet":

					return m_carpetClips[Random.Range(0, m_carpetClips.Length)]; ;

				case "tile":

					return m_tileClips[Random.Range(0, m_tileClips.Length)]; ;

				default:

					return null;
			}
		}

		float DetermineVolumeModifer(string tag)
		{
			tag = tag.ToLower();

			switch (tag)
			{
				case "floor":

					return 1f;

				case "wood":

					return 0.7f;

				case "carpet":

					return 0.35f;

				case "tile":

					return 1.5f;

				default:

					return 0;
			}
		}

	}


}

