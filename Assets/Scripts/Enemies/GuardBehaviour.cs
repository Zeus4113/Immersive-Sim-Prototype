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

    public class GuardBehaviour : MonoBehaviour, IBehaviourable
    {
		[SerializeField] private float m_alertFalloffModifier = 1f;
		[SerializeField] private float m_attackRange = 5f;
		[SerializeField] private float m_attackCooldown = 5f;

		private EnemyManager m_enemyManager;
		private NavMeshAgent m_agent;

        private GuardActions m_actions;
        private GuardPerception m_perception;
        private float m_alertFloat;
        private Vector3 m_alertLocation;

        public void Init(EnemyManager em)
        {
			Debug.Log(gameObject.name + " Initialised");

			m_enemyManager = em;
			m_alertLocation = Vector3.zero;
			m_alertFloat = 0f;
			c_isThinking = false;

            m_agent = GetComponent<NavMeshAgent>();
            m_actions = GetComponent<GuardActions>();
            m_perception = GetComponent<GuardPerception>();
            m_perception.sightAlerted += UpdateAlertLevel;
        }

		public void ResetBehaviour()
		{
			m_alertLocation = Vector3.zero;
			m_alertFloat = 0f;
			c_isThinking = false;
		}

		bool c_isThinking = false;
		Coroutine c_thinking;

		public void StartThinking()
		{
			if (c_isThinking) return;
			c_isThinking = true;

			if (c_thinking != null) return;
			c_thinking = StartCoroutine(Thinking());
		}

		public void StopThinking()
		{
			if (!c_isThinking) return;
			c_isThinking = false;

			if (c_thinking == null) return;
			StopCoroutine(c_thinking);
			c_thinking = null;
		}

		IEnumerator Thinking()
        {
			float attackCooldown = 0f;
			m_perception.StartLooking();

			while (c_isThinking)
			{
				m_agent.isStopped = false;

                switch (DetemineAlertLevel())
				{
					case AlertLevel.passive:

                        m_agent.speed = 2f;
                        m_actions.StartPatrolling();

						break;

					case AlertLevel.suspicious:

						m_agent.speed = 2.5f;

						if (m_alertLocation != Vector3.zero)
						{
							m_actions.StartInvestigating(m_alertLocation);
						}

						break;

					case AlertLevel.alerted:

                        m_agent.speed = 3f;

                        if (m_perception.GetPlayerRef() != null)
						{
							m_actions.StartPursuing(m_perception.GetPlayerRef());

                            if (Vector3.Distance(m_perception.GetPlayerRef().position, transform.position) < m_attackRange && attackCooldown <= 0)
							{
								m_agent.isStopped = true;
                                m_actions.Attack(m_perception.GetPlayerRef());
								attackCooldown = m_attackCooldown;
							}


                        }
						else if (m_alertLocation != Vector3.zero)
						{
							m_actions.StartInvestigating(m_alertLocation);
						}

						break;
				}

				m_alertFloat -= (Time.fixedDeltaTime * m_alertFalloffModifier);
				m_alertFloat = Mathf.Clamp(m_alertFloat, 0, 100);

				if(attackCooldown > 0)
				{
                    attackCooldown -= Time.fixedDeltaTime;
                }

				yield return new WaitForFixedUpdate();
			}

			m_perception.StopLooking();
			StopThinking();
        }

        private AlertLevel DetemineAlertLevel()
        {
            Debug.Log(gameObject.name + " Alert Level: " + Mathf.Round(m_alertFloat));
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

        private void UpdateAlertLevel(float amount, Vector3 position)
        {
            m_alertFloat += amount;
            m_alertLocation = position;
        }
    }
}
