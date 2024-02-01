using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
	[SerializeField] private float m_waitTime = 1f;

	public float GetWaitTime()
	{
		return m_waitTime;
	}
}
