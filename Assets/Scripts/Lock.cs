using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lock : MonoBehaviour
{
	[SerializeField] private AudioClip m_unlockSound;
	[SerializeField] private AudioSource m_source;

	[SerializeField] private bool m_isLocked = false;
	[SerializeField] private bool m_requiresKey = false;
	[SerializeField] private string m_keyID = "";

	public void SetLocked(bool isLocked)
	{
		m_isLocked = isLocked;

		if (m_unlockSound != null && m_source != null)
		{
			m_source.PlayOneShot(m_unlockSound, 2);
		}

	}

	public bool GetLocked()
	{
		return m_isLocked;
	}

	public bool GetKeyRequired()
	{
		return m_requiresKey;
	}

	public bool CheckKeyID(string keyID)
	{
		if(keyID == m_keyID)
		{
			return true;
		}
		else
		{
			return false;
		}
	}
}
