using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
	private GameManager m_gameManager;

	private List<GameObject> m_enemies;

	public void Init(GameManager gm)
	{
		m_gameManager = gm;
		m_gameManager.GetCheckpointManager().resetCheckpoint += ResetEnemies;

		m_enemies = new List<GameObject>();

		for (int i = 0; i < transform.childCount; i++)
		{
			m_enemies.Add(transform.GetChild(i).gameObject);
		}
	}

	public void ResetEnemies()
	{
		for (int i = 0; i < m_enemies.Count; i++)
		{
			m_enemies[i].GetComponent<IAlertable>().StopAlerted();
		}
	}

}
