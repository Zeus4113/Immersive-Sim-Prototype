using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
	public class Whacker : MonoBehaviour
	{
		/// <summary>
		///  Tool initialisation
		/// </summary>

		private Toolset m_tools;
		private InteractIcon m_icon;

		public void Init(Toolset tools)
		{
			m_tools = tools;
			m_icon = m_tools.GetController().GetManager().GetUIManager().GetInteractIcon();
		}

		/// <summary>
		/// Weapon Functionallity
		/// </summary>


		[SerializeField] private bool m_isCharged = false;
		[SerializeField] private float m_chargeTime = 3f;
		[SerializeField] private AudioSource m_audioSource = null;
		[SerializeField] private AudioClip m_hitSound, m_missSound, m_chargedSound;

		bool c_isCharging = false;
		Coroutine c_charging;

		public void StartCharge()
		{
			if (c_isCharging) return;
			c_isCharging = true;

			if (c_charging != null) return;
			c_charging = StartCoroutine(Charging());
		}

		public void StopCharge()
		{
			if (!c_isCharging) return;
			c_isCharging = false;

			if (c_charging == null) return;
			StopCoroutine(c_charging);
			c_charging = null;
		}

		IEnumerator Charging()
		{
			Debug.Log("Charging Started");

			float time = 0f;

			while (c_isCharging)
			{
				time += Time.fixedDeltaTime;

				m_icon.SetProgressWheel(Mathf.Lerp(0, 1, time));

				if(time > m_chargeTime)
				{
					Debug.Log("Charged");
					m_isCharged = true;
					m_audioSource.PlayOneShot(m_chargedSound , 0.25f);
					break;
				}

				yield return new WaitForFixedUpdate();

			}

			if (!m_isCharged)
			{
				m_icon.SetProgressWheel(0);
			}

			StopCharge();
		}

		public void Attack()
		{
			Debug.Log("Attacking");

			if (m_isCharged && m_target != null)
			{
				m_audioSource.PlayOneShot(m_hitSound , 1f);
				m_target.GetComponentInParent<Enemy.GuardActions>().GuardUnconcious(m_tools.GetController().gameObject);
				m_isCharged = false;
			}
			else if(m_isCharged && m_target == null)
			{
				m_audioSource.PlayOneShot(m_missSound, 0.5f);
				m_isCharged = false;
			}

			m_icon.SetProgressWheel(0);
		}


		GameObject m_target;

		private void OnTriggerEnter(Collider other)
		{
			m_target = other.gameObject;
		}

		private void OnTriggerExit(Collider other)
		{
			if (other.gameObject != m_target) return;
			m_target = null;
		}
	}
}
