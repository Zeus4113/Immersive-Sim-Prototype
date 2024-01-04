using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
	public class CameraPerception : MonoBehaviour
	{
		[SerializeField] private float m_cameraPerceptionModifier = 1f;

		private CameraBehaviour m_behaviour;

		public event Disturbance sightAlerted;

		[SerializeField] private Transform m_cameraLens;
		private GameObject m_playerRef;
		private float m_updateTime;

		public void Init(CameraBehaviour cb, float updateTime)
		{
			m_behaviour = cb;
			m_updateTime = updateTime;
			m_playerRef = null;
			m_isLooking = false;
		}

		bool m_isLooking = false;
		Coroutine m_looking;

		void StartLooking()
		{
			if (m_isLooking) return;
			m_isLooking = true;

			if (m_looking != null) return;
			m_looking = StartCoroutine(Looking());
		}

		void StopLooking()
		{
			if (!m_isLooking) return;
			m_isLooking = false;

			if (m_looking == null) return;
			StopCoroutine(m_looking);
			m_looking = null;
		}

		IEnumerator Looking()
		{
			LayerMask mask = LayerMask.GetMask("Player", "Environment");

			while (m_isLooking)
			{

				if (m_playerRef != null)
				{
					RaycastHit hit;
					Physics.Raycast(m_cameraLens.position, m_playerRef.transform.position - m_cameraLens.transform.position, out hit, 100f, mask);
					Debug.DrawRay(m_cameraLens.position, m_playerRef.transform.position - m_cameraLens.transform.position, Color.red, 5f);

					if (hit.collider != null)
					{

						if (hit.collider.gameObject == m_playerRef)
						{
							//Debug.Log(m_playerVisibilityCalculator.GetVisibility() * m_cameraPerceptionModifier);
							sightAlerted(m_playerVisibilityCalculator.GetVisibility() * m_cameraPerceptionModifier, hit.transform.position);
						}
					}
				}

				yield return new WaitForSeconds(m_updateTime);
			}
		}

		public bool CanSeePlayer()
		{
			if(m_playerRef == null) return false;

			RaycastHit hit;
			Physics.Raycast(m_cameraLens.position, m_playerRef.transform.position - m_cameraLens.transform.position, out hit, 100f, LayerMask.GetMask("Player", "Environment"));

			if (hit.collider.gameObject == m_playerRef)
			{
				return true;
			}

			return false;

		}

		VisibilityCalculator m_playerVisibilityCalculator;

		private void CachePlayerComponents(GameObject player)
		{
			if (player == null)
			{
				m_playerVisibilityCalculator = null;
				return;
			}

			m_playerVisibilityCalculator = player.GetComponent<VisibilityCalculator>();

		}

		private void OnTriggerEnter(Collider other)
		{
			Debug.Log(gameObject.name + " Triggered (Enter)");
			if (other.GetComponent<Player.Controller>() != null)
			{
				m_playerRef = other.gameObject;
				CachePlayerComponents(m_playerRef);
				StartLooking();
			}
		}

		private void OnTriggerExit(Collider other)
		{
			Debug.Log(gameObject.name + " Triggered (Exit)");
			if (other.gameObject == m_playerRef)
			{
				m_playerRef = null;
				CachePlayerComponents(m_playerRef);
				StopLooking();
			}
		}
	}
}