using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public enum AlertLevel
    {
        passive,
        suspicious,
        alerted,
    }

    public class GuardBehaviour : MonoBehaviour
    {
        private GuardActions m_actions;
        private GuardPerception m_perception;
        private float m_alertFloat;
        private Vector3 m_alertLocation;

        private void Start()
        {
            m_alertLocation = Vector3.zero;
            m_alertFloat = 0;

            m_actions = GetComponent<GuardActions>();
            m_perception = GetComponent<GuardPerception>();
            m_perception.sightAlerted += UpdateAlertLevel;
            m_perception.StartLooking();
        }

        private void FixedUpdate()
        {
            m_alertFloat -= (Time.deltaTime * 3);
            m_alertFloat = Mathf.Clamp(m_alertFloat, 0, 100);
            Debug.Log(m_alertFloat.ToString());

            switch (DetemineAlertLevel())
            {
                case AlertLevel.passive:

                    m_actions.StartPatrolling();

                    break;

                case AlertLevel.suspicious:

                    if(m_alertLocation  != Vector3.zero)
                    {
                        m_actions.StartInvestigating(m_alertLocation);
                    }

                    break;

                case AlertLevel.alerted:

                    if (m_perception.GetPlayerRef() != null)
                    {
                        m_actions.StartPursuing(m_perception.GetPlayerRef());
                    }
                    else if (m_alertLocation != Vector3.zero)
                    {
                        m_actions.StartInvestigating(m_alertLocation);
                    }

                    break;
            }
        }

        private AlertLevel DetemineAlertLevel()
        {
            //Debug.Log("Alert Level: " + Mathf.Round(m_alertFloat));
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

        private void UpdateAlertLevel(float amount)
        {
            m_alertFloat += amount;
        }

        private void UpdateAlertLevel(float amount, Vector3 position)
        {
            m_alertFloat += amount;
            m_alertLocation = position;
        }
    }
}
