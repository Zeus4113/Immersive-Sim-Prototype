using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
	public class Flashbang : MonoBehaviour
	{
		/// <summary>
		///  Tool initialisation
		/// </summary>

		private Toolset m_tools;
		private AudioSource m_audioSource;

		public void Init(Toolset tools)
		{
			m_tools = tools;
			m_currentFlashbangs = m_maxFlashbangs;
		}

		/// <summary>
		/// Weapon Functionallity
		/// </summary>
		/// 

		[SerializeField] private GameObject m_flashbangPrefab;
		[SerializeField] private float m_throwForce;

		[SerializeField] private int m_maxFlashbangs = 2;
		private int m_currentFlashbangs;

		public void ThrowFlashbang()
		{
			if (m_currentFlashbangs <= 0) return;

			GameObject newFlash = Instantiate(m_flashbangPrefab, transform.position, Quaternion.identity);
			newFlash.GetComponent<Rigidbody>().AddForce(transform.forward * m_throwForce, ForceMode.Impulse);
		}
	}
}
