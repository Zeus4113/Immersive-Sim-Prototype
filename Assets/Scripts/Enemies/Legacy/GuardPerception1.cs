using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class GuardPerception1 : MonoBehaviour
{
    [Header("Field-of-View Variables")]
    [SerializeField] private float m_fieldOfVisionDistance;
    [Range(0,360)] [SerializeField] private float m_fieldOfVisionCone;
    [Space(2)]

    [Header("Visibility Thresholds")]
    [SerializeField] private float m_suspicionThreshold;
    [SerializeField] private float m_alertThreshold;

    private Transform m_playerPosition;
    private Vector3 m_investigationPosition;

    private Transform m_actualPositon;
    private Vector3 m_lastKnownLocation;

    private PlayerDetector m_playerDetector;

    [Range(0, 100)] private float m_alertLevel;

    //void OnDrawGizmos()
    //{
    //    Handles.color = new Color(0, 115, 0, 0.3f);
    //    Handles.DrawSolidArc(
    //        transform.position, 
    //        transform.up, 
    //        Quaternion.AngleAxis (-m_fieldOfVisionCone / 2, transform.up) * Vector3.forward, 
    //        m_fieldOfVisionCone,
    //        m_fieldOfVisionDistance
    //        );
    //}

    private void Start()
    {
        m_playerDetector = GetComponent<PlayerDetector>();
    }

    private void FixedUpdate()
    {
        //m_playerPosition = null;
        //m_investigationPosition = Vector3.zero;

        if (m_playerDetector.GetPlayerTransform())
        {
            m_actualPositon = m_playerDetector.GetPlayerTransform();
            m_lastKnownLocation = m_actualPositon.position;

            //Debug.Log("Player Location: " + m_actualPositon.position);

            if (m_actualPositon.GetComponent<VisibilityCalculator>())
            {
                VisibilityCalculator visibilityCalculator = m_actualPositon.GetComponent<VisibilityCalculator>();

                float visibilityLevel = visibilityCalculator.GetVisibility();

                m_alertLevel += visibilityLevel / 250;

                if (m_alertLevel > 100)
                {
                    m_alertLevel = 100;
                }
            }
        }

        m_alertLevel -= ( Time.fixedDeltaTime * 3 );

        Mathf.RoundToInt(m_alertLevel);

        if (m_alertLevel < 0)
        {
            m_alertLevel = 0;
        }

       Debug.Log("Alert Level: " + m_alertLevel);
    }

    public float GetAlertLevel()
    {
        return m_alertLevel;
    }

    public Transform GetActualTarget()
    {
        return m_actualPositon;
    }

    public Vector3 GetLastKnownLocation()
    {
        return m_lastKnownLocation;
    }
}
