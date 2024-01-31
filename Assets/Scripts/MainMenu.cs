using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
	[SerializeField] private GameObject m_missionSelect;

	public void ShowMissionSelect()
	{
		m_missionSelect.SetActive(true);
		this.gameObject.SetActive(false);
	}

	public void LoadLevel(string levelName)
	{
		SceneManager.LoadScene(levelName);
	}

	public void QuitGame()
	{
		Application.Quit();
	}
}
