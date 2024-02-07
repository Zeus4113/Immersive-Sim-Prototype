using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
	public class Dartgun : MonoBehaviour
	{
		/// <summary>
		///  Tool initialisation
		/// </summary>

		private Toolset m_tools;
		private AudioSource m_audioSource;

		public void Init(Toolset tools)
		{
			m_tools = tools;
			m_currentDarts = m_maxDarts;
			m_audioSource = GetComponent<AudioSource>();
		}

		/// <summary>
		/// Weapon Functionallity
		/// </summary>
		/// 

		[SerializeField] private AudioClip m_fireSound, m_emptySound;
		[SerializeField] private int m_maxDarts = 3;
		private int m_currentDarts;

		public void Fire()
		{
			if (!m_canFire) return;

			if (m_currentDarts <= 0)
			{
				m_audioSource.PlayOneShot(m_emptySound, 0.5f);
				return;
			}

			m_currentDarts--;
			m_audioSource.PlayOneShot(m_fireSound, 1f);
			StartCoroutine(FireCooldown());

			LayerMask layerMask = LayerMask.GetMask("Environment", "Enemies", "Glass", "Interactables");

			RaycastHit hit;

			Physics.Raycast(transform.position, transform.forward, out hit, 100f, layerMask);
			Debug.DrawRay(transform.position, transform.forward * 100f, Color.red, 5f);

			if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Enemies"))
			{
				hit.collider.gameObject.GetComponentInParent<Enemy.GuardActions>().StartCountdown();
			}
		}

		private bool m_canFire = true;
		private float m_cooldownDuration = 1.2f;

		IEnumerator FireCooldown()
		{
			m_canFire = false;

			yield return new WaitForSeconds(m_cooldownDuration);

			m_canFire = true;
		}
	}
}
