using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour, IInteractable
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

	public void Interact()
	{
		for (int i = 0; i < m_checkpoints.Length; i++)
		{
			if (m_checkpoints[i].GetComponent<Checkpoint>().GetActive())
			{
				m_activeCheckpoint = m_checkpoints[i];
			}
		}

		ResetPlayer();
	}

	void ResetPlayer()
	{
		if (m_activeCheckpoint != null)
		{
			GameObject player = m_gameManager.GetController().gameObject;

			player.transform.position = m_activeCheckpoint.transform.position;
			player.transform.rotation = m_activeCheckpoint.transform.rotation;

			resetCheckpoint?.Invoke();
		}
	}


}
