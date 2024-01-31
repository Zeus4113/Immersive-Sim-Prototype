using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MissionSummary : MonoBehaviour
{
	[SerializeField] private MissionDataScriptableObject m_data;

	[SerializeField] private TMPro.TextMeshProUGUI m_gradeText, m_scoreText, m_alarmText, m_guardText;

	private void OnEnable()
	{
		if(m_data != null)
		{
			m_gradeText.text = m_data.grade.ToString();
			m_scoreText.text = Mathf.RoundToInt(m_data.scorePercentage).ToString() + "%";
			m_alarmText.text = m_data.alarmsTriggered.ToString();
			m_guardText.text = m_data.guardsAlerted.ToString();
		}

		Time.timeScale = 0f;
	}

	public void GotoMainMenu()
	{
		SceneManager.LoadScene("MainMenuScene");
	}

	private void OnDestroy()
	{
		Time.timeScale = 1f;
	}
}
