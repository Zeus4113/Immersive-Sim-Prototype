using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour, ITriggerable
{
	public delegate void CheckpointEvent();
	public event CheckpointEvent resetCheckpoint;

	private Transform[] m_checkpoints;
	private Transform m_activeCheckpoint;

	private GameManager m_gameManager;

	public void Init(GameManager gm)
	{
		m_gameManager = gm;

		m_checkpoints = new Transform[transform.childCount];
		
		for(int i = 0; i < m_checkpoints.Length; i++)
		{
			m_checkpoints[i] = transform.GetChild(i);
		}
	}

	public void Trigger()
	{
		for (int i = 0; i < m_checkpoints.Length; i++)
		{
			if (m_checkpoints[i].GetComponent<Checkpoint>().GetActive())
			{
				m_activeCheckpoint = m_checkpoints[i];
			}
		}

        StartReseting();
	}

	bool c_isReseting;
	Coroutine c_reseting;

	void StartReseting()
	{
		if (c_isReseting) return;
		c_isReseting = true;

		if (c_reseting != null) return;
		c_reseting = StartCoroutine(ResetPlayer());
	}

	void StopReseting()
    {
        if (!c_isReseting) return;
        c_isReseting = false;

        if (c_reseting == null) return;
        StopCoroutine(c_reseting);
		c_reseting = null;

    }

	IEnumerator ResetPlayer()
	{
		while (c_isReseting)
		{
            yield return new WaitForSeconds(2f);

            if (m_activeCheckpoint != null)
            {
				m_gameManager.GetUIManager().ResetUI();
				m_gameManager.GetEnemyManager().ResetEnemies();

				GameObject player = m_gameManager.GetController().gameObject;

                player.transform.position = m_activeCheckpoint.transform.position;
                player.transform.rotation = m_activeCheckpoint.transform.rotation;

                resetCheckpoint?.Invoke();
				break;
            }
        }

		StopReseting();

	}


}
