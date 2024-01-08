using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundedChecker : MonoBehaviour
{
	Player.Movement movementComponent;

	private string m_currentFloorTag;

	private void Awake()
	{
		movementComponent = gameObject.GetComponentInParent<Player.Movement>();
	}

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

        movementComponent.CheckGrounded(true);
    }

	void StopChecking()
	{
        if (!c_isChecking) return;
        c_isChecking = false;

        if (c_checking == null) return;
        StopCoroutine(c_checking);
        c_checking = null;

        movementComponent.CheckGrounded(false);
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

		

  //      if (other.gameObject != this.transform.parent.gameObject)
		//{
		//	movementComponent.CheckGrounded(false);
		//	m_currentFloorTile = null;

  //      }
	}

	private void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.layer == 3)
		{
            m_colliders.Add(other);
			StartChecking();

        }
        //if (other.gameObject != this.transform.parent.gameObject)
        //{
        //    movementComponent.CheckGrounded(false);
        //    m_currentFloorTile = null;

        //}
    }

	//private void OnTriggerStay(Collider other)
	//{
	//	if (other.gameObject != this.transform.parent.gameObject)
	//	{
	//		movementComponent.CheckGrounded(true);
	//	}
	//}
}
