using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathScreen : MonoBehaviour
{
	private void OnEnable()
	{
		Time.timeScale = 0f;
	}

	public void RestartLevel()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
	}

	public void LoadMenuScene()
	{
		SceneManager.LoadScene("MainMenuScene");
	}

	private void OnDestroy()
	{
		Time.timeScale = 1f;
	}
}
