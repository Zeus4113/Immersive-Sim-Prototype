using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
	public class Keychain : MonoBehaviour
	{
		[SerializeField] private List<string> m_keyIDs = new List<string>();

		private Toolset m_tools;
		public void Init(Toolset tools)
		{
			m_tools = tools;
		}

		public void AddKey(string keyID)
		{
			if (m_keyIDs.Contains(keyID)) return;
			m_keyIDs.Add(keyID);
		}

		public void RemoveKey(string keyID)
		{
			if (keyID.ToLower() == "all") m_keyIDs.Clear();
			m_keyIDs.Remove(keyID);
		}

		public void UseKeychain()
		{
			if (m_tools == null) return;

			foreach(Lock l in m_locks)
			{
				for(int i = 0; i < m_keyIDs.Count; i++)
				{
					if (l.GetKeyRequired() && l.CheckKeyID(m_keyIDs[i]) && l.GetLocked())
					{
						Debug.LogWarning(m_keyIDs[i]);
						l.SetLocked(false);
					}
				}
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
