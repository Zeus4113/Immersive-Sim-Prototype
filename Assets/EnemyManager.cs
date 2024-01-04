using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
	private GameManager m_gameManager;

	private List<IBehaviourable> m_behaviors;

	public void Init(GameManager gm)
	{
		m_gameManager = gm;
		m_gameManager.GetCheckpointManager().resetCheckpoint += ResetEnemies;

		m_behaviors = new List<IBehaviourable>();

		for (int i = 0; i < transform.childCount; i++)
		{
			m_behaviors.Add(transform.GetChild(i).GetComponent<IBehaviourable>());
			m_behaviors[i].Init(this);
			m_behaviors[i].StartThinking();
		}
	}

	public void ResetEnemies()
	{
		for (int i = 0; i < m_behaviors.Count; i++)
		{
			m_behaviors[i].ResetBehaviour();
			m_behaviors[i].StartThinking();
		}
	}

	public void RemoveBehaviour(GameObject enemy)
	{
		if(enemy.GetComponent<IBehaviourable>() != null)
		{
			m_behaviors.Remove(enemy.GetComponent<IBehaviourable>());
		}
	}

}
