using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundedChecker : MonoBehaviour
{
	private string m_currentFloorTag;

	public string GetTag()
	{
		return m_currentFloorTag;
	}

	bool c_isChecking = false;
	Coroutine c_checking;

	void StartChecking()
	{
		if(c_isChecking) return;
		c_isChecking = true;

		if (c_checking != null) return;
		c_checking = StartCoroutine(Checking());
    }

	void StopChecking()
	{
        if (!c_isChecking) return;
        c_isChecking = false;

        if (c_checking == null) return;
        StopCoroutine(c_checking);
        c_checking = null;
    }

	IEnumerator Checking()
	{
		while (c_isChecking)
		{
            foreach(Collider x in m_colliders)
			{
				if(x.gameObject.tag != "")
				{
                    m_currentFloorTag = x.gameObject.tag;
                    //Debug.Log(x.gameObject.tag);
                }
            }

			//Debug.Log(m_colliders.Count);

            yield return new WaitForFixedUpdate();
		}
	}

	List<Collider> m_colliders = new List<Collider>();

	public bool IsGrounded()
	{
		if (m_colliders.Count > 0) return true;
		return false;
	}

	private void OnTriggerExit(Collider other)
	{
        if (other.gameObject.layer == 3)
        {
            m_colliders.Remove(other);

			if(m_colliders.Count == 0)
			{
				StopChecking();
			}
        }
	}

	private void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.layer == 3)
		{
            m_colliders.Add(other);
			StartChecking();
        }
    }
}
