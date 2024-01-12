using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{

	public class Behaviour : MonoBehaviour
	{
		EnemyManager m_manager;

		[SerializeField] private float m_updateTime = 0.1f;
		[SerializeField] private PerceptionDataScriptableObject m_perceptionData;
		[SerializeField] private Perception m_perception;
		[SerializeField] private Actions m_actions;

		private float m_suspicionThreshold = 40f;
		private float m_alertThreshold = 80f;

		private float m_alertFloat = 0f;
		private Vector3 m_lastAlertPosition = Vector3.zero;

		private void Awake()
		{
			if (m_perception == null) m_perception = gameObject.AddComponent<Perception>();
			if (m_actions == null) m_actions = gameObject.AddComponent<Actions>();

			Init(null);
		}

		public void Init(EnemyManager em)
		{
			m_manager = em;
			m_perception.Init(m_perceptionData, m_updateTime);
			m_perception.perceptionAlerted += UpdateAlertFloat;

			if(c_thinking != null) StopThinking();
			StartThinking();
		}

		bool c_isThinking;
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
			if(!c_isThinking) return;
			c_isThinking = false;

			if(c_thinking == null) return;
			StopCoroutine(c_thinking);
			c_thinking = null;
		}

		protected IEnumerator Thinking()
		{
			m_alertFloat = 0f;

			m_perception.StartListening();
			m_perception.StartLooking();

			while (c_isThinking)
			{
				switch (m_alertFloat)
				{
					case float x when x >= 0 && x < m_suspicionThreshold:
						m_actions.BecomePassive();
						break;

					case float x when x >= m_suspicionThreshold && x < m_alertThreshold:
						m_actions.BecomeSuspicous();
						break;

					case float x when x >= m_alertThreshold && x < 100:
						m_actions.BecomeAlerted();
						break;
				}

				//Debug.Log(gameObject.name + " alert level:" + m_alertFloat);

				if (m_alertFloat > 0f) m_alertFloat -= 1; 
				else m_alertFloat = 0f;

				yield return new WaitForSeconds(m_updateTime);
			}

			m_perception.StopListening();
			m_perception.StopLooking();

		}

		void UpdateAlertFloat(float amount, Vector3 position)
		{
			m_alertFloat = Mathf.Clamp((m_alertFloat += amount), 0 , 100);
			if(position != Vector3.zero) m_lastAlertPosition = position;
		}
	}
}
