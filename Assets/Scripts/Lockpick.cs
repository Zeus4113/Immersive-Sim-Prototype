using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
	public class Lockpick : MonoBehaviour
	{
		[SerializeField] private AudioClip m_lockpickingSound, m_lockedSound;
		[SerializeField] float m_lockpickTime = 6f;

		private Toolset m_toolset;
		private InteractIcon m_icon;

		public void Init(Toolset tools)
		{
			m_toolset = tools;
			m_icon = m_toolset.GetController().GetManager().GetUIManager().GetInteractIcon();
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

			m_icon.SetProgressWheel(0);
		}

		//IEnumerator Lockpicking()
		//{
		//	while (c_isLockpicking)
		//	{
		//		foreach(Lock l in m_locks)
		//		{
		//			if (!l.GetKeyRequired())
		//			{
		//				l.GetComponent<AudioSource>().PlayOneShot(m_lockpickingSound);
		//			}
		//			else if(l.GetKeyRequired() == true)
		//			{
		//				l.GetComponent<AudioSource>().PlayOneShot(m_lockedSound);
		//			}
		//		}

		//		yield return new WaitForSeconds(m_lockpickTime);

		//		foreach (Lock l in m_locks)
		//		{
		//			if (!l.GetKeyRequired())
		//			{
		//				l.SetLocked(false);
		//			}
		//		}

		//		StopLockpicking();
		//	}
		//}

		IEnumerator Lockpicking()
		{
			if (m_locks.Count <= 0) yield return null;

			float time = 0f;
			int lockedDoors = 0;

			foreach (Lock l in m_locks)
			{
				if (!l.GetKeyRequired() && l.GetLocked())
				{
					Debug.LogWarning(l.gameObject.name+ " Playing Sound");
					l.GetComponent<AudioSource>().PlayOneShot(m_lockpickingSound);
					lockedDoors++;
				}
				else if (l.GetKeyRequired() == true && l.GetLocked())
				{
					l.GetComponent<AudioSource>().PlayOneShot(m_lockedSound);
				}
			}

			while (c_isLockpicking && lockedDoors > 0)
			{
				time += Time.fixedDeltaTime;

				m_icon.SetProgressWheel(Mathf.Lerp(0, 1, time / m_lockpickTime));

				if(time > m_lockpickTime)
				{
					List<Lock> unlockedLocks = new List<Lock>();

					foreach (Lock l in m_locks)
					{
						if (!l.GetKeyRequired())
						{
							l.SetLocked(false);
							unlockedLocks.Add(l);
						}
					}

					foreach (Lock l in m_locks)
					{
						l.GetComponent<AudioSource>().Stop();
					}

					foreach (Lock l in unlockedLocks)
					{
						m_locks.Remove(l);
					}

					StopLockpicking();

				}

				yield return new WaitForFixedUpdate();
			}

			StopLockpicking();
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
