using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace Enemy
{
	public delegate void Disturbance(float amount, Vector3 position);

	public class GuardPerception : MonoBehaviour
    {
        public event Disturbance sightAlerted;

        [Header("Field-of-View Variables")]
        [SerializeField] private float m_fieldOfVisionDistance;
        [Range(0, 360)][SerializeField] private float m_fieldOfVisionCone;
        [Space(2)]

        private Transform m_playerRef = null;
        private VisibilityCalculator m_visibilityCalculator;

        void OnDrawGizmos()
        {
            Handles.color = new Color(0, 115, 0, 0.3f);
            Handles.DrawSolidArc(
                this.transform.position + new Vector3(0, -1, 0),
                this.transform.up,
                Quaternion.AngleAxis(-m_fieldOfVisionCone / 2, this.transform.up) * this.transform.forward,
                m_fieldOfVisionCone,
                m_fieldOfVisionDistance
                );
        }

        // Looking Coroutine

        bool c_isLooking = false;
        Coroutine c_looking;

        public void StartLooking()
        {
            if (c_isLooking) return;
            c_isLooking = true;

            if (c_looking != null) return;
            c_looking = StartCoroutine(Looking());
        }
        public void StopLooking()
        {
            if (!c_isLooking) return;
            c_isLooking = false;

            if (c_looking == null) return;
            StopCoroutine(c_looking);
        }

        IEnumerator Looking()
        {
            LayerMask playerMask = LayerMask.GetMask("Player");

            while (c_isLooking)
            {
                Collider[] colliders = Physics.OverlapSphere(transform.position, m_fieldOfVisionDistance, playerMask);

                if(m_playerRef != null)
                {
                    //Debug.Log(m_playerRef.position);
                    StartDecay();
                }

                foreach (Collider collider in colliders)
                {

                    float angle = Vector3.Angle(transform.forward, collider.transform.position - transform.position);

                    if(Mathf.Abs(angle) < m_fieldOfVisionCone / 2)
                    {
                        if (collider.GetComponent<VisibilityCalculator>() != null)
                        {
                            Debug.Log("Player Detected");
                            m_playerRef = collider.transform;
                            m_visibilityCalculator = collider.GetComponent<VisibilityCalculator>();

                            if (collider.transform.position != null)
                            {
                                sightAlerted?.Invoke(m_visibilityCalculator.GetVisibility() / 100, collider.transform.position);
                                StopDecay();
                            }
                        }
                    }
                }

                yield return new WaitForFixedUpdate();
            }
        }

        // Decay Coroutine

        bool c_isDecaying = false;
        Coroutine c_decaying;

        void StartDecay()  
        {
            if (c_isDecaying) return;
            c_isDecaying = true;

            if (c_decaying != null) return;
            c_decaying = StartCoroutine(PlayerRefDecay());
        }

        void StopDecay()
        {
            if (!c_isDecaying) return;
            c_isDecaying = false;

            if (c_decaying == null) return;
            StopCoroutine(c_decaying);
            c_decaying = null;
        }

        IEnumerator PlayerRefDecay()
        {
            Debug.LogWarning("Decay Started");
            while(c_isDecaying)
            {

                yield return new WaitForSeconds(5);

                Debug.LogWarning("Transform Decayed");
                sightAlerted?.Invoke(0, m_playerRef.position);
                m_playerRef = null;
                m_visibilityCalculator = null;
                StopDecay();
            }
        }

        public Transform GetPlayerRef()
        {
            if (m_playerRef == null) return null; return m_playerRef;
        }

        void CacheVisibilityCalculator(VisibilityCalculator vc)
        {
            if(m_visibilityCalculator == null) m_visibilityCalculator = vc;
        }

    }
}
