using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
	public class Lockpick : MonoBehaviour
	{
		[SerializeField] private AudioClip m_lockpickingSound, m_lockedSound;

		Toolset m_toolset;
		[SerializeField] float m_lockpickTime = 6f;

		public void Init(Toolset tools)
		{
			m_toolset = tools;
		}

		bool c_isLockpicking;
		Coroutine c_lockpicking;

		public void StartLockpicking()
		{
			if (c_isLockpicking) return;
			c_isLockpicking = true;

			if (c_lockpicking != null) return;
			c_lockpicking = StartCoroutine(Lockpicking());
		}

		public void StopLockpicking()
		{
			if (!c_isLockpicking) return;
			c_isLockpicking = false;

			if (c_lockpicking == null) return;
			StopCoroutine(c_lockpicking);
			c_lockpicking = null;

			foreach (Lock l in m_locks)
			{
				l.GetComponent<AudioSource>().Stop();
			}
		}

		IEnumerator Lockpicking()
		{
			while (c_isLockpicking)
			{
				foreach(Lock l in m_locks)
				{
					if (!l.GetKeyRequired())
					{
						l.GetComponent<AudioSource>().PlayOneShot(m_lockpickingSound);
					}
					else if(l.GetKeyRequired() == true)
					{
						l.GetComponent<AudioSource>().PlayOneShot(m_lockedSound);
					}
				}

				yield return new WaitForSeconds(m_lockpickTime);

				foreach (Lock l in m_locks)
				{
					if (!l.GetKeyRequired())
					{
						l.SetLocked(false);
					}
				}

				StopLockpicking();
			}
		}

		List<Lock> m_locks = new List<Lock>();

		private void OnTriggerEnter(Collider other)
		{
			Lock myLock = other.GetComponent<Lock>();

			if (myLock != null && !m_locks.Contains(myLock) && myLock.GetLocked() == true)
			{
				m_locks.Add(myLock);
			}
		}

		private void OnTriggerExit(Collider other)
		{
			Lock myLock = other.GetComponent<Lock>();

			if (myLock != null && m_locks.Contains(myLock))
			{
				m_locks.Remove(myLock);
			}
		}
	}
}
