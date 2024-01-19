using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct MissionData
{
	public float scorePercentage;
	public int alarmsTriggered;
	public int guardsAlerted;
}

public class MissionManager : MonoBehaviour
{
	[SerializeField] EnemyManager m_enemyManager;
	[SerializeField] LootManager m_lootManager;

	[SerializeField] float m_alarmPenalty = 5f;
	[SerializeField] float m_guardPenalty = 10f;

	MissionData m_missionData;

	void Init()
	{
		m_missionData = new MissionData();
	}

	void OnTriggerEnter(Collider other)
	{
		if (m_lootManager.CheckRequiredItems())
		{
			SetMissionData();

			char grade = CalculateGrade(GetMissionData());

			Debug.LogWarning("Mission Completed:" +
				"/n Grade : " + grade +
				"/n Loot Percentage : " + m_missionData.scorePercentage +
				"/n Alarms Triggered : " + m_missionData.alarmsTriggered +
				"/n Guards Alerted : " + m_missionData.guardsAlerted);
		}

		else Debug.LogWarning("Required Items Not Found!");

	}

	MissionData GetMissionData()
	{
		return m_missionData;
	}

	void SetMissionData()
	{
		m_missionData.scorePercentage = m_lootManager.GetScore();
		Debug.LogWarning("Score: " + m_lootManager.GetScore());

		m_missionData.alarmsTriggered = m_enemyManager.GetAlarmsTripped();
		Debug.LogWarning("Alarms: " + m_enemyManager.GetAlarmsTripped());

		m_missionData.guardsAlerted = m_enemyManager.GetGuardsAlerted();
		Debug.LogWarning("Guards: " + m_enemyManager.GetGuardsAlerted());

	}

	char CalculateGrade(MissionData m)
	{
		float score = m.scorePercentage;
		
		for(int i = 0; i < m.alarmsTriggered; i++)
		{
			score -= m_alarmPenalty;
		}

		for(int i = 0; i < m.guardsAlerted; i++)
		{
			score -= m_guardPenalty;
		}

		switch (score)
		{
			case float x when x >= 0 && x < 50:
				return 'F';

			case float x when x >= 50 && x < 60:
				return 'E';

			case float x when x >= 60 && x < 70:
				return 'D';

			case float x when x >= 70 && x < 80:
				return 'C';

			case float x when x >= 80 && x < 90:
				return 'B';

			case float x when x >= 90 && x <= 100:
				return 'A';

			default:
				return 'U';
		}
	}
}
