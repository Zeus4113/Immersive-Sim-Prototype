using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy 
{
	public class Perception : MonoBehaviour
	{
		[SerializeField] private float m_visibilityModifier = 0.1f;
		[SerializeField] private float m_volumeModifier = 30f;
		[SerializeField] private Transform m_eyeSocket;

		public event Disturbance perceptionAlerted;
		public event Disturbance perceptionLost;

		public event Disturbance perceptionTick;

		float m_updateTime = 0.1f;
		PerceptionDataScriptableObject m_data;

		float m_decayTime = 5f;
		Transform m_playerRef = null;

		bool c_isLooking = false;
		Coroutine c_looking;


		public void Init(PerceptionDataScriptableObject data)
		{
			m_data = data;
		}

		public void Init(PerceptionDataScriptableObject data, float updateTime)
		{
			m_data = data;
			m_updateTime = updateTime;
		}

		public Transform GetPlayerRef()
		{
			return m_playerRef;
		}

		public void StartLooking()
		{

			if (!m_data.canLook) return;

			if (c_isLooking) return;
			c_isLooking = true;

			if (c_looking != null) return;
			c_looking = StartCoroutine(Looking());
		}

		public void StopLooking()
		{
			if (!m_data.canLook) return;

			if (!c_isLooking) return;
			c_isLooking = false;

			if (c_looking == null) return;
			StopCoroutine(c_looking);
			c_looking = null;
		}

		IEnumerator Looking()
		{
			LayerMask mask = LayerMask.GetMask("Player", "DeadBodies");
			LayerMask rayMask = LayerMask.GetMask("Player", "DeadBodies", "Environment", "Interactables");


			while (c_isLooking)
			{
				if (m_playerRef != null) StartDecay();

				Collider[] colliders = Physics.OverlapSphere(transform.position, m_data.lookRange, mask);

				foreach (Collider c in colliders)
				{

					float angle;

					if (m_eyeSocket != null) angle = Vector3.Angle(-m_eyeSocket.up, c.transform.position - transform.position);
					else angle = Vector3.Angle(transform.forward, c.transform.position - transform.position);

					if (Mathf.Abs(angle) < m_data.fovAngle / 2)
					{
						RaycastHit hit;
						Physics.Raycast(transform.position, c.transform.position - transform.position, out hit, m_data.lookRange, rayMask);
						Debug.DrawRay(transform.position, c.transform.position - transform.position, Color.red, 2f);

						if (hit.collider == c)
						{
							if (hit.collider.transform.GetComponent<VisibilityCalculator>())
							{
								//Debug.LogWarning("Player Found");

								float visibility = hit.collider.GetComponent<VisibilityCalculator>().GetVisibility();

								if (visibility > m_data.lookThreshold)
								{
									perceptionAlerted?.Invoke((visibility * m_visibilityModifier), hit.transform.position);
									m_playerRef = hit.collider.transform;
								}

								if (m_data.hasPeripherals && Vector3.Distance(transform.position, hit.collider.transform.position) < m_data.peripheralDistance && visibility >= m_data.peripheralThreshold)
								{
									//Debug.LogWarning("Peripherals HIT!");
									perceptionAlerted?.Invoke((100), hit.transform.position);
									m_playerRef = hit.collider.transform;
								}

							}
							else if(hit.collider.gameObject.layer == LayerMask.NameToLayer("DeadBodies"))
							{
								perceptionAlerted?.Invoke((100), hit.transform.position);
							}

							StopDecay();
						}
					}
				}

				yield return new WaitForSeconds(m_updateTime);
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
			while (c_isDecaying)
			{
				yield return new WaitForSeconds(m_decayTime);

				perceptionAlerted?.Invoke(0, m_playerRef.position);
				m_playerRef = null;
				StopDecay();
			}
		}

		bool c_isListening = false;
		Coroutine c_listening;

		public void StartListening()
		{
			if (!m_data.canListen) return;

			if (c_isListening) return;
			c_isListening = true;

			if (c_listening != null) return;
			c_listening = StartCoroutine(Listening());
		}

		public void StopListening()
		{
			if (!m_data.canListen) return;

			if (!c_isListening) return;
			c_isListening = false;

			if (c_listening == null) return;
			StopCoroutine(c_listening);
			c_listening = null;
		}

		IEnumerator Listening()
		{
			LayerMask mask = LayerMask.GetMask("Player", "Sound Effects");
			LayerMask rayMask = LayerMask.GetMask("Player", "Sound Effects", "Environment", "Glass");


			while (c_isListening)
			{
				Collider[] colliders = Physics.OverlapSphere(transform.position, m_data.listenRange, mask);
				//Gizmos.DrawSphere(transform.position, m_data.listenRange);

				foreach (Collider c in colliders)
				{
					AudioSource source = c.GetComponent<AudioSource>();


					if(source != null)
					{
						if (source.isPlaying && source.clip != null)
						{
							float volume = source.volume;
							PlayerAudioManager audioManager = c.GetComponent<PlayerAudioManager>();

							RaycastHit hit;
							Physics.Raycast(transform.position, c.transform.position - transform.position, out hit, m_data.listenRange, rayMask);
							Debug.DrawRay(transform.position, c.transform.position - transform.position, Color.red, 2f);

							if (hit.collider != c) volume /= m_data.volumeFalloff;

							if (audioManager != null) volume *= audioManager.GetModifier();

							if (m_data.distanceFalloff)
							{
								volume = volume / Vector3.Distance(c.transform.position, transform.position);
							}

							if (volume > m_data.listenThreshold)
							{
								perceptionAlerted?.Invoke(volume * m_volumeModifier, c.transform.position); 
								perceptionTick?.Invoke(volume * m_volumeModifier, c.transform.position);
							}

							//Debug.Log(volume);

						}
					}
				}

				yield return new WaitForSeconds(m_updateTime);
			}
		}
	}
}


