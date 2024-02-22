using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    public enum AlertLevel
    {
        passive,
        suspicious,
        alerted,
    }

    public class GuardBehaviour : MonoBehaviour, IAlertable
    {
		public event IAlertable.Alert alertTriggered;

		[SerializeField] private float m_alertFalloffModifier = 1f;
		[SerializeField] private float m_attackRange = 5f;
		[SerializeField] private float m_attackCooldown = 5f;
		[SerializeField] private float m_stunTime = 5f;
		[SerializeField] private ParticleSystem m_particleSystem;

		[Header("Audio")]
		[SerializeField] private AudioClip m_suspiciousClip, m_passiveClip, m_alertedClip;

		private EnemyManager m_enemyManager;
		private NavMeshAgent m_agent;
        private GuardActions m_actions;
        private Perception m_perception;
        private float m_alertFloat;
        private Vector3 m_alertLocation;
		private Rigidbody m_rb;
		private AudioSource m_audioSource;

		[SerializeField] private PerceptionDataScriptableObject m_data;

        public void Init(EnemyManager em)
        {

			m_enemyManager = em;
			m_alertLocation = Vector3.zero;
			m_alertFloat = 0f;
			c_isThinking = false;

            m_agent = GetComponent<NavMeshAgent>();
            m_actions = GetComponent<GuardActions>();
            m_perception = GetComponent<Perception>();
			m_audioSource = GetComponent<AudioSource>();

			m_perception.Init(m_data);
			//m_perception.StartLooking();
			//m_perception.StartListening();

            m_perception.perceptionAlerted += UpdateAlertLevel;
			m_perception.perceptionTick += TickAlertLevel;

			StartAlerted();
        }

		public EnemyManager GetManager()
		{
			return m_enemyManager;
		}

		private void OnCollisionEnter(Collision collision)
		{
			if (collision == null) return;
			
			if(collision.gameObject.layer == LayerMask.NameToLayer("Player"))
			{
				UpdateAlertLevel(100, collision.transform.position);
			}
		}

		public AlertLevel GetAlertLevel()
		{
			return DetemineAlertLevel();
		}

		bool c_isThinking = false;
		Coroutine c_thinking;

		public void StartAlerted()
		{
			if (c_isThinking) return;
			c_isThinking = true;

			if (c_thinking != null) return;
			c_thinking = StartCoroutine(Thinking());
		}

		public void StopAlerted()
		{
			if (!c_isThinking) return;
			c_isThinking = false;

			if (c_thinking == null) return;
			StopCoroutine(c_thinking);
			c_thinking = null;
		}

		IEnumerator Thinking()
        {
			AlertLevel oldAlertLevel = AlertLevel.passive;
			Vector3 investigationPos = Vector3.zero;
			float attackCooldown = 0f;

			while (c_isThinking)
			{
				if (m_isStunned)
				{
					m_actions.StopPatrolling();
					m_actions.StopInvestigating();
					m_actions.StopPursuing();

					m_actions.SetMeshColour("white");
					UpdateAlertLevel(100, transform.position);
					//m_actions.StopAllCoroutines();
					m_perception.StopLooking();
					m_perception.StopListening();
					m_agent.isStopped = true;
				}

				else if (!m_isStunned)
				{
					m_perception.StartLooking();
					m_perception.StartListening();

                    switch (DetemineAlertLevel())
					{

						case AlertLevel.passive:

							if (oldAlertLevel != AlertLevel.passive)
							{
								m_audioSource.PlayOneShot(m_passiveClip, 1f);
							}

							oldAlertLevel = AlertLevel.passive;
							m_agent.speed = 1f;
							m_agent.isStopped = false;
							m_actions.StartPatrolling();

							break;

						case AlertLevel.suspicious:

							if (oldAlertLevel != AlertLevel.suspicious)
							{
								m_audioSource.PlayOneShot(m_suspiciousClip, 0.5f);
								Debug.Log("Clip Played");
							}


							//Debug.Log("Suspicious");
							oldAlertLevel = AlertLevel.suspicious;
							m_agent.speed = 2f;
							m_agent.isStopped = false;

							if (m_alertLocation != Vector3.zero)
							{
								m_actions.StartInvestigating(m_alertLocation);

								if (m_actions.GetInvestigationPosition() != m_alertLocation)
								{
									m_actions.StopInvestigating();
									m_actions.StartInvestigating(m_alertLocation);
								}
							}

							break;

						case AlertLevel.alerted:

							if (oldAlertLevel != AlertLevel.alerted)
							{
								alertTriggered?.Invoke(this.gameObject);
								m_audioSource.PlayOneShot(m_alertedClip, 0.75f);
							}

							oldAlertLevel = AlertLevel.alerted;
							m_agent.speed = 3f;

							if (m_perception.GetPlayerRef() != null)
							{
								m_actions.StartPursuing(m_perception.GetPlayerRef());

								if (Vector3.Distance(m_perception.GetPlayerRef().position, transform.position) < m_attackRange)
								{
									m_agent.isStopped = true;
									m_agent.transform.LookAt(new Vector3(m_perception.GetPlayerRef().position.x, transform.position.y, m_perception.GetPlayerRef().position.z));

									if (attackCooldown <= 0)
									{
										m_actions.Attack(m_perception.GetPlayerRef());
										attackCooldown = m_attackCooldown;
									}
								}
								else
								{
									m_agent.isStopped = false;
								}
							}
							else if (m_alertLocation != Vector3.zero)
							{
								m_actions.StartInvestigating(m_alertLocation);

								if (m_actions.GetInvestigationPosition() != m_alertLocation)
								{
									m_actions.StopInvestigating();
									m_actions.StartInvestigating(m_alertLocation);
								}
							}

							break;
					}

					m_alertFloat -= (Time.fixedDeltaTime * m_alertFalloffModifier);
					m_alertFloat = Mathf.Clamp(m_alertFloat, 0, 100);
				}

				if(attackCooldown > 0)
				{
                    attackCooldown -= Time.fixedDeltaTime;
                }

				yield return new WaitForFixedUpdate();
			}

			m_perception.StopLooking();
			StopAlerted();
        }

		bool m_isStunned = false;

		public void SetStunned()
		{
			if (m_isStunned) return;
			m_isStunned = true;

			Debug.Log(gameObject.name + " is Stunned");

			StartCoroutine(StunDuration());
		}

		IEnumerator StunDuration()
		{
			m_particleSystem.Play();

            yield return new WaitForSeconds(m_stunTime);

			m_particleSystem.Stop();

            m_isStunned = false;
		}

        private AlertLevel DetemineAlertLevel()
        {

			m_alertFloat = Mathf.Clamp(m_alertFloat, 0, 100);

            switch (m_alertFloat)
            {
                default:
                    return AlertLevel.passive;

                case float x when x == 0:
                    return AlertLevel.passive;

                case float x when x > 0 && x <= 60:
                    return AlertLevel.suspicious;

                case float x when x > 60 && x <= 100:
                    return AlertLevel.alerted;
            }
        }

        public void UpdateAlertLevel(float amount, Vector3 position)
        {
			if(amount > m_alertFloat) m_alertFloat = amount;
            if(position != Vector3.zero) m_alertLocation = position;
        }

		private void TickAlertLevel(float amount, Vector3 position)
		{
			m_alertFloat += amount;
			m_alertLocation = position;
		}
	}
}
