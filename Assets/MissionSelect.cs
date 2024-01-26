using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MissionSelect : MonoBehaviour
{
	[SerializeField] private GameObject[] m_missions;

	[SerializeField] private GameObject m_mainMenu;

	private GameObject m_shownMission = null;
	private int m_index = 0;

	private void OnEnable()
	{
		ShowMissionSelect();
		ShowMission();

		foreach(GameObject go in m_missions)
		{
			Debug.Log(go);
		}
	}

	void ShowMission()
	{
		if (m_shownMission == null) Debug.Log("Mission Null");

		foreach(GameObject go in m_missions)
		{
			if (go != null)
			{
				go.SetActive(false);
			}
		}

		m_shownMission.SetActive(true);
	}
	public void ShowMainMenu()
	{
		m_mainMenu.SetActive(true);
		this.gameObject.SetActive(false);
	}

	public void LoadLevel(string levelName)
	{
		SceneManager.LoadScene(levelName);
	}

	public void ShowMissionSelect()
	{
		m_shownMission = m_missions[m_index];
	}

	public void NextMission()
	{
		if (m_index < m_missions.Length - 1)
		{
			m_index++;
		}
		else
		{
			m_index = 0;
		}

		m_shownMission = m_missions[m_index];

		ShowMission();
	}

	public void PreviousMission()
	{
		if (m_index > 0)
		{
			m_index--;
		}
		else
		{
			m_index = m_missions.Length - 1;
		}

		m_shownMission = m_missions[m_index];
		ShowMission();
	}

}
