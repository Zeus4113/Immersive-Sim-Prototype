using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{

	public enum BehaviourState
	{
		passive,
		suspicious,
		alert
	}

	public class Behaviour : MonoBehaviour
	{
		[Header("Transforms")]
		[SerializeField] private Transform m_patrolPath;
		[SerializeField] private Transform m_playerTransform;
		[Space(2)]

		[Header("Floats")]
		[SerializeField] private float m_waitTime = 0f;
		[SerializeField] private float m_lingertime = 0f;
		[SerializeField] private float m_attackCooldown = 0f;

		private AlertActions m_alertActions;
		private SuspiciousActions m_suspiciousActions;
		private PassiveActions m_passiveActions;

		private GuardPerception1 m_guardPerception;

		private NavMeshAgent m_navMeshAgent;

		private BehaviourState m_behaviourState;

		private void Start()
		{
			m_alertActions = GetComponent<AlertActions>();
			m_suspiciousActions = GetComponent<SuspiciousActions>();
			m_passiveActions = GetComponent<PassiveActions>();
			m_navMeshAgent = GetComponent<NavMeshAgent>();
            m_guardPerception = GetComponentInChildren<GuardPerception1>();

            m_behaviourState = BehaviourState.passive;

			InitialiseScripts();
			StartThinking();
			UpdateBehaviour(m_behaviourState);
		}

		private void OnDrawGizmos()
		{
			Vector3 startPosition = m_patrolPath.GetChild(0).position;
			Vector3 previousPosition = startPosition;

			foreach (Transform waypoint in m_patrolPath)
			{
				Gizmos.DrawSphere(waypoint.position, .3f);
				Gizmos.DrawLine(previousPosition, waypoint.position);
				previousPosition = waypoint.position;
			}

			Gizmos.DrawLine(previousPosition, startPosition);
		}

		private void InitialiseScripts()
		{
			if(m_patrolPath == null) Debug.Log("Patrol path not set for: " + gameObject.name);

			m_passiveActions.Init(m_patrolPath, m_waitTime);
			m_alertActions.Init(m_attackCooldown);
			m_suspiciousActions.Init(m_lingertime);
		}

		bool c_isThinking = false;
		Coroutine c_thinking;

		void StartThinking()
		{
			if (c_isThinking) return;

			c_isThinking = true;

			if (c_thinking != null) return;

			c_thinking = StartCoroutine(Thinking());
		}

		void StopThinking()
		{
			if (!c_isThinking) return;

			c_isThinking = false;

			if(c_thinking == null) return;

			StopCoroutine(c_thinking);
			c_thinking = null;
		}

		void DetermineState(float alertLevel)
		{
            BehaviourState newState = BehaviourState.passive;


            switch (alertLevel)
			{
                case float x when (x == 0):
                    newState = BehaviourState.passive;
                    break;
                case float x when (x > 0 && x <= 100):
                    newState = BehaviourState.suspicious;
                    break;
                case float x when (x > 80 && x <= 100) && m_guardPerception.GetActualTarget() != null:
                    newState = BehaviourState.alert;
                    break;
			}

            if (m_behaviourState != newState)
            {
                m_behaviourState = newState;

                m_alertActions.StopAlerted();
                m_suspiciousActions.StopSuspicion();
                m_passiveActions.StopPatrol();

                UpdateBehaviour(m_behaviourState);
				Debug.Log("Behavior Updated: " + m_behaviourState);
            }
        }

		void UpdateBehaviour(BehaviourState newState)
		{
            switch (newState)
            {
                case BehaviourState.passive:
                    m_passiveActions.StartPatrol();
                    m_navMeshAgent.speed = 1.5f;
                    //Debug.Log("Passive");
                break;

                case BehaviourState.suspicious:
                    m_suspiciousActions.StartSuspicion(m_guardPerception.GetLastKnownLocation());
                    m_navMeshAgent.speed = 2f;
                    //Debug.Log("Suspicious");
                break;

                case BehaviourState.alert:
                    m_alertActions.StartAlerted(m_guardPerception.GetActualTarget());
                    m_navMeshAgent.speed = 3f;
                    //Debug.Log("Alert");

                break;
            }
        }

		IEnumerator Thinking()
		{
			while (c_isThinking)
			{
				DetermineState(m_guardPerception.GetAlertLevel());
				yield return new WaitForFixedUpdate();
			}

			StopThinking();
		}

	}
}

