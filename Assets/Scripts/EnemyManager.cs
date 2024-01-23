using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy;

public class EnemyManager : MonoBehaviour
{
	private GameManager m_gameManager;

	private List<GameObject> m_enemies;

	private int m_alarmsTripped = 0;
	private int m_guardsAlerted = 0;

	public void Init(GameManager gm)
	{
		m_gameManager = gm;
		m_gameManager.GetCheckpointManager().resetCheckpoint += ResetEnemies;

		m_enemies = new List<GameObject>();

		for (int i = 0; i < transform.childCount; i++)
		{
			m_enemies.Add(transform.GetChild(i).gameObject);
		}

		BindEvents();
		InitialiseEnemies();
	}

	public void InitialiseEnemies()
	{
		foreach (GameObject enemy in m_enemies)
		{
			if (enemy.GetComponent<IAlertable>() != null)
			{
				IAlertable alertable = enemy.GetComponent<IAlertable>();
				alertable.Init(this);
			}
		}
	}

	public void BindEvents()
	{
		foreach(GameObject enemy in m_enemies)
		{
			if(enemy.GetComponent<Alarm>() != null || enemy.GetComponent<GuardBehaviour>() != null)
			{
				IAlertable alertable = enemy.GetComponent<IAlertable>();
				alertable.alertTriggered += RegisterAlert;
			}
		}
	}

	public void RemoveEnemy(GameObject enemy)
	{
		IAlertable alertable = enemy.GetComponent<IAlertable>();
		alertable.alertTriggered -= RegisterAlert;
		m_enemies.Remove(enemy);
		Destroy(enemy);
	}

	public void ResetEnemies()
	{
		for (int i = 0; i < m_enemies.Count; i++)
		{
			m_enemies[i].GetComponent<IAlertable>().StopAlerted();

			if (m_enemies[i].GetComponent<GuardBehaviour>())
			{
				m_enemies[i].GetComponent<IAlertable>().StartAlerted();
			}
		}
	}

	public void RegisterAlert(GameObject alertedObject)
	{
		if (alertedObject != null) return;

		if (alertedObject.GetComponent<GuardBehaviour>()) m_guardsAlerted++;

		if(alertedObject.GetComponent<Alarm>()) m_alarmsTripped++;
	}

	public int GetAlarmsTripped()
	{
		return m_alarmsTripped;
	}

	public int GetGuardsAlerted()
	{
		return m_guardsAlerted;
	}
}
