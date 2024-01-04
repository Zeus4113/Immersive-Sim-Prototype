using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
	public delegate void EnemyAlerted();

	public class CameraBehaviour : MonoBehaviour, IBehaviourable
	{
		public event EnemyAlerted enemyAlerted;

		private EnemyManager m_enemyManager;

		[Header("Float Variables")]

		[SerializeField] private float m_suspicionThreshold = 33;
		[SerializeField] private float m_alertThreshold = 66;
		[SerializeField] private float m_alertFalloffModifier = 1f;
		[SerializeField] private float m_updateTime = 0.1f;

		[Space(2)]

		[Header("Interactables")]

		[SerializeField] private GameObject[] m_interactables;

		[Space(2)]

		[Header("Components")]

		[SerializeField] private CameraPerception m_perception;
		[SerializeField] private CameraActions m_actions;

		[Space(2)]

		private float m_alertFloat;

		public void Init(EnemyManager em)
		{
			m_enemyManager = em;
			m_alertFloat = 0f;
			c_isThinking = false;

			if (m_perception != null)
			{
				m_perception.Init(this, m_updateTime);
				m_perception.sightAlerted += UpdateAlertLevel;
			}

			if (m_actions != null)
			{
				m_actions.Init(this);
			}

		}

		public void ResetBehaviour()
		{
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
			while (c_isThinking)
			{
				if (m_perception.CanSeePlayer() && m_alertFloat > m_suspicionThreshold)
				{
					m_actions.PlaySound();
				}

				switch (DetermineAlertLevel())
				{
					case AlertLevel.passive:
						m_actions.CameraPassive();
						break;

					case AlertLevel.suspicious:
						m_actions.CameraSuspicious();
						break;

					case AlertLevel.alerted:
						m_actions.CameraAlerted();
						m_actions.TriggerInteractables(m_interactables);
						break;
				}

				if (m_alertFloat > 0)
				{
					m_alertFloat -= m_updateTime * m_alertFalloffModifier;
				}

				yield return new WaitForSeconds(m_updateTime);
			}

			StopThinking();
		}



		void UpdateAlertLevel(float amount, Vector3 position)
		{
			m_alertFloat += amount;
			m_alertFloat = Mathf.Clamp(m_alertFloat, 0 , 100);
		}

		private AlertLevel DetermineAlertLevel()
		{
			Debug.Log(m_alertFloat);
			switch (m_alertFloat)
			{
				case float x when x >= 0 && x < m_suspicionThreshold:
					return AlertLevel.passive;

				case float x when x >= m_suspicionThreshold && x <= m_alertThreshold:
					return AlertLevel.suspicious;

				case float x when x > m_alertThreshold && x <= 100:
					return AlertLevel.alerted;

				default:
					return 0;
			}
		}
	}
}

