using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{

	private bool m_isActive;

	public void Init()
	{
		m_isActive = false;
	}

	public bool GetActive()
	{
		return m_isActive;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!other.GetComponent<Player.Controller>()) return;

		m_isActive = true;

	}
}
