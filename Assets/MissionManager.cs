using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MissionManager : MonoBehaviour
{
	private GameManager m_gameManager;
	private EnemyManager m_enemyManager;
	private LootManager m_lootManager;

	[SerializeField] float m_alarmPenalty = 5f;
	[SerializeField] float m_guardPenalty = 10f;

	[SerializeField] MissionDataScriptableObject m_data;

	public delegate void OnMissionEnd();

	public event OnMissionEnd missionComplete;

	public void Init(GameManager gm, EnemyManager em, LootManager lm)
	{
		m_gameManager = gm;
		m_lootManager = lm;
		m_enemyManager = em;

		m_data.scorePercentage = 0f;
		m_data.alarmsTriggered = 0;
		m_data.guardsAlerted = 0;
		m_data.grade = 'U';
	}

	void OnTriggerEnter(Collider other)
	{
		if (m_lootManager.CheckRequiredItems())
		{
			SetMissionData();
			m_gameManager.EnableInputEvents(false);
			missionComplete?.Invoke();
		}
	}

	void SetMissionData()
	{
		m_data.scorePercentage = m_lootManager.GetScore();

		m_data.alarmsTriggered = m_enemyManager.GetAlarmsTripped();

		m_data.guardsAlerted = m_enemyManager.GetGuardsAlerted();

		m_data.grade = CalculateGrade(m_data);

	}

	char CalculateGrade(MissionDataScriptableObject m)
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
