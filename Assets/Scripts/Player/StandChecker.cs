using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandChecker : MonoBehaviour
{
	Player.Movement movementComponent;

	bool m_isBlocked = false;


	List<GameObject> m_list = new List<GameObject>();

	private void Awake()
	{
		movementComponent = gameObject.GetComponentInParent<Player.Movement>();
	}

	public bool IsBlocked()
	{
		return m_isBlocked;
	}

	private void OnTriggerExit(Collider other)
	{
		if (m_list.Contains(other.gameObject))
		{
			m_list.Remove(other.gameObject);
		}

		if(m_list.Count <= 0)
		{
			m_isBlocked = false;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!m_list.Contains(other.gameObject))
		{
			m_list.Add(other.gameObject);
		}

		if (m_list.Count > 0)
		{
			m_isBlocked = true;
		}
	}
}
