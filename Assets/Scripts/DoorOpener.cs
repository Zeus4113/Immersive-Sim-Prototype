using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpener : MonoBehaviour
{
	List<Door> m_doors = new List<Door>();

	[SerializeField] private float m_waitTime = 2f;

	bool c_isOpening = false;
	Coroutine c_opening;

	void StartOpening()
	{
		Debug.LogWarning("Started Opening");
		if (c_isOpening) return;
		c_isOpening = true;

		if (c_opening != null) return;
		c_opening = StartCoroutine(Opening());
	}

	void StopOpening()
	{
		Debug.LogWarning("Stopped Opening");
		if (!c_isOpening) return;
		c_isOpening = false;

		if (c_opening == null) return;
		StopCoroutine(c_opening);
		c_opening = null;
	}

	IEnumerator Opening()
	{
		while (c_isOpening && m_doors.Count > 0)
		{
			yield return new WaitForSeconds(m_waitTime);

			foreach(Door door in m_doors)
			{
				Debug.Log("Opening Doors");
				door.EnemyInteract();
				StartCoroutine(AutoClose(door));
				StopOpening();
			}
		}

		StopOpening();
	}

	IEnumerator AutoClose(Door newDoor)
	{
		yield return new WaitForSeconds(5.5f);
		newDoor.EnemyInteract();
		Debug.Log("Auto Closing");

	}

	private void OnTriggerEnter(Collider other)
	{
		if (other == null) return;

		Debug.LogWarning(other.gameObject);

		if (other.GetComponent<Door>())
		{
			m_doors.Add(other.GetComponent<Door>());
			StartOpening();
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other == null) return;

		//Debug.LogWarning(other.gameObject);

		if (other.GetComponent<Door>() && m_doors.Contains(other.GetComponent<Door>()))
		{
			m_doors.Remove(other.GetComponent<Door>());

			if(m_doors.Count !> 0)
			{
				StopOpening();
			}
		}
	}
}
